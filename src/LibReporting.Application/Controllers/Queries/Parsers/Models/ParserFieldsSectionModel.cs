namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Sección con datos de dimensiones / expresiones
/// </summary>
internal class ParserFieldsSectionModel : ParserBaseSectionModel
{
    /// <summary>
    ///		Indica si se debe añadir una coma antes de la consulta
    /// </summary>
    internal bool WithPreviousComma { get; set; }

    /// <summary>
    ///		Indica si se debe añadir una coma después de la consulta
    /// </summary>
    internal bool WithComma { get; set; }

    /// <summary>
    ///		Dimensiones que se deben añadir a la lista de campos
    /// </summary>
    internal List<ParserDimensionModel> ParserDimensions { get; } = [];

    /// <summary>
    ///     Expresiones que se deben añadir a la lista de campos
    /// </summary>
    internal List<ParserIfRequestSectionExpressionModel> ParserExpressions { get; } = [];

    /// <summary>
    ///     Sql cuando se consulta con totales
    /// </summary>
    internal string? SqlTotals { get; set; }
}