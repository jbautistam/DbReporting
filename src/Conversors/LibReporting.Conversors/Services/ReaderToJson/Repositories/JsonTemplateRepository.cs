using AnalyticAlways.Helper.Toolbelt.Extensors;
using AnalyticAlways.Xml.Toolbelt;

using Bau.Libraries.LibReporting.Conversors.Services.ReaderToJson.Models;

namespace Bau.Libraries.LibReporting.Conversors.Services.ReaderToJson.Repositories;

/// <summary>
///     Repositorio para el contenido de las plantillas
/// </summary>
internal class JsonTemplateRepository
{
    // Constantes privadas
    private const string TagId = "Id";
    private const string TagRoot = "Template";
    private const string TagSectionFirstRow = "FirstRow";
    private const string TagSectionRows = "Rows";
    private const string TagSectionResult = "Result";
    private const string TagSectionSubtemplate = "Subtemplate";

    /// <summary>
    ///     Carga los datos de plantilla de una cadena XML
    /// </summary>
    internal JsonTemplate? Load(string fileName)
    {
        JsonTemplate? template = null;
        MLFile fileML = new Xml.Toolbelt.Services.XML.XMLParser().Load(fileName);

            // Obtiene los datos de la plantilla
            foreach (MLNode rootML in fileML.Nodes)
                if (rootML.Name == TagRoot)
                {
                    // Crea la planitlla
                    template = new(rootML.Attributes[TagId].Value.TrimIgnoreNull());
                    // Carga los datos
                    foreach (MLNode nodeML in rootML.Nodes)
                        switch (nodeML.Name)
                        {
                            case TagSectionFirstRow:
                                    template.Sections.Add(GetSection(template, JsonTemplateSection.SectionType.FirstRow, nodeML));
                                break;
                            case TagSectionRows:
                                    template.Sections.Add(GetSection(template, JsonTemplateSection.SectionType.Rows, nodeML));
                                break;
                            case TagSectionResult:
                                    template.Sections.Add(GetSection(template, JsonTemplateSection.SectionType.Result, nodeML));
                                break;
                            case TagSectionSubtemplate:
                                    template.Sections.Add(GetSection(template, JsonTemplateSection.SectionType.Subtemplate, nodeML));
                                break;
                        }
                }
            // Devuelve los datos de la plantilla
            return template;
    }

    /// <summary>
    ///     Obtiene los datos de una sección
    /// </summary>
    private JsonTemplateSection GetSection(JsonTemplate template, JsonTemplateSection.SectionType type, MLNode rootML)
    {
        return new JsonTemplateSection(template)
                        {
                            Id = rootML.Attributes[TagId].Value.TrimIgnoreNull(),
                            Type = type,
                            Content = rootML.Value.TrimIgnoreNull()
                        };
    }
}
