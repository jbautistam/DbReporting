namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Modelo de interpretación de una expresión
/// </summary>
internal class ParserIfRequestSectionModel : ParserBaseSectionModel
{
	/// <summary>
	///     Expresiones solicitadas
	/// </summary>
	internal List<ParserIfRequestExpressionSectionModel> Expressions { get; } = new();

    /// <summary>
    ///     Expresiones que se comprueban cuando se solicitan totales
    /// </summary>
    internal List<ParserIfRequestExpressionSectionModel> WhenRequestTotals { get; } = new();

    /// <summary>
    ///     Indica si se debe añadir una coma al generar la SQL
    /// </summary>
    internal bool WithComma { get; set; }
}