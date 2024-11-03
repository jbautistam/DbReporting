namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Sección con datos de un ORDER BY
/// </summary>
/// <example>
/// -OrderBy 
///     -Dimension:Users
///     -Additional
///         Field2
///     -Required
/// </example>
internal class ParserOrderBySectionModel : ParserBaseSectionModel
{
    /// <summary>
    ///		Dimensiones del ORDER BY
    /// </summary>
    internal List<ParserDimensionModel> Dimensions { get; } = new();

    /// <summary>
    ///     Indica si el ORDER BY es obligatorio (por ejemplo porque tiene paginación)
    /// </summary>
    internal bool Required { get; set; }

    /// <summary>
    ///		Sql adicional
    /// </summary>
    internal string AdditionalSql { get; set; } = string.Empty;
}
