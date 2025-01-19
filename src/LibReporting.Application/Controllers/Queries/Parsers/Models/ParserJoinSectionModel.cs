namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Sección con datos de un JOIN
/// </summary>
internal class ParserJoinSectionModel : ParserBaseSectionModel
{
	//TODO: ¿esto no es lo mismo que QueryJoinModel?
	/// <summary>
	///		Tipo de join
	/// </summary>
	public enum JoinType
	{
		/// <summary>INNER JOIN</summary>
		InnerJoin,
		/// <summary>LEFT JOIN</summary>
		LeftJoin,
		/// <summary>RIGHT JOIN</summary>
		RightJoin,
		/// <summary>FULL OUTER JOIN</summary>
		FullJoin,
		/// <summary>CROSS JOIN</summary>
		CrossJoin
	}

	/// <summary>
	///		Tipo de unión
	/// </summary>
	internal JoinType Join { get; set; }

    /// <summary>
    ///     Nombre de tabla con la que se relaciona la dimensión
    /// </summary>
    internal string Table { get; set; } = default!;

    /// <summary>
    ///     Relaciones asociadas al JOIN
    /// </summary>
    internal List<ParserJoinDimensionSectionModel> JoinDimensions { get; } = [];

    /// <summary>
    ///     Sql adicional
    /// </summary>
    internal string? Sql { get; set; }

    /// <summary>
    ///     Sql que se debe añadir cuando no hay nada en el Join
    /// </summary>
    internal string? SqlNoDimension { get; set; }
}
