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
							Fields.Add(new QueryDimensionFieldModel(this, true, Dimension.GetTableAlias(), field.Field, field.Alias, true));
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
				foreach (RequestColumnModel columnRequest in requestDimension.Columns)
				{
					DataSourceColumnModel? column = dimension.GetColumn(columnRequest.Id, false);

						if (column is null)
							throw new Exceptions.ReportingParserException($"Can't find the column {columnRequest.Id} at dimension {requestDimension.Dimension.Id}");
						else if (column.IsPrimaryKey)
							AddPrimaryKey(columnRequest, column.Id, column.Alias, column.Visible);
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
						QueryDimensionJoinModel join = new(QueryDimensionJoinModel.JoinType.Inner, childQuery, $"child_{childQuery.Alias}");

							// Asigna las relaciones
							foreach (RelationForeignKey foreignKey in relation.ForeignKeys)
								join.Relations.Add(new QueryDimensionRelationModel(foreignKey.ColumnId, childQuery.FromAlias, foreignKey.TargetColumnId));
							// Añade la unión
							Joins.Add(join);
					}
			}
	}

	/// <summary>
	///		Añade un campo de clave primaria a la consulta
	/// </summary>
	internal void AddPrimaryKey(RequestColumnModel? requestColumn, string columnId, string columnAlias, bool visible)
	{
		QueryDimensionFieldModel field = new(this, true, FromAlias, columnId, columnAlias, visible);

			// Añade los filtros
			if (requestColumn is not null)
				field.FiltersWhere.AddRange(GetFilters(requestColumn.FiltersWhere));
			// Añade el campo a la colección de campos de la consulta
			Fields.Add(field);
	}

	/// <summary>
	///		Añade un campo a la consulta
	/// </summary>
	private void AddColumn(string columnId, string columnAlias, RequestColumnModel requestColumn)
	{
		QueryDimensionFieldModel? field = Fields.FirstOrDefault(item => item.Field.Equals(columnId, StringComparison.CurrentCultureIgnoreCase));
		
			// Si no existía, lo añade
			if (field is null)
			{
				// Crea el campo
				field = new QueryDimensionFieldModel(this, false, FromAlias, columnId, columnAlias, requestColumn.Visible);
				// y lo añade a la columna
				Fields.Add(field);
			}
			// Añade los filtros
			field.FiltersWhere.AddRange(GetFilters(requestColumn.FiltersWhere));
			field.FilterHaving.AddRange(GetFilters(requestColumn.FiltersHaving));
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
		foreach (QueryDimensionFieldModel queryField in Fields)
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
		Tools.SqlStringBuilder prettifier = new();

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
				prettifier.Append(GetSqlJoins(FromAlias, Joins));
			}
			prettifier.Unindent();
			prettifier.NewLine();
			// Añade los WHERE
			prettifier.Indent();
			prettifier.Append(GetSqlWhere());
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
			foreach (QueryDimensionFieldModel field in Fields)
				if (field.IsPrimaryKey)
					sql = sql.AddWithSeparator(GetSqlField(field), ",");
			// Añade los campos
			foreach (QueryDimensionFieldModel field in Fields)
				if (!field.IsPrimaryKey && field.Visible && field.Aggregation == RequestColumnModel.AggregationType.NoAggregated)
					sql = sql.AddWithSeparator(GetSqlField(field), ",");
			// Añade los campos (no clave) de los JOIN hijo (dimensiones hija) que no estén agregados
			sql = sql.AddWithSeparator(GetSqlFields(Joins), ",");
			// Devuelve los campos
			return sql;
	}

	/// <summary>
	///		Obtiene la cadena SQL de los campos de tablas relacionadas
	/// </summary>
	private string GetSqlFields(List<QueryDimensionJoinModel> joins)
	{
		string sql = string.Empty;

			// Añade los campos
			foreach (QueryDimensionJoinModel join in joins)
			{
				// Añade los campos de la consulta relacionada
				foreach (QueryDimensionFieldModel field in join.Query.Fields)
					if (!field.IsPrimaryKey && field.Visible && field.Aggregation == RequestColumnModel.AggregationType.NoAggregated)
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
	private string GetSqlField(QueryDimensionFieldModel field)
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
	///		Obtiene la cadena SQL de los filtros y de sus hijos
	/// </summary>
	private string GetSqlFilters(bool where)
	{
		string sql = string.Empty;

			// Añade los filtros de esta consulta
			foreach (QueryDimensionFieldModel field in Fields)
				foreach (QueryFilterModel filter in where ? field.FiltersWhere : field.FilterHaving)
					if (where)
						sql = sql.AddWithSeparator(filter.GetSql(FromAlias, field.Field), Environment.NewLine + "AND");
			// Añade los filtros de las consulta con JOIN
			foreach (QueryDimensionJoinModel join in Joins)
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
	private string GetSqlJoins(string parentTable, List<QueryDimensionJoinModel> joins)
	{
		string sql = string.Empty;

			// Añade las consultas para los JOIN
			foreach (QueryDimensionJoinModel join in joins)
			{
				bool needAnd = false;

					// Añade el tipo de JOIN
					sql = sql.AddWithSeparator(GetJoin(join.Type), Environment.NewLine);
					// Añade el nombre de tabla y el alias
					sql += $" {join.Query.FromTable} AS {Generator.SqlTools.GetFieldName(join.Query.FromAlias)}" + Environment.NewLine;
					// Añade las relaciones
					sql += " ON ";
					foreach (QueryDimensionRelationModel relation in join.Relations)
					{
						// Añade el operador AND si es necesario
						if (needAnd)
							sql += " AND ";
						else
							needAnd = true;
						// Añade la condición entre campos
						sql += Generator.SqlTools.GetFieldName(parentTable, relation.Column) + " = " + 
									Generator.SqlTools.GetFieldName(relation.RelatedTable, relation.RelatedColumn) + Environment.NewLine;
					}
					// Añade las consultas para los join hijos
					sql = sql.AddWithSeparator(GetSqlJoins(join.Query.FromAlias, join.Query.Joins), Environment.NewLine);
			}
			// Devuelve la consulta
			return sql;
	}

	/// <summary>
	///		Obtiene la cláusula adecuada para el JOIN
	/// </summary>
	private string GetJoin(QueryDimensionJoinModel.JoinType type)
	{
		return type switch
				{
					QueryDimensionJoinModel.JoinType.Left => "LEFT JOIN",
					QueryDimensionJoinModel.JoinType.Right => "RIGHT JOIN",
					QueryDimensionJoinModel.JoinType.Full => "FULL OUTER JOIN",
					QueryDimensionJoinModel.JoinType.Cross => "CROSS JOIN",
					_ => "INNER JOIN"
				};
	}

	/// <summary>
	///		Comprueba si hay algún campo que no sea clave primaria
	/// </summary>
	internal bool HasFieldsNoPrimaryKey()
	{
		// Comprueba si alguno de los campos es clave primaria
		foreach (QueryDimensionFieldModel field in Fields)
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
	internal List<QueryDimensionFieldModel> Fields { get; } = [];

	/// <summary>
	///		Filtros de la consulta de dimensión
	/// </summary>
	internal List<ClauseFilterModel>? Filters { get; }

	/// <summary>
	///		Subconsultas combinadas
	/// </summary>
	internal QueryDimensionJoinCollectionModel Joins { get; } = [];
}