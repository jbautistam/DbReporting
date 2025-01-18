namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Sección con datos de dimensiones / expresiones
/// </summary>
internal class ParserFieldsSectionModel : ParserBaseSectionModel
{
    /// <summary>
    ///		Indica si se debe añadir una coma
    /// </summary>
    internal bool WithComma { get; set; }

    /// <summary>
    ///		Dimensiones que se deben añadir a la lista de campos
    /// </summary>
    internal List<ParserFieldsDimensionSectionModel> ParserDimensions { get; } = [];

    /// <summary>
    ///     Expresiones que se deben añadir a la lista de campos
    /// </summary>
    internal List<ParserIfRequestSectionModel> ParserExpressions { get; } = [];
}