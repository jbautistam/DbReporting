namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Sección base con datos de una condición (WHERE o HAVING)
/// </summary>
internal class ParserCondiciontSectionModel : ParserBaseSectionModel
{
    /// <summary>
    ///     Sql adicional
    /// </summary>
    internal string Sql { get; set; } = string.Empty;

    /// <summary>
    ///     Operador con el que se va a asociar la SQL
    /// </summary>
    internal string Operator { get; set; } = "AND";
}
