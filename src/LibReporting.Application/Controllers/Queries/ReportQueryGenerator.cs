﻿using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Request.Models;
using Bau.Libraries.LibReporting.Application.Exceptions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries;

/// <summary>
///		Generador de consultas SQL para un informe
/// </summary>
internal class ReportQueryGenerator
{
	// Variables privadas
	private List<QueryDimensionModel> _queryDimensions = [];

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
					request.Dimensions.Add(fixedRequest.DimensionKey, field.Field, false);

		// Comprueba si se ha solicitado alguno de los campos considerados como obligatorios
		bool CheckIsRequestedAnyField(RequestModel reportRequest, ReportRequestDimension fixedRequest)
		{
			RequestDimensionModel? dimensionRequest = reportRequest.Dimensions.GetRequested(fixedRequest.DimensionKey);

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
				QueryDimensionModel queryDimension = new(this, dimension, block.Fields);
				string sql = queryDimension.Build();
				
					// Añade la consulta de dimensión a la lista interna
					_queryDimensions.Add(queryDimension);
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
				BaseDimensionModel? dimensionJoin = Request.Dimensions.GetIfRequest(dimension);

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
	private List<(string table, string field)> GetListFields(QueryDimensionModel query, string tableAlias, bool includePrimaryKey)
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
				RequestDimensionModel? requestDimension = Request.Dimensions.GetRequested(parserDimension.DimensionKey);

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
		BaseDimensionModel? dimension = Request.Dimensions.GetIfRequest(parserDimension);

			// Si se ha solicitado algo de esta dimensión, se obtienen los datos
			if (dimension is not null)
			{
				RequestDimensionModel? request = Request.Dimensions.GetRequested(dimension.Id);

					// Añade los campos solicitados a la SQL
					if (request is not null)
					{
						QueryDimensionModel? queryDimension = GetQueryFromRequest(request);

							if (queryDimension is null)
								throw new ReportingParserException($"Can't find the dimension query for dimension {request.Dimension.Id}");
							else
								foreach (string field in GetListFields(queryDimension, includeRequestFields, includePrimaryKey))
									fields.Add((parserDimension.TableAlias, field));
					}
			}
			// Devuelve la lista de campos
			return fields;
	}

	/// <summary>
	///		Obtiene los campos asociados a una consulta
	/// </summary>
	private List<string> GetListFields(QueryDimensionModel query, bool includeRequestFields, bool includePrimaryKey)
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
	///		Obtiene la consulta para una solicitud de una dimensión del informe
	/// </summary>
	private QueryDimensionModel? GetQueryFromRequest(RequestDimensionModel dimensionRequest)
	{
		// Obtiene la consulta asociada a la dimensión
		foreach (QueryDimensionModel queryDimension in _queryDimensions)
			if (queryDimension.Dimension.Id.Equals(dimensionRequest.Dimension.Id))
				return queryDimension;
		// Si ha llegado hasta aquí es porque no ha encontrado nada
		return null;
	}

	/// <summary>
	///		Crea la sentencia SQL asociada a un bloque que comprueba si se ha solicitado una (o varias) dimensiones
	/// </summary>
	private List<QuerySqlModel> GetQueries(BlockIfRequest block)
	{
		bool mustExecute;

			// Comprueba si se debe ejecutar
			if (block.DimensionKeys.Count > 0)
				mustExecute = Request.Dimensions.IsRequested(block.DimensionKeys);
			else // ... si no hay dimensiones, se pone a true para que se comprueben las expresiones
				mustExecute = true;
			if (block.ExpressionKeys.Count > 0)
				mustExecute &= Request.Expressions.IsRequested(block.ExpressionKeys);
			// Obtiene las consultas
			if (mustExecute)
				return GetQueries(block.Then);
			else
				return GetQueries(block.Else);
	}
	
	/// <summary>
	///		Solicitud
	/// </summary>
	internal RequestModel Request { get; }

	/// <summary>
	///		Herramientas para generación de SQL
	/// </summary>
	internal Tools.SqlTools SqlTools { get; } = new();
}