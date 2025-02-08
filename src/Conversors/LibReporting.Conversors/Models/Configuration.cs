namespace Bau.Libraries.LibReporting.Conversors.Models;

/// <summary>
///		Datos de configuración
/// </summary>
public class Configuration
{
	public Configuration(string templatesPath)
	{
		TemplatesPath = templatesPath;
	}

	/// <summary>
	///		Directorio de las plantillas
	/// </summary>
	public string TemplatesPath { get; set; }

	/// <summary>
	///		Extensión de las plantillas Json
	/// </summary>
	public string TemplatesJsonExtension { get; set; } = ".template.json.xml";

	/// <summary>
	///		Extensión de las plantillas Excel
	/// </summary>
	public string TemplatesExcelExtension { get; set; } = ".template.excel.xml";
}
