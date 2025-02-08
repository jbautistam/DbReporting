namespace Bau.Libraries.LibReporting.Conversors.Services.ReaderToJson.Models;

/// <summary>
///     Sección de la plantilla
/// </summary>
internal class JsonTemplateSection
{
    /// <summary>
    ///     Tipo de sección
    /// </summary>
    internal enum SectionType
    {
        /// <summary>Sección que se aplica a la primera fila</summary>
        FirstRow,
        /// <summary>Sección que se aplica a cada una de las filas de resultado</summary>
        Rows,
        /// <summary>Sección que se aplica al resultado final</summary>
        Result,
        /// <summary>Sección para una subplantilla</summary>
        Subtemplate
    }

    internal JsonTemplateSection(JsonTemplate template)
    {
        Template = template;
    }

    /// <summary>
    ///     Plantilla a la que se asocia la sección
    /// </summary>
    internal JsonTemplate Template { get; }

    /// <summary>
    ///     Tipo de sección
    /// </summary>
    internal SectionType Type { get; set; }

    /// <summary>
    ///     Identificador de la sección (para las subplantillas)
    /// </summary>
    internal string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Contenido de la sección
    /// </summary>
    internal string Content { get; set; } = string.Empty;
}
