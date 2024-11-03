using Bau.Libraries.LibHelper.Extensors;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

/// <summary>
///		Modelo de una tabla para una consulta
/// </summary>
internal class QueryTableNameModelNew
{
	// Variables privadas
	private string _alias = string.Empty;

	internal QueryTableNameModelNew(string name, string alias)
	{
		Name = name;
		Alias = alias;
	}

	/// <summary>
	///		Obtiene el nombre de un campo
	/// </summary>
	internal string GetFieldName(string table, string field) => GetFieldName(string.Empty, table, field);

	/// <summary>
	///		Obtiene el nombre de un campo
	/// </summary>
	internal string GetFieldName(string field) => GetFieldName(string.Empty, string.Empty, field);

	/// <summary>
	///		Obtiene el nombre de un campo
	/// </summary>
	internal string GetFieldName(string schema, string table, string field)
	{
		return Normalize(schema).AddWithSeparator(Normalize(table), ".", false).AddWithSeparator(Normalize(field), ".", false);

		// Normaliza un nombre de esquema, tabla o campo
		string Normalize(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return string.Empty;
			else if (!value.EndsWith("]"))
				return $"[{value.Trim()}]";
			else
				return value.Trim();
		}
	}

	/// <summary>
	///		Nombre
	/// </summary>
	internal string Name { get; }

	/// <summary>
	///		Alias de la tabla
	/// </summary>
	internal string Alias
	{
		get
		{
			if (string.IsNullOrWhiteSpace(_alias))
				return Name;
			else
				return _alias;
		}
		set { _alias = value; }
	}
}
