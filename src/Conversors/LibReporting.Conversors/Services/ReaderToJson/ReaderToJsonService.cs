using System.Data;

using Bau.Libraries.LibReporting.Conversors.Services.ReaderToJson.Builders;
using Bau.Libraries.LibReporting.Conversors.Models;
using Bau.Libraries.LibReporting.Conversors.Services.ReaderToJson.Models;

namespace Bau.Libraries.LibReporting.Conversors.Services.ReaderToJson;

/// <summary>
///		Conversor de <see cref="IDataReader"/> a JSON
/// </summary>
internal class ReaderToJsonService
{
	internal ReaderToJsonService(ReaderService manager)
	{
		Manager = manager;
	}

	/// <summary>
	///		Carga todas las plantillas de un directorio
	/// </summary>
	internal void Load()
	{
		foreach (string fileName in Directory.GetFiles(Manager.Configuration.TemplatesPath, "*" + Manager.Configuration.TemplatesJsonExtension))
		{
			JsonTemplate? template = new Repositories.JsonTemplateRepository().Load(fileName);

				// Añade la plantilla
				if (template is not null)
					JsonTemplates.Add(template.Id, template);
		}
	}

	/// <summary>
	///		Limpia los datos del servicio
	/// </summary>
	internal void Clear()
	{
		JsonTemplates.Clear();
	}
	
	/// <summary>
	///		Obtiene una plantilla
	/// </summary>
	internal JsonTemplate? GetTemplate(string id)
	{
		if (JsonTemplates.TryGetValue(id, out JsonTemplate? template))
			return template;
		else
			return null;
	}

	/// <summary>
    ///     Lee los datos resultantes de base de datos y aplica una plantilla
    /// </summary>
    internal string TransformToJson(string templateId, Request request, IDataReader reader)
	{
		JsonTemplate? template = GetTemplate(templateId);

			if (template is null)
				throw new ArgumentOutOfRangeException($"Can't find the template {templateId}");
			else
			{
				JsonTemplateBuilder builderFirstRow = new(this, template.GetSection(JsonTemplateSection.SectionType.FirstRow));
				JsonTemplateBuilder builderRows = new(this, template.GetSection(JsonTemplateSection.SectionType.Rows));
				long records = ReadData(reader, builderFirstRow, builderRows);

					// Obtiene el Json
					return GetResultJson(template.GetSection(JsonTemplateSection.SectionType.Result),
										 builderFirstRow.GetString(), builderRows.GetString(),
										 request, records);
			}
	}

	/// <summary>
	///		Lee los datos y transforma las secciones
	/// </summary>
	private long ReadData(IDataReader reader, JsonTemplateBuilder builderFirstRow, JsonTemplateBuilder builderRows)
	{
		long records = 0;
		bool firstRow = true;

			// Lee los datos
			while (reader.Read())
			{
				// Genera los datos de la primera línea
				if (firstRow)
				{
					// Genera los datos
					builderFirstRow.Add(reader);
					// Indica que ya no es la primera línea
					firstRow = false;
				}
				// Genera los datos de cada línea
				builderRows.Add(reader);
				// Incrementa el número de registros
				records++;
			}
			// Devuelve el número de registros generados
			return records;
	}

	/// <summary>
	///     Obtiene el Json resultante
	/// </summary>
	private string GetResultJson(JsonTemplateSection? section, string firstRow, string rows, Request request, long records)
    {
        string result = string.Empty;

            // Obtiene la cadena final
            if (section is not null && !string.IsNullOrWhiteSpace(section.Content))
                result = section.Content
                            .Replace("{{firstRow}}", firstRow, StringComparison.CurrentCultureIgnoreCase)
                            .Replace("{{rows}}", rows, StringComparison.CurrentCultureIgnoreCase)
                            .Replace("{{PaginatedTotalRows}}", records.ToString(), StringComparison.CurrentCultureIgnoreCase)
                            .Replace("{{PaginatedFirstRow}}", ((request.Page - 1) * request.Size + 1).ToString(), 
                                        StringComparison.CurrentCultureIgnoreCase)
                            .Replace("{{PaginatedSize}}", request.Size.ToString(), StringComparison.CurrentCultureIgnoreCase)
                            .Replace("{{PaginatedCountRows}}", records.ToString(), StringComparison.CurrentCultureIgnoreCase)
                            .Replace("{{PaginatedAnyRows}}", (records > 0).ToString().ToLower(), 
                                        StringComparison.CurrentCultureIgnoreCase)
                            .Replace("{{PaginatedNewFirstRow}}", (request.Page * request.Size + 1).ToString(), 
                                        StringComparison.CurrentCultureIgnoreCase);
            // Devuelve el resultado
            return result;
    }

	/// <summary>
	///		Manager principal
	/// </summary>
	internal ReaderService Manager { get; }

	/// <summary>
	///		Diccionario de plantillas
	/// </summary>
	private Dictionary<string, JsonTemplate> JsonTemplates { get; } = new(StringComparer.InvariantCultureIgnoreCase);
}