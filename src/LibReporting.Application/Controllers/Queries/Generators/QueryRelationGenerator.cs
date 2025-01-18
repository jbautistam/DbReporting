using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Generators;

/// <summary>
///		Clase para generar la SQL de <see cref="ParserJoinSectionModel"/>
/// </summary>
internal class QueryRelationGenerator : QueryBaseGenerator
{
	// Registros privadas
	private record JoinField(string FieldSource, string TableTarget, string FieldTarget);

	internal QueryRelationGenerator(ReportQueryGenerator manager, ParserJoinSectionModel section, QueryDimensionsCollection queryDimensions) : base(manager)
	{
		Section = section;
		QueryDimensions = queryDimensions;
	}

	/// <summary>
	///		Obtiene la SQL
	/// </summary>
	internal override string GetSql()
	{
		parece que está bien pero esto es lo que hay que seguir probando
		if (Section.JoinDimensions.Count == 0)
			throw new Exceptions.ReportingParserException($"Can't find relations at join {Section.Join.ToString()} with table {Section.Table}");
		else
		{
			string sql = GetSql(Section);

				// Añade la SQL adicional o bien la SQL definida para cuando no se encuentran datos de la dimensión
				if (!string.IsNullOrWhiteSpace(sql))
				{
					if (!string.IsNullOrWhiteSpace(Section.Sql))
						sql = sql.AddWithSeparator(Section.Sql, " AND ");
				}
				else if (!string.IsNullOrWhiteSpace(Section.SqlNoDimension))
					sql = $" {GetJoinClause(Section.Join)} {Section.SqlNoDimension}";
				// Devuelve la cadena SQL generada
				return sql;
		}
	}

	/// <summary>
	///		Obtiene la cadena SQL de la dimensión
	/// </summary>
	private string GetSql(ParserJoinSectionModel section)
	{
		string sql = string.Empty;

			// Obtiene las cadenas de unión con las dimensiones
			foreach (ParserJoinDimensionSectionModel joinDimension in section.JoinDimensions)
			{
				QueryDimensionModel? queryDimension = QueryDimensions.Get(joinDimension.DimensionKey);

					// Crea el join con los campos a la consulta
					if (queryDimension is not null)
						sql = sql.AddWithSeparator(GetSqlJoin(section.Join, section.Table, GetJoinFields(joinDimension, queryDimension), 
															  joinDimension.CheckIfNull), 
												   Environment.NewLine);
			}
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Obtiene la cadena SQL del JOIN
	/// </summary>
	private string GetSqlJoin(ParserJoinSectionModel.JoinType join, string table, List<JoinField> joinFields, bool checkIfNull)
	{
		string sql = string.Empty;

			// Añade la SQL de los campos
			foreach (JoinField joinField in joinFields)
				sql = sql.AddWithSeparator(GetFieldCompare(table, joinField.FieldSource, joinField.TableTarget, joinField.FieldTarget, checkIfNull),
										   Environment.NewLine + "AND");
			// Si hay algún campo, obtiene la cadena final
			if (!string.IsNullOrWhiteSpace(sql))
				sql = $"{GetJoinClause(join)} {sql}";
			// Devuelve la cadena SQL creada
			return sql;

		// Obtiene una cadena de comparación de campos
		string GetFieldCompare(string tableSource, string fieldSource, string tableTarget, string fieldTarget, bool checkIfNull)
		{
			if (!checkIfNull)
				return $"{Manager.SqlTools.GetFieldName(tableSource, fieldSource)} = {Manager.SqlTools.GetFieldName(tableTarget, fieldTarget)}";
			else //TODO: debería comparar con el valor predeterminado del tipo del campo
				return $"IsNull({Manager.SqlTools.GetFieldName(tableSource, fieldSource)}, '') = IsNull({Manager.SqlTools.GetFieldName(tableTarget, fieldTarget)}, '')";
		}

	}

	/// <summary>
	///		Obtiene la cadena adecuada para un tipo de JOIN
	/// </summary>
	private string GetJoinClause(ParserJoinSectionModel.JoinType join)
	{
		return join switch
					{
						ParserJoinSectionModel.JoinType.InnerJoin => " INNER JOIN ",
						ParserJoinSectionModel.JoinType.CrossJoin => " CROSS JOIN ",
						ParserJoinSectionModel.JoinType.FullJoin => " FULL OUTER JOIN ",
						ParserJoinSectionModel.JoinType.LeftJoin => " LEFT JOIN ",
						ParserJoinSectionModel.JoinType.RightJoin => " RIGHT JOIN ",
						_ => throw new Exceptions.ReportingParserException($"Join type unknown: {join.ToString()}"),
					};
	}

	/// <summary>
	///		Obtiene los campos que se van a relacionar
	/// </summary>
	private List<JoinField> GetJoinFields(ParserJoinDimensionSectionModel joinDimension, QueryDimensionModel queryDimension)
	{
		string tableDimension = GetJoinDimensionTableName(joinDimension);

			// Obtiene los campos que se van a relacionar
			if (joinDimension.WithRequestedFields)
				return GetJoinFieldsRequested(tableDimension, queryDimension);
			else if (joinDimension.Fields.Count > 0)
				return GetJoinFieldsWithDimension(tableDimension, joinDimension.Fields);
			else
				return GetJoinFieldsPrimaryKey(tableDimension, queryDimension);

		// Obtiene el nombre de la tabla de la dimensión para el Join teniendo en cuenta si tiene o no alias
		string GetJoinDimensionTableName(ParserJoinDimensionSectionModel joinDimension)
		{
			if (!string.IsNullOrWhiteSpace(joinDimension.TableAlias) && 
					!joinDimension.TableAlias.Equals(joinDimension.TableAlias, StringComparison.CurrentCultureIgnoreCase))
				return $"{joinDimension.Table} AS {joinDimension.TableAlias}";
			else
				return joinDimension.Table;
		}

	}

	/// <summary>
	///		Obtiene la lista de campos solicitados para el JOIN
	/// </summary>
	private List<JoinField> GetJoinFieldsRequested(string tableDimension, QueryDimensionModel queryDimension)
	{
		List<JoinField> fields = [];

			// Obtiene los campos solicitados
			foreach (QueryFieldModel field in queryDimension.Fields)
				if (!field.IsPrimaryKey)
					fields.Add(new JoinField(field.Alias, tableDimension, field.Alias));
			// Devuelve la lista de campos
			return fields;
	}

	/// <summary>
	///		Obtiene la lista de campos solicitados para el JOIN para la clave primaria de la dimensión
	/// </summary>
	private List<JoinField> GetJoinFieldsPrimaryKey(string tableDimension, QueryDimensionModel queryDimension)
	{
		List<JoinField> fields = [];

			// Obtiene los campos solicitados
			foreach (QueryFieldModel field in queryDimension.Fields)
				if (field.IsPrimaryKey)
					fields.Add(new JoinField(field.Alias, tableDimension, field.Alias));
			// Devuelve la lista de campos
			return fields;
	}

	/// <summary>
	///		Obtiene las comparaciones entre campos
	/// </summary>
	private List<JoinField> GetJoinFieldsWithDimension(string tableDimension, List<(string fieldDimension, string fieldTable)> requested)
	{
		List<JoinField> fields = [];
		string sql = string.Empty;

			// Añade las comparaciones
			foreach ((string fieldDimension, string fieldTable) in requested)
				fields.Add(new JoinField(fieldTable, tableDimension, fieldDimension));
			// Devuelve los campos
			return fields;			
	}

	/*
		/// <summary>
		///		Obtiene la SQL de la dimensión
		/// </summary>
		private string GetSqlJoin(ParserJoinSectionModel section, ParserJoinDimensionSectionModel joinDimension, QueryDimensionModel queryDimension)
		{
			List<JoinField> joinFields = GetJoinFields(joinDimension, queryDimension);
			string sql = $"{section.GetJoin()} {GetJoinDimensionTableName(joinDimension)}";

				// Añade la comparación con los campos solicitados para la dimensión
				if (joinDimension.WithRequestedFields)
					sql = GetJoinCompareRequestedFields(section.Table, joinDimension.TableAlias, queryDimension, joinDimension.CheckIfNull);
				else if (joinDimension.Fields.Count > 0)
					sql = GetJoinCompareFields(section.Table, joinDimension.TableAlias, joinDimension.Fields, joinDimension.CheckIfNull);
				else
					sql = GetJoinPrimaryKeys(section.Table, joinDimension.TableAlias, queryDimension, joinDimension.CheckIfNull);
				// Devuelve la cadena SQL
				return sql;

			// Obtiene el nombre de la tabla de la dimensión para el Join teniendo en cuenta si tiene o no alias
			string GetJoinDimensionTableName(ParserJoinDimensionSectionModel joinDimension)
			{
				if (!string.IsNullOrWhiteSpace(joinDimension.TableAlias) && 
						!joinDimension.TableAlias.Equals(joinDimension.TableAlias, StringComparison.CurrentCultureIgnoreCase))
					return $"{joinDimension.Table} AS {joinDimension.TableAlias}";
				else
					return joinDimension.Table;
			}
		}

		/// <summary>
		///		Obtiene las comparaciones entre campos
		/// </summary>
		private string GetJoinCompareFields(string tableSource, string tableTarget, List<(string fieldDimension, string fieldTable)> fields, bool checkIfNull)
		{
			string sql = string.Empty;

				// Añade las comparaciones
				foreach ((string fieldDimension, string fieldTable) in fields)
					sql = sql.AddWithSeparator(GetFieldCompare(tableSource, fieldTable, tableTarget, fieldTable, checkIfNull), " AND ") + Environment.NewLine;
				// Devuelve la cadena SQL
				return sql;			
		}

		/// <summary>
		///		Obtiene la comparación con los campos definidos en la consulta
		/// </summary>
		private string GetJoinCompareRequestedFields(string tableSource, string tableTarget, QueryDimensionModel queryDimension, bool checkIfNull)
		{
		aquí me quedé
		throw new NotImplementedException();
		}

		/// <summary>
		///		Genera el JOIN con la clave principal
		/// </summary>
		private string GetJoinPrimaryKeys(string table, string tableAlias, QueryDimensionModel queryDimension, bool checkIfNull)
		{
		throw new NotImplementedException();
		}

		/// <summary>
		///		Obtiene una cadena de comparación de campos
		/// </summary>
		private string GetFieldCompare(string tableSource, string fieldSource, string tableTarget, string fieldTarget, bool checkIfNull)
		{
			if (!checkIfNull)
				return $"{Manager.SqlTools.GetFieldName(tableSource, fieldSource)} = {Manager.SqlTools.GetFieldName(tableTarget, fieldTarget)}";
			else //TODO: debería comparar con el valor predeterminado del tipo del campo
				return $"IsNull({Manager.SqlTools.GetFieldName(tableSource, fieldSource)}, '') = IsNull({Manager.SqlTools.GetFieldName(tableTarget, fieldTarget)}, '')";
		}
	*/
	/*
		/// <summary>
		///		Concatena las SQL de relaciones
		/// </summary>
		private string ConcatSql(List<(string dimensionTable, string dimensionAlias, string join)> sqlJoins)
		{
			string sql = string.Empty, lastTable = string.Empty;

				// Genera la cadena SQL
				foreach ((string dimensionTable, string dimensionAlias, string join) in sqlJoins)
				{
					string table = string.Empty;

						// Añade la tabla
						if (string.IsNullOrWhiteSpace(lastTable) || !lastTable.Equals(dimensionAlias, StringComparison.CurrentCultureIgnoreCase))
						{
							// Asigna el nombre de tabla
							if (!dimensionTable.Equals(dimensionAlias, StringComparison.CurrentCultureIgnoreCase))
								table = $"{dimensionTable} AS {dimensionAlias}";
							else
								table = dimensionTable;
							// Guarda la última tabla añadida
							lastTable = dimensionAlias;
						}
						// Si tiene que añadir un nombre de tabla, lo añade, si no, añade una cláusula AND
						if (!string.IsNullOrWhiteSpace(table))
							sql = sql.AddWithSeparator(@$"{Section.GetJoin()} {table}
																ON ", 
														" ");
						else
							sql += " AND ";
						// Añade la relación
						sql = sql.AddWithSeparator(join, Environment.NewLine);
				}
				// Devuelve la cadena SQL generada
				return sql;
		}

		/// <summary>
		///		Obtiene las SQL de las JOINS de la relación de tablas dimensiones
		/// </summary>
		private List<(string dimensionTable, string dimensionAlias, string join)> GetSqlJoins(ParserJoinSectionModel section)
		{
			List<(string dimensionTable, string dimensionAlias, string sql)> sqlJoins = [];

				// Crea las SQL de las dimensiones
				foreach ((QueryTableModel tableSource, QueryTableModel tableDimension, bool checkIfNull) in GetRelatedTables(section))
				{
					string sql = tableSource.GetSqlJoinOn(tableDimension, checkIfNull);

						// Si hay algo que añadir, lo añade a la lista de relaciones
						if (!string.IsNullOrWhiteSpace(sql))
							sqlJoins.Add((tableDimension.NameParts.Name, tableDimension.NameParts.Alias, sql));
				}
				// Devuelve las SQL de las JOINS
				return sqlJoins;
		}

		/// <summary>
		///		Obtiene las SQL de las JOINS de la relación de tablas dimensiones
		/// </summary>
		private List<(string dimensionTable, string dimensionAlias, string join)> GetSqlJoins(ParserJoinSectionModel section)
		{
			List<(string dimensionTable, string dimensionAlias, string sql)> sqlJoins = [];

				// Crea las SQL de las dimensiones
				foreach ((QueryTableModel tableSource, QueryTableModel tableDimension, bool checkIfNull) in GetRelatedTables(section))
				{
					string sql = tableSource.GetSqlJoinOn(tableDimension, checkIfNull);

						// Si hay algo que añadir, lo añade a la lista de relaciones
						if (!string.IsNullOrWhiteSpace(sql))
							sqlJoins.Add((tableDimension.NameParts.Name, tableDimension.NameParts.Alias, sql));
				}
				// Devuelve las SQL de las JOINS
				return sqlJoins;
		}

		/// <summary>
		///		Obtiene las tablas relacionadas
		/// </summary>
		private List<(QueryTableModel tableSource, QueryTableModel tableDimension, bool checkIfNull)> GetRelatedTables(ParserJoinSectionModel join)
		{
			List<(QueryTableModel tableSource, QueryTableModel tableDimension, bool checkIfNull)> tables = [];

				// Añade las tablas de las relaciones
				foreach (ParserJoinDimensionSectionModel joinDimension in join.JoinDimensions)
					if (Manager.Request.Dimensions.IsRequested(joinDimension.DimensionKey))
					{
						(QueryTableModel? tableSource, QueryTableModel? tableDimension) = GetTablesForJoin(join, joinDimension);

							// Si se han encontrado tablas realmente, se añaden
							if (tableSource is not null && tableDimension is not null)
								tables.Add((tableSource, tableDimension, joinDimension.CheckIfNull || joinDimension.WithRequestedFields));
					}
				// Devuelve la lista de datos
				return tables;
		}

		/// <summary>
		///		Obtiene las tablas para las que se hace un JOIN
		/// </summary>
		private (QueryTableModel? tableSource, QueryTableModel? tableDimension) GetTablesForJoin(ParserJoinSectionModel join, 
																								 ParserJoinDimensionSectionModel relationDimension)
		{
			QueryTableModel? tableDimension = null, tableSource = null;

				// Crea las tablas de origen y los campos de relación
				if (relationDimension.WithRequestedFields)
				{
					// Obtiene una tabla con todos los campos solicitados para una dimensión
					tableDimension = Manager.Request.Dimensions.GetRequestedTable(relationDimension.Table, relationDimension.TableAlias, relationDimension.DimensionKey, 
																				  false, true);
					// Si tenemos una tabla de dimensión, creamos una tabla origen con los mismos campos
					if (tableDimension is not null)
						tableSource = tableDimension.Clone(join.Table, join.TableAlias);
				}
				else if (relationDimension.Fields.Count > 0)
				{
					// Crea las tablas
					tableSource = new QueryTableModel(join.Table, join.TableAlias);
					tableDimension = new QueryTableModel(relationDimension.Table, relationDimension.TableAlias);
					// Añade los campos
					foreach ((string fieldTable, string fieldDimension) in relationDimension.Fields)
					{
						tableSource.AddColumn(false, fieldTable, fieldTable, DataSourceColumnModel.FieldType.Unknown);
						tableDimension.AddColumn(false, fieldDimension, fieldDimension, DataSourceColumnModel.FieldType.Unknown);
					}
				}
				// Devuelve la tabla origen y la lista de campos
				return (tableSource, tableDimension);
		}
	*/

	/// <summary>
	///		Sección que se está generando
	/// </summary>
	internal ParserJoinSectionModel Section { get; }

	/// <summary>
	///		Dimensiones definidas en la consulta
	/// </summary>
	internal QueryDimensionsCollection QueryDimensions { get; }
}