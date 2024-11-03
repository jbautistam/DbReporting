namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Sección de paginación
/// </summary>
internal class ParserSubquerySectionModel : ParserBaseSectionModel
{
    /// <summary>
    ///     Nombre de la subconsulta asociada
    /// </summary>
    internal string Name { get; set; } = string.Empty;
}
