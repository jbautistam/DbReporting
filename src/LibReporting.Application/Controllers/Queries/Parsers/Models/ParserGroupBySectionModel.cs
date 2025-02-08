namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Sección con datos de un GROUP BY
/// </summary>
/// <example>
/// GroupBy 
///     -Dimension:Users 
///         --Table:UsersCte
///     -Sql: sql adicional
/// </example>
internal class ParserGroupBySectionModel : ParserBaseSectionModel
{
    /// <summary>
    ///		Dimensiones del GROUP BY
    /// </summary>
    internal List<ParserDimensionModel> Dimensions { get; } = [];

    /// <summary>
    ///		Sql adicional
    /// </summary>
    internal string? Sql { get; set; }
}
