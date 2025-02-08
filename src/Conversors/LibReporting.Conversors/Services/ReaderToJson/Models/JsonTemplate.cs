namespace Bau.Libraries.LibReporting.Conversors.Services.ReaderToJson.Models;

/// <summary>
///     Clase con los datos de la plantilla
/// </summary>
internal class JsonTemplate
{
    internal JsonTemplate(string id)
    {
        Id = id;
    }

    /// <summary>
    ///     Identificador de la plantilla
    /// </summary>
    internal string Id { get; }

    /// <summary>
    ///     Obtiene los datos de una sección
    /// </summary>
    internal JsonTemplateSection GetSection(JsonTemplateSection.SectionType type) => GetSection(Sections.FirstOrDefault(item => item.Type == type));

    /// <summary>
    ///     Obtiene los datos de una sección por su tipo e Id
    /// </summary>
	internal JsonTemplateSection GetSection(JsonTemplateSection.SectionType type, string id)
	{
	    return GetSection(Sections.FirstOrDefault(item => item.Type == type && item.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase)));
	}

    /// <summary>
    ///     Obtiene una sección asegurando que no sea nula
    /// </summary>
    private JsonTemplateSection GetSection(JsonTemplateSection? section) => section ?? new JsonTemplateSection(this);

	/// <summary>
	///     Secciones de la plantilla
	/// </summary>
	internal List<JsonTemplateSection> Sections { get; } = new();
}
