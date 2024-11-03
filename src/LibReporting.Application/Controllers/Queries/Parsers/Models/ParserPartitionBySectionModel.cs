namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Sección con datos de un PARTITION BY
/// </summary>
/// <example>
/// -PartitionBy 
///     -Dimension:Users
///     -Additional
///         YearMonth
///     -OrderBy
///         Date
/// </example>
internal class ParserPartitionBySectionModel : ParserBaseSectionModel
{
    /// <summary>
    ///		Dimensiones del PARTITION BY
    /// </summary>
    internal List<ParserDimensionModel> Dimensions { get; } = new();

    /// <summary>
    ///		Campos adicionales
    /// </summary>
    internal string Additional { get; set; } = string.Empty;

    /// <summary>
    ///		Campos de ORDER BY
    /// </summary>
    internal string OrderBy { get; set; } = string.Empty;
}
