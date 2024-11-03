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
	internal QueryRelationGenerator(ReportQueryGenerator manager, ParserJoinSectionModel section) : base(manager)
	{
		Section = section;
	}

	/// <summary>
	///		Obtiene la SQL
	/// </summary>
	internal override string GetSql()
	{
		if (Section.JoinDimensions.Count == 0)
			throw new Exceptions.ReportingParserException($"Can't find relations at join {Section.Join.ToString()} with table {Section.Table}");
		else
		{
			string sql = ConcatSql(GetSqlJoins(Section));

				// Añade la SQL adicional. Si no se ha generado ningún SQL para el JOIN añade el SQL adicional
				if (!string.IsNullOrWhiteSpace(sql))
					sql = sql.AddWithSeparator(Section.Sql, " AND ");
				else if (!string.IsNullOrWhiteSpace(Section.SqlNoDimension))
					sql = $" {Section.GetJoin()} {Section.SqlNoDimension}";
				// Devuelve la cadena SQL generada
				return sql;
		}
	}

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
			foreach ((QueryTableModelNew tableSource, QueryTableModelNew tableDimension, bool checkIfNull) in GetRelatedTables(section))
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
	private List<(QueryTableModelNew tableSource, QueryTableModelNew tableDimension, bool checkIfNull)> GetRelatedTables(ParserJoinSectionModel join)
	{
		List<(QueryTableModelNew tableSource, QueryTableModelNew tableDimension, bool checkIfNull)> tables = [];

			// Añade las tablas de las relaciones
			foreach (ParserJoinDimensionSectionModel joinDimension in join.JoinDimensions)
				if (Manager.Request.IsDimensionRequested(joinDimension.DimensionKey))
				{
					(QueryTableModelNew? tableSource, QueryTableModelNew? tableDimension) = GetTablesForJoin(join, joinDimension);
				
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
	private (QueryTableModelNew? tableSource, QueryTableModelNew? tableDimension) GetTablesForJoin(ParserJoinSectionModel join, 
																								   ParserJoinDimensionSectionModel relationDimension)
	{
		QueryTableModelNew? tableDimension = null, tableSource = null;

			// Crea las tablas de origen y los campos de relación
			if (relationDimension.WithRequestedFields)
			{
				// Obtiene una tabla con todos los campos solicitados para una dimensión
				tableDimension = Manager.Request.GetRequestedTable(relationDimension.Table, relationDimension.TableAlias, relationDimension.DimensionKey, 
																   false, true);
				// Si tenemos una tabla de dimensión, creamos una tabla origen con los mismos campos
				if (tableDimension is not null)
					tableSource = tableDimension.Clone(join.Table, join.TableAlias);
			}
			else if (relationDimension.Fields.Count > 0)
			{
				// Crea las tablas
				tableSource = new QueryTableModelNew(join.Table, join.TableAlias);
				tableDimension = new QueryTableModelNew(relationDimension.Table, relationDimension.TableAlias);
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
	
	/// <summary>
	///		Sección que se está generando
	/// </summary>
	internal ParserJoinSectionModel Section { get; }
}