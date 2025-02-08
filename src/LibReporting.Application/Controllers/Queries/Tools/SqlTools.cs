using Bau.Libraries.LibHelper.Extensors;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Tools;

/// <summary>
///		Herramientas de SQL
/// </summary>
internal class SqlTools
{
	internal SqlTools()
	{
		SqlFilterGenerator = new SqlFilterGenerator(this);
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
			else
				return $"[{value.Trim()}]";
		}
	}

	/// <summary>
	///		Generador de SQL para filtros
	/// </summary>
	internal SqlFilterGenerator SqlFilterGenerator { get; }
}