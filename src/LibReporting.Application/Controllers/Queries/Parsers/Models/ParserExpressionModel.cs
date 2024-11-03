namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Modelo de interpretación de una dimensión
/// </summary>
internal class ParserExpressionModel : ParserBaseSectionModel
{
	/// <summary>
	///		Nombre de la expresión
	/// </summary>
	internal string Expression { get; set; } = string.Empty;

    /// <summary>
    ///		Tabla de la que se obtiene el origen de datos
    /// </summary>
    internal string Table { get; set; } = string.Empty;

	/// <summary>
	///		Agregación (SUM, MAX...) en caso que estemos en una cláusula HAVING
	/// </summary>
	internal string Aggregation { get; set; } = "SUM";
}
