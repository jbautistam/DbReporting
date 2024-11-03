namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Modelo de interpretación de los campos de una dimensión
/// </summary>
internal class ParserFieldsDimensionSectionModel : ParserBaseSectionModel
{
	/// <summary>
	///		Clave de la dimensión
	/// </summary>
	internal string DimensionKey { get; set; } = string.Empty;

    /// <summary>
    ///		Tabla de la dimensión
    /// </summary>
    internal string Table { get; set; } = string.Empty;

    /// <summary>
    ///		Tabla adicional de la dimensión (para los IfNull)
    /// </summary>
    internal string AdditionalTable { get; set; } = string.Empty;

    /// <summary>
    ///		Indica si se deben incluir las claves primarias en la lista de campos
    /// </summary>
    internal bool WithPrimaryKeys { get; set; }

    /// <summary>
    ///     Indica si se deben incluir los campos solicitados (sean o no claves primarias)
    /// </summary>
    internal bool WithRequestedFields { get; set; }
}