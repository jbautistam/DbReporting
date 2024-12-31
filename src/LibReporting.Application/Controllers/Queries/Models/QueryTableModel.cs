using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

/// <summary>
///		Modelo de una tabla para una consulta
/// </summary>
internal class QueryTableModel
{
	internal QueryTableModel(string table, string alias)
	{
		NameParts = new QueryTableNameModel(table, alias);
	}

	/// <summary>
	///		Clona la tabla cambiándole los nombres
	/// </summary>
	internal QueryTableModel Clone(string table, string alias)
	{
		QueryTableModel queryTable = new(table, alias);

			// Añade los campos
			foreach (QueryTableColumnModel column in Columns)
				queryTable.AddColumn(column.IsPrimaryKey, column.NameParts.Name, column.NameParts.Alias, column.Type);
			// Devuelve la tabla generada
			return queryTable;
	}

	/// <summary>
	///		Añade una columna
	/// </summary>
	internal void AddColumn(bool isPrimaryKey, string name, string alias, DataSourceColumnModel.FieldType type)
	{
		Columns.Add(new QueryTableColumnModel(this, isPrimaryKey, name, alias, type));
	}

	/// <summary>
	///		Obtiene la SQL de un join con otra tabla
	/// </summary>
	internal string GetSqlJoinOn(QueryTableModel target, bool checkIfNull)
	{
		string sql = string.Empty;

			// Obtiene la cadena SQL con las condiciones
			if (target.Columns.Count != Columns.Count)
				throw new Exceptions.ReportingParserException($"Join {NameParts.Name} - {target.NameParts.Name}: the number of fields does not match");
			else
				for (int index = 0; index < Columns.Count; index++)
					sql = sql.AddWithSeparator($"{ComposeField(Columns[index], checkIfNull)} = {ComposeField(target.Columns[index], checkIfNull)}",
											   Environment.NewLine + " AND ");
			// Devuelve la cadena SQL
			return sql;

		// Rutina para componer el nombre del campo
		string ComposeField(QueryTableColumnModel column, bool useIsNull)
		{
			string sql = column.GetFieldName();

				// Añade la función IsNull si es necesario
				//TODO: debería coger el valor predeterminado del tipo en el IsNull 
				if (useIsNull)
					return $"IsNull({sql}, '')";
				else
					return sql;
		}
	}

	/// <summary>
	///		Partes del nombre de tabla
	/// </summary>
	internal QueryTableNameModel NameParts { get; }

	/// <summary>
	///		Columnas
	/// </summary>
	internal List<QueryTableColumnModel> Columns { get; } = [];
}