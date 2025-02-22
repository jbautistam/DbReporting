
namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Modelo de interpretación de una expresión
/// </summary>
internal class ParserIfRequestSectionExpressionModel : ParserBaseSectionModel
{
    /// <summary>
    ///     Añade las expresiones
    /// </summary>
	internal void AddExpressions(string content)
	{
        Expressions.AddRange(SplitContent(content, ","));
	}

	/// <summary>
	///		Clave de las expresiones solicitadas
	/// </summary>
	internal List<string> Expressions { get; } = [];

    /// <summary>
    ///		Consulta SQL a añadir cuando se solicita la expresión
    /// </summary>
    internal string? Sql { get; set; }

    /// <summary>
    ///		Consulta SQL a añadir cuando se solicita la expresión en una consulta con totales
    /// </summary>
    internal string? SqlTotals { get; set; }

    /// <summary>
    ///		Consulta SQL a añadir cuando no se solicita la expresión
    /// </summary>
    internal string? SqlWhenNotRequest { get; set; }
}
