using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

/// <summary>
///		Modelo de una columna para una consulta
/// </summary>
internal class QueryTableColumnModel
{
	internal QueryTableColumnModel(QueryTableModel table, bool isPrimaryKey, string name, string alias, DataSourceColumnModel.FieldType type)
	{
		Table = table;
		IsPrimaryKey = isPrimaryKey;
		NameParts = new QueryTableNameModel(name, alias);
		Type = type;
	}

	/// <summary>
	///		Obtiene el nombre del campo
	/// </summary>
	internal string GetFieldName() => NameParts.GetFieldName(Table.NameParts.Alias, NameParts.Alias);

	/// <summary>
	///		Obtiene el nombre del campo
	/// </summary>
	internal string GetFieldName(string table) => NameParts.GetFieldName(table, NameParts.Alias);

	/// <summary>
	///		Tabla a la que se asocia la columna
	/// </summary>
	internal QueryTableModel Table { get; }

	/// <summary>
	///		Indica si es una clave primaria
	/// </summary>
	internal bool IsPrimaryKey { get; }

	/// <summary>
	///		Partes del nombre del campo
	/// </summary>
	internal QueryTableNameModel NameParts { get; }

	/// <summary>
	///		Tipo de la columna
	/// </summary>
	internal DataSourceColumnModel.FieldType Type { get; }
}