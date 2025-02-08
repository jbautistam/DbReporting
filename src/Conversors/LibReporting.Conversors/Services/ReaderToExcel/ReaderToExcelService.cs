using System.Data;

using Bau.Libraries.LibReporting.Conversors.Services.ReaderToExcel.Models;

namespace Bau.Libraries.LibReporting.Conversors.Services.ReaderToExcel;

/// <summary>
///		Servicio de generación de Excel a partir de un <see cref="IDataReader"/>
/// </summary>
internal class ReaderToExcelService
{
	internal ReaderToExcelService(ReaderService manager)
	{
		Manager = manager;
	}

	/// <summary>
	///		Carga todas las plantillas de un directorio
	/// </summary>
	internal void Load()
	{
		foreach (string fileName in Directory.GetFiles(Manager.Configuration.TemplatesPath, "*" + Manager.Configuration.TemplatesExcelExtension))
		{
			ExcelTemplate? template = new Repositories.ExcelTemplateRepository().Load(fileName);

				// Añade la plantilla
				if (template is not null)
					ExcelTemplates.Add(template.Id, template);
		}
	}

	/// <summary>
	///		Limpia los datos del servicio
	/// </summary>
	internal void Clear()
	{
		ExcelTemplates.Clear();
	}

	/// <summary>
	///		Transforma un <see cref="IDataReader"/> en un archivo Excel utilizando una plantilla
	/// </summary>
	internal void Transform(string templateId, string fileName, IDataReader reader)
	{
		if (!ExcelTemplates.TryGetValue(templateId, out ExcelTemplate? template))
			throw new ArgumentException($"Can't find the Excel template {templateId}");
		else
			new Builders.ExcelFileBuilder(template).Generate(fileName, reader);
	}

	/// <summary>
	///		Manager principal
	/// </summary>
	internal ReaderService Manager { get; }

	/// <summary>
	///		Diccionario de plantillas
	/// </summary>
	private Dictionary<string, ExcelTemplate> ExcelTemplates { get; } = new(StringComparer.InvariantCultureIgnoreCase);
}