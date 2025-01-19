namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Modelo de interpretación de una expresión
/// </summary>
internal class ParserIfRequestExpressionSectionModel : ParserBaseSectionModel
{
    /// <summary>
    ///     Añade una expresión
    /// </summary>
    internal void AddExpressions(string expression)
    { 
        if (!string.IsNullOrWhiteSpace(expression))
            foreach (string part in expression.Split(';'))
                if (!string.IsNullOrWhiteSpace(part))
                    ExpressionKeys.Add(part.Trim());
    }

    /// <summary>
    ///     Indica si se debe añadir sin comprobar la lista de expresiones (porque la queremos siempre se haya
    /// solicitado lo que se haya solicitado. Por ejemplo resulta útil cuando lo que tenemos es el conteo de
    /// número de registros)
    /// </summary>
    internal bool IsDefault { get; set; }

	/// <summary>
	///		Clave de las expresiones solicitadas
	/// </summary>
	internal List<string> ExpressionKeys { get; } = [];

    /// <summary>
    ///     Indica si se debe añadir coma a la SQL
    /// </summary>
    internal bool WithComma { get; set; } = true;

    /// <summary>
    ///		Consulta SQL a añadir cuando se solicita la expresión
    /// </summary>
    internal string? Sql { get; set; }
}
