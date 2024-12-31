using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Request.Models;
using Bau.Libraries.LibReporting.Application.Exceptions;
using Bau.Libraries.LibReporting.Models.Base;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries;

/// <summary>
///		Generador de consultas SQL para un informe
/// </summary>
internal class ReportQueryGenerator
{
	internal ReportQueryGenerator(RequestModel request) 
	{
		Request = request;
	}

	/// <summary>
	///		Obtiene la cadena SQL de un informe para responder a una solicitud
	/// </summary>
	internal string GetSql()
	{
		if (Request.Report is not ReportModel reportAdvanced)
			throw new ReportingParserException($"Unknown report {Request.Report.Id}. {Request.Report.GetType().ToString()}");
		else
		{
			// Inicializa el almacén de datos
			Request.Manager.Schema.Initialize();
			// Normaliza la solicitud
			NormalizeRequest(Request, reportAdvanced);
			// Devuelve la SQL generada
			return GetSql(GetQueries(reportAdvanced.Blocks));
		}
	}

	/// <summary>
	///		Normaliza la solicitud añadiendo las dimensiones que aparezcan en el informe como obligatorios
	///	pero para las que no se haya añadido ningún campo
	/// </summary>
	private void NormalizeRequest(RequestModel request, ReportModel report)
	{
		foreach (ReportRequestDimension fixedRequest in report.RequestDimensions)
			if (fixedRequest.Required || CheckIsRequestedAnyField(request, fixedRequest))
				foreach (ReportRequestDimensionField field in fixedRequest.Fields)
					request.AddDimension(fixedRequest.DimensionKey, field.Field, false);

		// Comprueba si se ha solicitado alguno de los campos considerados como obligatorios
		bool CheckIsRequestedAnyField(RequestModel reportRequest, ReportRequestDimension fixedRequest)
		{
			RequestDimensionModel? dimensionRequest = reportRequest.GetRequestedDimension(fixedRequest.DimensionKey);

				// Si se ha solicitado la dimensión
				if (dimensionRequest is not null)
					foreach (ReportRequestDimensionField field in fixedRequest.Fields)
						if (dimensionRequest.GetRequestColumn(field.Field) is not null)
							return true;
				// Si ha llegado hasta aquí es porque no se ha solicitado
				return false;
		}
	}

	/// <summary>
	///		Obtiene la SQL asociada a una serie de consultas
	/// </summary>
	private string GetSql(List<QuerySqlModel> queries)
	{
		string sql = string.Empty;

			// Obtiene la cadena de la consulta
			foreach (QuerySqlModel query in queries)
				switch (query.Type)
				{
					case QuerySqlModel.QueryType.Block:
							// Si hay alguna CTE se pone el WITH (puede que no se solicite ninguna dimensión)
							if (query.ExistsCte())
								sql += "WITH" + Environment.NewLine;
							// Añade las consultas
							sql += GetSql(query.Queries);
						break;
					case QuerySqlModel.QueryType.Cte:
							int index = queries.IndexOf(query);

								// Añade la Cte a la consulta
								sql += query.Key + " AS " + Environment.NewLine;
								sql += "(" + Environment.NewLine + query.Sql + Environment.NewLine +  ")";
								// Añade una coma si es necesario
								if (index < queries.Count - 1 && queries[index + 1].Type == QuerySqlModel.QueryType.Cte)
									sql += ",";
								// Añade un salto de una línea
								sql += Environment.NewLine;
						break;
					case QuerySqlModel.QueryType.Execution:
					case QuerySqlModel.QueryType.Query:
							sql += query.Sql;
						break;
				}
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Obtiene la cadena SQL de una serie de bloques
	/// </summary>
	private List<QuerySqlModel> GetQueries(List<BaseBlockModel> blocks)
	{
		List<QuerySqlModel> queries = [];

			// Genera el SQL de los bloques
			foreach (BaseBlockModel blockBase in blocks)
				switch (blockBase)
				{
					case BlockWithModel block:
							queries.Add(GetQuery(block));
						break;
					case BlockExecutionModel block:
							queries.Add(GetQuery(block));
						break;
					case BlockQueryModel block:
							queries.Add(GetQuery(block, queries));
						break;
					case BlockCreateCteDimensionModel block:
							queries.Add(GetQuery(block));
						break;
					case BlockCreateCteModel block:
							queries.Add(GetQuery(block));
						break;
					case BlockIfRequest block:
							queries.AddRange(GetQueries(block));
						break;
					default:
						throw new ReportingParserException($"Unknown block type {blockBase.Key}. {blockBase.GetType().ToString()}");
				}
			// Devuelve la lista de consultas
			return queries;
	}

	/// <summary>
	///		Obtiene la cadena SQL de un bloque With
	/// </summary>
	private QuerySqlModel GetQuery(BlockWithModel block)
	{
		QuerySqlModel query = new(QuerySqlModel.QueryType.Block, block.Key, string.Empty);

			// Interpreta los bloques
			query.Queries.AddRange(GetQueries(block.Blocks));
			// Devuelve la consulta
			return query;
	}

	/// <summary>
	///		Obtiene la SQL de un bloque de ejecución
	/// </summary>
	private QuerySqlModel GetQuery(BlockExecutionModel block) => new(QuerySqlModel.QueryType.Execution, block.Key, block.Sql);

	/// <summary>
	///		Obtiene la SQL de un bloque de consulta
	/// </summary>
	private QuerySqlModel GetQuery(BlockQueryModel block, List<QuerySqlModel> queriesBlock)
	{
		ParserSection parser = new(block.Sql);

			// Convierte las secciones
			foreach ((string marker, ParserBaseSectionModel section) in parser.Parse())
				switch (section)
				{ 
					case ParserFieldsSectionModel item:
							parser.Replace(marker, new Generators.QueryFieldsGenerator(this, item).GetSql());
						break;
					case ParserJoinSectionModel item:
							parser.Replace(marker, new Generators.QueryRelationGenerator(this, item).GetSql());
						break;
					case ParserGroupBySectionModel item:
							parser.Replace(marker, GetSqlForGroupBy(item));
						break;
					case ParserOrderBySectionModel item:
							parser.Replace(marker, GetSqlForOrderBy(item));
						break;
					case ParserPartitionBySectionModel item:
							parser.Replace(marker, GetSqlForPartitionBy(item));
						break;
					case ParserIfRequestSectionModel item:
							parser.Replace(marker, new Generators.QueryIfRequestGenerator(this, item).GetSql());
						break;
					case ParserCondiciontSectionModel item:
							parser.Replace(marker, new Generators.QueryConditionsGenerator(this, item).GetSql());
						break;
					case ParserSubquerySectionModel item:
							parser.Replace(marker, new Generators.QuerySubqueryGenerator(this, item, queriesBlock).GetSql());
						break;
					case ParserPaginationSectionModel item:
							parser.Replace(marker, new Generators.QueryPaginationGenerator(this, item).GetSql());
						break;
					default:
						throw new ReportingParserException($"Unknown section: {section.GetType().ToString()}");
				}
			// Quita los marcadores vacíos
			parser.RemoveMarkers();
			// Devuelve la consulta
			return new(QuerySqlModel.QueryType.Query, block.Key, parser.Sql);
	}

	/// <summary>
	///		Crea la SQL de un bloque de CTE a partir de una dimensión
	/// </summary>
	private QuerySqlModel GetQuery(BlockCreateCteDimensionModel block)
	{
		BaseDimensionModel? dimension = Request.Report.DataWarehouse.Dimensions[block.DimensionKey];

			if (dimension is null)
				throw new ReportingParserException($"Can't find the dimension {block.DimensionKey}");
			else
			{
				string sql = GetSqlDimension(dimension, block.Fields);
				
					// Añade los filtros adicionales a la consulta
					sql += Environment.NewLine + GetSqlForFilters(block.Filters);
					// Devuelve la consulta
					return new QuerySqlModel(QuerySqlModel.QueryType.Cte, block.Key, sql);
			}
	}

	/// <summary>
	///		Crea la SQL de un bloque de CTE
	/// </summary>
	private QuerySqlModel GetQuery(BlockCreateCteModel block)
	{
		List<QuerySqlModel> queries = GetQueries(block.Blocks);
		string sql = string.Empty;

			// Añade los valores de las consultas a la CTE
			foreach (QuerySqlModel query in queries)
				if (!query.IsSubquery)
					sql = sql.AddWithSeparator(query.Sql, Environment.NewLine, false);
			// Devuelve el resultado de la consulta
			if (string.IsNullOrWhiteSpace(sql))
				throw new ReportingParserException($"There is no SQL query in CTE block '{block.Key}'");
			else
				return new QuerySqlModel(QuerySqlModel.QueryType.Cte, block.Key, sql);
	}

	/// <summary>
	///		Obtiene la cadena SQL adicional para los filtros
	/// </summary>
	private string GetSqlForFilters(List<ClauseFilterModel> filters)
	{
		string sql = string.Empty;

			// Añade los filtros
			foreach (ClauseFilterModel filter in filters)
				sql = sql.AddWithSeparator(filter.Sql + Environment.NewLine, " AND ");
			// Añade la cláusula WHERE si es necesario
			if (!string.IsNullOrWhiteSpace(sql))
				sql = Environment.NewLine + " WHERE " + sql;
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Obtiene la cadena SQL para los campos solicitados de las dimensiones
	/// </summary>
	private string GetSqlFieldsForDimensions(List<ParserDimensionModel> dimensions)
	{
		string sql = string.Empty;

			// Obtiene los campos
			foreach (ParserDimensionModel dimension in dimensions)
			{
				BaseDimensionModel? dimensionJoin = Request.GetDimensionIfRequest(dimension);

					if (dimensionJoin is not null)
					{
						List<(string table, string field)> fields = GetFieldsRequest(dimension, dimension.WithRequestedFields, dimension.WithPrimaryKeys);

							// Añade los campos solicitados a la SQL
							foreach ((string table, string field) in fields)
							{
								string sqlField = string.Empty;

									if (dimension.CheckIfNull)
										sqlField = $"IsNull({SqlTools.GetFieldName(table, field)}, {SqlTools.GetFieldName(dimension.AdditionalTable, field)}) AS {field}";
									else
										sqlField = SqlTools.GetFieldName(table, field);
									// Añade el campo a la cadena SQL
									sql = sql.AddWithSeparator(sqlField, ",");
							}
					}
			}
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Obtiene la cadena SQL necesaria para un GROUP BY
	/// </summary>
	private string GetSqlForGroupBy(ParserGroupBySectionModel section)
	{
		string sql = GetSqlFieldsForDimensions(section.Dimensions);

			// Añade la SQL adicional
			sql = sql.AddWithSeparator(section.Sql, ",");
			// Obtiene la cadena de salida
			if (!string.IsNullOrWhiteSpace(sql))
				sql = $" GROUP BY {sql}";
			// Devuelve la cadena con los campos
			return sql;
	}

	/// <summary>
	///		Obtiene la cadena SQL necesaria para un PARTITION BY
	/// </summary>
	private string GetSqlForPartitionBy(ParserPartitionBySectionModel section)
	{
		string sql = GetSqlFieldsForDimensions(section.Dimensions);

			// Añade los campos adicionales
			if (!string.IsNullOrWhiteSpace(section.Additional))
				sql = sql.AddWithSeparator(section.Additional, ",");
			// Añade la cláusula PARTITION BY si es necesario
			if (!string.IsNullOrWhiteSpace(sql))
				sql = $"PARTITION BY {sql}";
			// Añade la cláusula ORDER BY si es necesario
			if (!string.IsNullOrWhiteSpace(section.OrderBy))
				sql = $"{sql} ORDER BY {section.OrderBy}";
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Obtiene la lista con los campos asociados a una consulta
	/// </summary>
	private List<(string table, string field)> GetListFields(QueryModel query, string tableAlias, bool includePrimaryKey)
	{
		List<(string table, string field)> fields = [];

			// Añade los campos de la consulta
			foreach (QueryFieldModel field in query.Fields)
				if (includePrimaryKey || !field.IsPrimaryKey)
					fields.Add((tableAlias, field.Alias));
			// Añade los campos hijo
			foreach (QueryJoinModel child in query.Joins)
				fields.AddRange(GetListFields(child.Query, tableAlias, includePrimaryKey));
			// Devuelve los campos
			return fields;
	}

	/// <summary>
	///		Obtiene la cláusula ORDER BY
	/// </summary>
	private string GetSqlForOrderBy(ParserOrderBySectionModel section)
	{
		List<(string table, string field, int orderIndex, RequestColumnBaseModel.SortOrder sortOrder)> fieldsSort = [];
		string sql = string.Empty;

			// Obtiene los campos para ORDER BY
			foreach (ParserDimensionModel parserDimension in section.Dimensions)
			{
				List<(string tableDimension, string fieldDimension)> fields = GetFieldsRequest(parserDimension, parserDimension.WithRequestedFields, 
																							   parserDimension.WithPrimaryKeys);
				RequestDimensionModel? requestDimension = Request.GetRequestedDimension(parserDimension.DimensionKey);

					// Obtiene las columnas ordenables
					if (requestDimension is not null)
						foreach ((string table, string field) in fields)
						{
							BaseDimensionModel? dimension = Request.Report.DataWarehouse.Dimensions[parserDimension.DimensionKey];

								if (dimension is not null)
								{
									DataSourceColumnModel? column = dimension.GetColumn(field, true);

										if (column is not null)
										{
											RequestDimensionColumnModel? requestColumn = requestDimension.GetRequestColumn(column.Id);

												// Añade los datos de ordenación
												if (requestColumn is not null && requestColumn.OrderBy != RequestColumnBaseModel.SortOrder.Undefined)
													fieldsSort.Add((table, field, requestColumn.OrderIndex, requestColumn.OrderBy));
										}
								}
						}
			}
			// Ordena por el índice
			fieldsSort.Sort((first, second) => first.orderIndex.CompareTo(second.orderIndex));
			// Obtiene la cadena SQL
			foreach ((string table, string field, int orderIndex, RequestColumnBaseModel.SortOrder sortOrder) in fieldsSort)
				sql = sql.AddWithSeparator($"{SqlTools.GetFieldName(table, field)} {GetSorting(sortOrder)}", ",");
			// Si hay una cadena SQL adicional en la sección, se añade
			sql = sql.AddWithSeparator(section.AdditionalSql, ",");
			// Si es obligatorio y está vacío, ordena por el primer campo
			if (section.Required && string.IsNullOrWhiteSpace(sql))
				sql = "1";
			// Añade el ORDER BY
			if (!string.IsNullOrWhiteSpace(sql))
				sql = $"ORDER BY {sql}";
			// Devuelve la cadena SQL
			return sql;

		// Obtiene la cadena con el tipo de ordenación
		string GetSorting(RequestColumnBaseModel.SortOrder sortOrder)
		{
			if (sortOrder == RequestColumnBaseModel.SortOrder.Descending)
				return "DESC";
			else
				return "ASC";
		}
	}

	/// <summary>
	///		Obtiene los campos solicitados de una dimensión
	/// </summary>
	private List<(string tableDimension, string fieldDimension)> GetFieldsRequest(ParserDimensionModel parserDimension, bool includeRequestFields, bool includePrimaryKey)
	{
		List<(string tableDimension, string fieldDimension)> fields = [];
		BaseDimensionModel? dimension = Request.GetDimensionIfRequest(parserDimension);

			// Si se ha solicitado algo de esta dimensión, se obtienen los datos
			if (dimension is not null)
			{
				RequestDimensionModel? request = Request.GetRequestedDimension(dimension.Id);

					// Añade los campos solicitados a la SQL
					if (request is not null)
						foreach (string field in GetListFields(GetQueryFromRequest(request), includeRequestFields, includePrimaryKey))
							fields.Add((parserDimension.TableAlias, field));
			}
			// Devuelve la lista de campos
			return fields;
	}

	/// <summary>
	///		Obtiene los campos asociados a una consulta
	/// </summary>
	private List<string> GetListFields(QueryModel query, bool includeRequestFields, bool includePrimaryKey)
	{
		List<string> fields = [];

			// Añade los campos de la consulta
			foreach (QueryFieldModel field in query.Fields)
				if (!field.IsPrimaryKey || includePrimaryKey || includeRequestFields)
					fields.Add(field.Alias);
			// Añade los campos hijo
			foreach (QueryJoinModel child in query.Joins)
				fields.AddRange(GetListFields(child.Query, false, includePrimaryKey));
			// Devuelve los campos
			return fields;
	}

	/// <summary>
	///		Obtiene la SQL de consulta de una dimensión
	/// </summary>
	private string GetSqlDimension(BaseDimensionModel dimension, List<ClauseFieldModel> fields)
	{
		QueryModel query;
		RequestDimensionModel? request = Request.GetRequestedDimension(dimension.Id);

			// Obtiene la consulta de la solicitud o de la dimensión
			if (request is not null)
				query = GetQueryFromRequest(request);
			else
				query = GetQueryFromDimension(dimension);
			// Añade los campos adicionales
			foreach (ClauseFieldModel field in fields)
			{
				string table = dimension.GetTableAlias();

					// Añade el campo adicional si no estaba ya en la consulta
					if (!query.ExistsField(table, field.Alias))
						query.Fields.Add(new QueryFieldModel(query, true, dimension.GetTableAlias(), field.Field, field.Alias, 
															 RequestColumnBaseModel.SortOrder.Undefined,
															 RequestDataSourceColumnModel.AggregationType.NoAggregated, true));
			}
			// Devuelve la cadena SQL de esta dimensión
			return query.Build();
	}

	/// <summary>
	///		Obtiene la consulta para una dimensión del informe
	/// </summary>
	private QueryModel GetQueryFromDimension(BaseDimensionModel dimension)
	{
		QueryModel query = new(this, dimension.Id, dimension.Id);

			aquí está el error
			// Prepara la consulta
			query.Prepare(dimension);
			// Añade sólo los campos clave
			foreach (DataSourceColumnModel column in dimension.GetColumns().EnumerateValues())
				if (column.IsPrimaryKey)
					query.AddPrimaryKey(null, column.Id, column.Alias, true);
			// Devuelve la consulta
			return query;
	}

	/// <summary>
	///		Obtiene la consulta para una solicitud de una dimensión del informe
	/// </summary>
	private QueryModel GetQueryFromRequest(RequestDimensionModel dimensionRequest)
	{
		List<QueryModel> childsQueries = [];
		BaseDimensionModel dimension = GetDimension(dimensionRequest);
		QueryModel query = GetChildQuery(dimensionRequest);

			// Obtiene las consultas de las dimensiones hija: si hay algún campo solicitado de alguna dimensión hija, 
			// necesitaremos también la consulta de esta dimensión para poder hacer el JOIN posterior, por eso las
			// calculamos antes que la consulta de esta dimensión
			foreach (RequestDimensionModel childDimension in Request.GetChildRequestedDimensions(dimensionRequest))
			{
				QueryModel childQuery = GetQueryFromRequest(childDimension);

					// Añade la query si realmente hay algo que añadir
					if (childQuery != null)
						childsQueries.Add(childQuery);
			}
			// Añade las consultas con las dimensiones hija
			if (childsQueries.Count > 0)
				foreach (QueryModel childQuery in childsQueries)
				{
					QueryJoinModel join = new(QueryJoinModel.JoinType.Inner, childQuery, $"child_{childQuery.Alias}");

						// Asigna las relaciones
						foreach (DimensionRelationModel relation in dimension.GetRelations())
							if (relation.Dimension is not null && relation.Dimension.Id.Equals(childQuery.SourceId, StringComparison.CurrentCultureIgnoreCase))
								foreach (RelationForeignKey foreignKey in relation.ForeignKeys)
									join.Relations.Add(new QueryRelationModel(foreignKey.ColumnId, childQuery.FromAlias, foreignKey.TargetColumnId));
						// Añade la unión
						query.Joins.Add(join);
				}
			// Devuelve la consulta
			return query;
	}

	/// <summary>
	///		Obtiene la consulta de una dimensión
	/// </summary>
	private QueryModel GetChildQuery(RequestDimensionModel dimensionRequest)
	{
		BaseDimensionModel dimension = GetDimension(dimensionRequest);
		QueryModel query = new(this, dimensionRequest.Dimension.Id, dimension.Id);
		BaseReportingDictionaryModel<DataSourceColumnModel> dimensionColumns = dimension.GetColumns();

			// Prepara la consulta
			query.Prepare(dimension);
			// Añade los campos clave
			foreach (DataSourceColumnModel column in dimensionColumns.EnumerateValues())
				if (column.IsPrimaryKey)
					query.AddPrimaryKey(dimensionRequest.GetRequestColumn(column.Id), column.Id, column.Alias, 
										CheckIsColumnAtColumnRequested(column, dimensionRequest.Columns));
			// Asigna los campos
			foreach (RequestDimensionColumnModel columnRequest in dimensionRequest.Columns)
			{
				DataSourceColumnModel? column = columnRequest.Column;

					if (column is not null && !column.IsPrimaryKey)
						query.AddColumn(column.Id, column.Alias, columnRequest);
			}
			// Devuelve la consulta
			return query;
	}

	/// <summary>
	///		Comprueba si la columna está entre las columnas solicitadas
	/// </summary>
	private bool CheckIsColumnAtColumnRequested(DataSourceColumnModel column, List<RequestDimensionColumnModel> columnRequests)
	{
		// Busca la columna entre las columnas solicitadas
		foreach (RequestDimensionColumnModel columnRequest in columnRequests)
			if (column.Id.Equals(columnRequest.Column.Id, StringComparison.CurrentCultureIgnoreCase))
				return true;
		// Si llega hasta aquí es porque no lo ha encontrado
		return false;
	}

	/// <summary>
	///		Crea la sentencia SQL asociada a un bloque que comprueba si se ha solicitado una (o varias) dimensiones
	/// </summary>
	private List<QuerySqlModel> GetQueries(BlockIfRequest block)
	{
		bool mustExecute;

			// Comprueba si se debe ejecutar
			if (block.DimensionKeys.Count > 0)
				mustExecute = Request.IsDimensionRequested(block.DimensionKeys);
			else // ... si no hay dimensiones, se pone a true para que se comprueben las expresiones
				mustExecute = true;
			if (block.ExpressionKeys.Count > 0)
				mustExecute &= Request.IsExpressionRequested(block.ExpressionKeys);
			// Obtiene las consultas
			if (mustExecute)
				return GetQueries(block.Then);
			else
				return GetQueries(block.Else);
	}

	/// <summary>
	///		Obtiene la dimensión
	/// </summary>
	private BaseDimensionModel GetDimension(RequestDimensionModel dimensionRequest) => dimensionRequest.Dimension;
	
	/// <summary>
	///		Solicitud
	/// </summary>
	internal RequestModel Request { get; }

	/// <summary>
	///		Herramientas para generación de SQL
	/// </summary>
	internal Tools.SqlTools SqlTools { get; } = new();
}