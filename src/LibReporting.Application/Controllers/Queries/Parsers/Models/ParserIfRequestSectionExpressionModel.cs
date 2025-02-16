
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
        if (!string.IsNullOrWhiteSpace(content))
            foreach (string part in content.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                if (!Expressions.Any(item => item.Equals(part, StringComparison.CurrentCultureIgnoreCase)))
                    Expressions.Add(part);
	}

	/// <summary>
	///		Clave de las expresiones solicitadas
	/// </summary>
	internal List<string> Expressions { get; } = [];

    /// <summary>
    ///		Consulta SQL a añadir cuando se solicita la expresión
    /// </summary>
    internal string Sql { get; set; } = default!;
}
