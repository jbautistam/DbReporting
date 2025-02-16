namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

/// <summary>
///		Clase con los datos de un JOIN
/// </summary>
internal class QueryDimensionJoinModel
{
	/// <summary>
	///		Tipo de unión
	/// </summary>
	internal enum JoinType
	{
		/// <summary>LEFT JOIN</summary>
		Left,
		/// <summary>INNER JOIN</summary>
		Inner,
		/// <summary>RIGHT JOIN</summary>
		Right,
		/// <summary>FULL OUTER JOIN</summary>
		Full,
		/// <summary>CROSS JOIN</summary>
		Cross
	}

	internal QueryDimensionJoinModel(JoinType type, QueryDimensionModel query, string alias)
	{
		Type = type;
		Query = query;
		Alias = alias;
	}

	/// <summary>
	///		Tipo de JOIN
	/// </summary>
	internal JoinType Type { get; }

	/// <summary>
	///		Subconsulta
	/// </summary>
	internal QueryDimensionModel Query { get; }

	/// <summary>
	///		Alias de la subconsulta
	/// </summary>
	internal string Alias { get; }

	/// <summary>
	///		Campos de relación
	/// </summary>
	internal List<QueryDimensionRelationModel> Relations { get; } = [];
}
