﻿using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Request.Models;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

/// <summary>
///		Clase con los datos de una consulta para una dimensión
/// </summary>
internal class QueryDimensionModel
{
	internal QueryDimensionModel(ReportQueryGenerator generator, BaseDimensionModel dimension, List<ClauseFieldModel>? fields, List<ClauseFilterModel>? filters)
	{
		Generator = generator;
		Dimension = dimension;
		Prepare(fields);
		Filters = filters;
	}

	/// <summary>
	///		Prepara la consulta con los datos de la solicitud
	/// </summary>
	private void Prepare(List<ClauseFieldModel>? fields)
	{
		RequestDimensionModel? request = Generator.Request.Dimensions.GetRequested(Dimension.Id);

			// Obtiene la consulta de la solicitud o de la dimensión
			if (request is not null)
				PrepareDimensionQuery(Dimension, Generator.Request);
			else
				PrepareFromDimension(Dimension);
			// Añade los campos adicionales
			if (fields is not null)
				foreach (ClauseFieldModel field in fields)
				{
					string table = Dimension.GetTableAlias();

						// Añade el campo adicional si no estaba ya en la consulta
						if (!ExistsField(table, field.Alias))
							Fields.Add(new QueryFieldModel(this, true, Dimension.GetTableAlias(), field.Field, field.Alias, 
														   RequestColumnBaseModel.SortOrder.Undefined, 0,
														   RequestDataSourceColumnModel.AggregationType.NoAggregated, true));
				}
	}

	/// <summary>
	///		Prepara la consulta para una dimensión del informe (añade sólo los campos clave de la dimensión)
	/// </summary>
	private void PrepareFromDimension(BaseDimensionModel dimension)
	{
		foreach (DataSourceColumnModel column in dimension.GetColumns().EnumerateValues())
			if (column.IsPrimaryKey)
				AddPrimaryKey(null, column.Id, column.Alias, true);
	}

	/// <summary>
	///		Prepara la consulta de una dimensión a partir de los datos de la solicitud
	/// </summary>
	private void PrepareDimensionQuery(BaseDimensionModel dimension, RequestModel request)
	{
		// Busca las solicitudes de esta dimensión
		foreach (RequestDimensionModel requestDimension in request.Dimensions)
			if (requestDimension.Dimension.Id.Equals(dimension.Id, StringComparison.CurrentCultureIgnoreCase))
				foreach (RequestDimensionColumnModel columnRequest in requestDimension.Columns)
				{
					DataSourceColumnModel? column = columnRequest.Column;

						if (column.IsPrimaryKey)
							AddPrimaryKey(columnRequest, columnRequest.Column.Id, columnRequest.Column.Alias, columnRequest.Column.Visible);
						else
							AddColumn(column.Id, column.Alias, columnRequest);
				}
		// Añade los campos clave
		foreach (DataSourceColumnModel column in dimension.GetColumns().EnumerateValues())
			if (column.IsPrimaryKey && !ExistsField(column.DataSource.Id, column.Alias))
				AddPrimaryKey(null, column.Id, column.Alias, false);
		// Añade las dimensiones hija
		foreach (DimensionRelationModel relation in dimension.GetRelations())
			if (relation.Dimension is null)
				throw new Exceptions.ReportingParserException($"Can't find the dimension {relation.DimensionId}");
			else
			{
				QueryDimensionModel childQuery = new(Generator, relation.Dimension, null, null);

					// Añade la consulta hija si tiene alguna subconsulta o si alguno de sus campos solicitados no es una clave primaria
					if (childQuery.Joins.Count > 0 || childQuery.HasFieldsNoPrimaryKey())
					{
						QueryJoinModel join = new(QueryJoinModel.JoinType.Inner, childQuery, $"child_{childQuery.Alias}");

							// Asigna las relaciones
							foreach (RelationForeignKey foreignKey in relation.ForeignKeys)
								join.Relations.Add(new QueryRelationModel(foreignKey.ColumnId, childQuery.FromAlias, foreignKey.TargetColumnId));
							// Añade la unión
							Joins.Add(join);
					}
			}
	}

	/// <summary>
	///		Añade un campo de clave primaria a la consulta
	/// </summary>
	internal void AddPrimaryKey(RequestColumnBaseModel? requestColumn, string columnId, string columnAlias, bool visible)
	{
		QueryFieldModel field = new(this, true, FromAlias, columnId, columnAlias, RequestColumnBaseModel.SortOrder.Undefined, 0,
									RequestDataSourceColumnModel.AggregationType.NoAggregated, visible);

			// Añade los filtros
			if (requestColumn is not null)
				field.FiltersWhere.AddRange(GetFilters(requestColumn.FiltersWhere));
			// Añade el campo a la colección de campos de la consulta
			Fields.Add(field);
	}

	/// <summary>
	///		Añade un campo a la consulta
	/// </summary>
	internal void AddColumn(string columnId, string columnAlias, RequestColumnBaseModel requestColumn)
	{
		AddColumn(columnId, columnAlias, RequestDataSourceColumnModel.AggregationType.NoAggregated, requestColumn);
	}

	/// <summary>
	///		Añade un campo a la consulta
	/// </summary>
	internal void AddColumn(string columnId, string columnAlias, RequestDataSourceColumnModel.AggregationType aggregatedBy, 
							RequestColumnBaseModel requestColumn)
	{
		QueryFieldModel field = GetQueryField(columnId, columnAlias, aggregatedBy, requestColumn);

			// Añade los filtros
			field.FiltersWhere.AddRange(GetFilters(requestColumn.FiltersWhere));
			field.FilterHaving.AddRange(GetFilters(requestColumn.FiltersHaving));
			// Añade la columna a la consulta
			Fields.Add(field);
	}

	/// <summary>
	///		Obtiene el campo de la consulta
	/// </summary>
	private QueryFieldModel GetQueryField(string columnId, string columnAlias, RequestDataSourceColumnModel.AggregationType aggregatedBy, 
										  RequestColumnBaseModel requestColumn)
	{
		QueryFieldModel? field = Fields.FirstOrDefault(item => item.Field.Equals(columnId, StringComparison.CurrentCultureIgnoreCase));

			// Si no existía, lo añade
			if (field is null)
				field = new QueryFieldModel(this, false, FromAlias, columnId, columnAlias, requestColumn.OrderBy, 
											requestColumn.OrderIndex, aggregatedBy, requestColumn.Visible);
			// Devuelve el campo
			return field;
	}

	/// <summary>
	///		Convierte los filtros
	/// </summary>
	private List<QueryFilterModel> GetFilters(List<RequestFilterModel> filters)
	{
		List<QueryFilterModel> converted = [];

			// Convierte los filtros
			foreach (RequestFilterModel filter in filters)
				converted.Add(new QueryFilterModel(this, filter.Condition, filter.Values));
			// Devuelve los filtros convertidos
			return converted;
	}

	/// <summary>
	///		Comprueba si existe el campo
	/// </summary>
	internal bool ExistsField(string dataSourceId, string alias)
	{
		// Comprueba si existe el campo
		foreach (QueryFieldModel queryField in Fields)
			if (queryField.Table.Equals(dataSourceId, StringComparison.CurrentCultureIgnoreCase) &&
					queryField.Alias.Equals(alias, StringComparison.CurrentCultureIgnoreCase))
				return true;
		// Si ha llegado hasta aquí es porque el campo no existe
		return false;
	}

	/// <summary>
	///		Genera la consulta SQL de esta clase
	/// </summary>
	internal string Build()
	{
		Prettifier.StringPrettifier prettifier = new();

			// Añade la cláusula SELECT con los campos (los de ésta y los de mis JOIN hija)
			prettifier.Append("SELECT " + GetSqlFields(), 100, ",");
			// Añade la cláusula FROM
			prettifier.NewLine();
			prettifier.Indent();
			prettifier.Append($"FROM {FromTable} AS {Generator.SqlTools.GetFieldName(FromAlias)}");
			// Añade los JOIN
			if (Joins.Count > 0)
			{
				prettifier.NewLine();
				prettifier.Append(GetSqlJoins());
			}
			prettifier.Unindent();
			prettifier.NewLine();
			// Añade los WHERE
			prettifier.Indent();
			prettifier.Append(GetSqlWhere());
			prettifier.Unindent();
			prettifier.NewLine();
			// Añade los GROUP BY
			prettifier.Indent();
			prettifier.Append(GetSqlGroupBy(), 100, ",");
			prettifier.Unindent();
			prettifier.NewLine();
			// Añade los HAVING
			prettifier.Indent();
			prettifier.Append(GetSqlHaving());
			prettifier.Unindent();
			prettifier.NewLine();
			// Devuelve la consulta SQL
			return prettifier.ToString();
	}

	/// <summary>
	///		Obtiene los campos de la consulta (los campos clave de esta tabla, los campos de salida de esta tabla y 
	///	los campos con las tabla con las que se haga JOIN por debajo)
	/// </summary>
	private string GetSqlFields()
	{
		string sql = string.Empty;

			// Añade las claves
			foreach (QueryFieldModel field in Fields)
				if (field.IsPrimaryKey)
					sql = sql.AddWithSeparator(GetSqlField(field), ",");
			// Añade los campos
			foreach (QueryFieldModel field in Fields)
				if (!field.IsPrimaryKey && field.Visible && field.Aggregation == RequestDataSourceColumnModel.AggregationType.NoAggregated)
					sql = sql.AddWithSeparator(GetSqlField(field), ",");
			// Añade los campos (no clave) de los JOIN hijo (dimensiones hija) que no estén agregados
			sql = sql.AddWithSeparator(GetSqlFields(Joins), ",");
			// Añade los campos agrupados
			foreach (QueryFieldModel field in Fields)
				if (field.Aggregation != RequestDataSourceColumnModel.AggregationType.NoAggregated)
					sql = sql.AddWithSeparator($"{field.GetAggregation(FromAlias)} AS {Generator.SqlTools.GetFieldName(field.Alias)}", ",");
			// Devuelve los campos
			return sql;
	}

	/// <summary>
	///		Obtiene la cadena SQL de los campos de tablas relacionadas
	/// </summary>
	private string GetSqlFields(List<QueryJoinModel> joins)
	{
		string sql = string.Empty;

			// Añade los campos
			foreach (QueryJoinModel join in joins)
			{
				// Añade los campos de la consulta relacionada
				foreach (QueryFieldModel field in join.Query.Fields)
					if (!field.IsPrimaryKey && field.Visible && field.Aggregation == RequestDataSourceColumnModel.AggregationType.NoAggregated)
						sql = sql.AddWithSeparator(GetSqlField(field), ",");
				// Añade los campos de las relaciones hija
				sql = sql.AddWithSeparator(GetSqlFields(join.Query.Joins), ",");
			}
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Obtiene el nombre de un campo con el AS
	/// </summary>
	private string GetSqlField(QueryFieldModel field)
	{
		string fieldName = Generator.SqlTools.GetFieldName(field.Table, field.Field);

			// Añade el alias
			if (!string.IsNullOrWhiteSpace(field.Alias))
				fieldName += $" AS {Generator.SqlTools.GetFieldName(field.Alias)}";
			// Devuelve el nombre del campo
			return fieldName;
	}

	/// <summary>
	///		Obtiene la cadena SQL de la condición WHERE
	/// </summary>
	private string GetSqlWhere()
	{
		string sql = GetSqlFilters(true) + Environment.NewLine + GetSqlForFilters(Filters);

			// Añade la cláusula WHERE si tiene algún filtro
			if (!string.IsNullOrWhiteSpace(sql))
				sql = $" WHERE {sql}";
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Obtiene la cadena SQL adicional para los filtros
	/// </summary>
	private string GetSqlForFilters(List<ClauseFilterModel>? filters)
	{
		string sql = string.Empty;

			// Añade los filtros
			if (filters is not null)
				foreach (ClauseFilterModel filter in filters)
					sql = sql.AddWithSeparator(filter.Sql + Environment.NewLine, " AND ");
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Obtiene la cadena SQL de la condición WHERE
	/// </summary>
	private string GetSqlHaving()
	{
		string sql = GetSqlFilters(false);

			// Añade la cláusula HAVING si es necesario
			if (!string.IsNullOrWhiteSpace(sql))
				sql = $" HAVING {sql}";
			// Devuelve el filtro
			return sql;
	}

	/// <summary>
	///		Obtiene la cadena SQL de los filtros y de sus hijos
	/// </summary>
	private string GetSqlFilters(bool where)
	{
		string sql = string.Empty;

			// Añade los filtros de esta consulta
			foreach (QueryFieldModel field in Fields)
				foreach (QueryFilterModel filter in where ? field.FiltersWhere : field.FilterHaving)
					if (where)
						sql = sql.AddWithSeparator(filter.GetSql(FromAlias, field.Field), Environment.NewLine + "AND");
					else // ... si estamos en un having, la condición es por el agregado
						sql = sql.AddWithSeparator(filter.GetSql(field.GetAggregation(FromAlias)), Environment.NewLine + "AND");
			// Añade los filtros de las consulta con JOIN
			foreach (QueryJoinModel join in Joins)
			{
				string sqlFilter = join.Query.GetSqlFilters(where);

					if (!string.IsNullOrWhiteSpace(sqlFilter))
						sql = sql.AddWithSeparator(sqlFilter, Environment.NewLine + "AND");
			}
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Obtiene la SQL de consulta de los JOIN
	/// </summary>
	private string GetSqlJoins()
	{
		string sql = string.Empty;

			// Añade las consultas para los JOIN
			foreach (QueryJoinModel join in Joins)
			{
				bool needAnd = false;

					// Añade el tipo de JOIN
					sql = sql.AddWithSeparator(GetJoin(join.Type), Environment.NewLine);
					// Añade el nombre de tabla y el alias
					sql += $" {join.Query.FromTable} AS {Generator.SqlTools.GetFieldName(join.Query.FromAlias)}" + Environment.NewLine;
					// Añade las relaciones
					sql += " ON ";
					foreach (QueryRelationModel relation in join.Relations)
					{
						// Añade el operador AND si es necesario
						if (needAnd)
							sql += " AND ";
						else
							needAnd = true;
						// Añade la condición entre campos
						sql += Generator.SqlTools.GetFieldName(FromAlias, relation.Column) + " = " + Generator.SqlTools.GetFieldName(relation.RelatedTable, relation.RelatedColumn) + Environment.NewLine;
					}
			}
			// Devuelve la consulta
			return sql;
	}

	/// <summary>
	///		Obtiene la cláusula adecuada para el JOIN
	/// </summary>
	private string GetJoin(QueryJoinModel.JoinType type)
	{
		return type switch
				{
					QueryJoinModel.JoinType.Left => "LEFT JOIN",
					QueryJoinModel.JoinType.Right => "RIGHT JOIN",
					QueryJoinModel.JoinType.Full => "FULL OUTER JOIN",
					QueryJoinModel.JoinType.Cross => "CROSS JOIN",
					_ => "INNER JOIN"
				};
	}

	/// <summary>
	///		Obtiene la cadena de agrupación
	/// </summary>
	private string GetSqlGroupBy()
	{
		string sqlFields = string.Empty;

			// Sólo tiene GROUP BY cuando hay algún campo agregado
			if (NeedGroupBy())
			{
				// Añade los campos de agrupación
				foreach (QueryFieldModel field in Fields)
					if (field.Aggregation == RequestDataSourceColumnModel.AggregationType.NoAggregated && field.Visible)
						sqlFields = sqlFields.AddWithSeparator(Generator.SqlTools.GetFieldName(field.Table, field.Field), ",");
				// Si hay algún campo de agrupación, le añade la cláusula
				if (!string.IsNullOrWhiteSpace(sqlFields))
					sqlFields = $"GROUP BY {sqlFields}" + Environment.NewLine;
			}
			// Devuelve la cadena de agrupación
			return sqlFields;
	}

	/// <summary>
	///		Comprueba si necesita una cláusula GROUP BY: si hay algún agregado
	/// </summary>
	private bool NeedGroupBy()
	{
		// Recorre los campos buscando si hay algún agregado
		foreach (QueryFieldModel field in Fields)
			if (field.Aggregation != RequestDataSourceColumnModel.AggregationType.NoAggregated)
				return true;
		// Si ha llegado hasta aquí es porque no hay ningún campo agregado
		return false;
	}

	/// <summary>
	///		Obtiene la cadena de ordenación
	/// </summary>
	internal string GetOrderByFields(string? tableAliasAtWith = null)
	{
		string sql = string.Empty;

			// Normaliza el nombre del alias de la tabla que
			if (string.IsNullOrWhiteSpace(tableAliasAtWith))
				tableAliasAtWith = Alias;
			// Añade los campos a ordenar
			foreach (QueryFieldModel field in Fields)
				if (field.Visible && field.OrderBy != RequestColumnBaseModel.SortOrder.Undefined)
					sql = sql.AddWithSeparator(Generator.SqlTools.GetFieldName(tableAliasAtWith, field.Alias) + " " + GetSortClause(field.OrderBy), ",");
			// Añade los campos a ordenar de las consultas hijo
			foreach (QueryJoinModel join in Joins)
				sql = sql.AddWithSeparator(join.Query.GetOrderByFields(tableAliasAtWith), ",");
			// Devuelve la cadena
			return sql;
	}

	/// <summary>
	///		Obtiene la cláusula de ordenación
	/// </summary>
	private string GetSortClause(RequestColumnBaseModel.SortOrder orderBy)
	{
		if (orderBy == RequestColumnBaseModel.SortOrder.Ascending)
			return "ASC";
		else
			return "DESC";
	}

	/// <summary>
	///		Comprueba si hay algún campo que no sea clave primaria
	/// </summary>
	internal bool HasFieldsNoPrimaryKey()
	{
		// Comprueba si alguno de los campos es clave primaria
		foreach (QueryFieldModel field in Fields)
			if (!field.IsPrimaryKey)
				return true;
		// Si ha llegado hasta aquí es porque no hay ninguna clave primaria
		return false;
	}

	/// <summary>
	///		Generador de informes
	/// </summary>
	internal ReportQueryGenerator Generator { get; }

	/// <summary>
	///		Dimensión
	/// </summary>
	internal BaseDimensionModel Dimension { get; }

	/// <summary>
	///		Tabla de la consulta
	/// </summary>
	internal string FromTable => Dimension.GetTableFullName();

	/// <summary>
	///		Alias de la tabla
	/// </summary>
	internal string FromAlias => Dimension.GetTableAlias();

	/// <summary>
	///		Alias de la consulta
	/// </summary>
	internal string Alias => Dimension.Id;

	/// <summary>
	///		Campos de la consulta
	/// </summary>
	internal QueryFieldsCollection Fields { get; } = [];

	/// <summary>
	///		Filtros de la consulta de dimensión
	/// </summary>
	internal List<ClauseFilterModel>? Filters { get; }

	/// <summary>
	///		Subconsultas combinadas
	/// </summary>
	internal QueryJoinsCollection Joins { get; } = [];

	/// <summary>
	///		Claves foráneas de esta consulta
	/// </summary>
	internal QueryForeignKeyCollection ForeignKeys { get; } = [];
}