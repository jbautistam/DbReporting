namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Sección con datos de un ORDER BY
/// </summary>
/// <example>
/// -OrderBy 
///     -Dimension: Users
///     -Expression: Expression
///         --Table: nombre de la tabla
///         --Field: alias del campo
///     -Sql
/// </example>
internal class ParserOrderBySectionModel : ParserBaseSectionModel
{
    /// <summary>
    ///		Dimensiones del ORDER BY
    /// </summary>
    internal List<ParserDimensionModel> Dimensions { get; } = [];

    /// <summary>
    ///     Expresiones asociadas al ORDER BY
    /// </summary>
    internal List<ParserExpressionModel> Expressions { get; } = [];

    /// <summary>
    ///		Sql adicional
    /// </summary>
    internal string? Sql { get; set; }
}