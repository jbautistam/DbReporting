using System.Linq;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

/// <summary>
///		Datos de una consulta SQL
/// </summary>
internal class QuerySqlModel
{
	/// <summary>
	///		Tipo de consulta
	/// </summary>
	internal enum QueryType
	{
		/// <summary>Bloque de consultas</summary>
		Block,
		/// <summary>Cte</summary>
		Cte,
		/// <summary>Consulta</summary>
		Query,
		/// <summary>Bloque de ejecución</summary>
		Execution
	}

	internal QuerySqlModel(QueryType type, string key, string sql)
	{
		Type = type;
		Key = key;
		Sql = sql;
	}

	/// <summary>
	///		Obtiene una consulta por su clave
	/// </summary>
	internal QuerySqlModel? GetQuery(string key)
	{
		// Busca la consulta en la lista
		foreach (QuerySqlModel query in Queries)
			if (query.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase))
				return query;
			else
			{ 
				QuerySqlModel? queryFound = query.GetQuery(key);

					// Si se ha encontrado en alguna de las consultas hija, la devuelve
					if (queryFound is not null)
						return queryFound;
			}
		// Si ha llegado hasta aquí es porque no ha encontrado nada
		return null;
	}

	/// <summary>
	///		Indica si hay alguna consulta de CTE en la lista
	/// </summary>
	internal bool ExistsCte() => Queries.Any(item => item.Type == QueryType.Cte);

	/// <summary>
	///		Tipo de consulta
	/// </summary>
	internal QueryType Type { get; }

	/// <summary>
	///		Clave de la consulta
	/// </summary>
	internal string Key { get; }

	/// <summary>
	///		Cadena SQL
	/// </summary>
	internal string Sql { get; }

	/// <summary>
	///		Indica si es una subconsulta
	/// </summary>
	internal bool IsSubquery { get; set; }

	/// <summary>
	///		Consultas hija
	/// </summary>
	internal List<QuerySqlModel> Queries { get; } = new();
}
