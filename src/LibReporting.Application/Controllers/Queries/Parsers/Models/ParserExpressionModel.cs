namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Modelo de interpretación de una dimensión
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
    ///		Tabla de la que se obtiene el origen de datos
    /// </summary>
    internal string Table { get; set; } = default!;

    /// <summary>
    ///		Campo / alias en la tabla
    /// </summary>
    internal string Field { get; set; } = default!;

	/// <summary>
	///		Agregación (SUM, MAX...) en caso que estemos en una cláusula HAVING
	/// </summary>
	internal string Aggregation { get; set; } = "SUM";
}
