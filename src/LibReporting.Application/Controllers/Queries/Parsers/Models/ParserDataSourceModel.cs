namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Modelo de interpretación de un origen de datos
/// </summary>
internal class ParserDataSourceModel : ParserBaseSectionModel
{
	/// <summary>
	///		Clave del origen de datos
	/// </summary>
	internal string DataSourceKey { get; set; } = string.Empty;

    /// <summary>
    ///		Campo de la que se obtiene el origen de datos
    /// </summary>
    internal string Table { get; set; } = string.Empty;
}
