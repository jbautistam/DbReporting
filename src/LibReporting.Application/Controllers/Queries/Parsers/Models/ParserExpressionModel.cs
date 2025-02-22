namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Modelo de interpretación de una expresión
/// </summary>
internal class ParserExpressionModel : ParserBaseSectionModel
{
	internal ParserExpressionModel(string expression)
	{
		Expression = expression;
	}

	/// <summary>
	///		Nombre de la expresión
	/// </summary>
	internal string Expression { get; }

    /// <summary>
    ///		Tabla de la que se obtiene la expresión
    /// </summary>
    internal string? Table { get; set; }

    /// <summary>
    ///		Campo / alias en la tabla
    /// </summary>
    internal string? Field { get; set; }
}
