using Bau.Libraries.LibReporting.Conversors.Models;

namespace Bau.Libraries.LibReporting.Conversors;

/// <summary>
///     Servicio para lectura de datos y transformación en un Json de salida
/// </summary>
public class ReaderService
{
	public ReaderService(Configuration configuration)
	{
		Configuration = configuration;
		JsonConversorService = new Services.ReaderToJson.ReaderToJsonService(this);
		ExcelConversorService = new Services.ReaderToExcel.ReaderToExcelService(this);
	}

	/// <summary>
	///		Carga todas las plantillas de un directorio
	/// </summary>
	public void Load()
	{
		JsonConversorService.Load();
		ExcelConversorService.Load();
	}

	/// <summary>
	///		Limpia los datos del servicio
	/// </summary>
	public void Clear()
	{
		JsonConversorService.Clear();
		ExcelConversorService.Clear();
	}
	
	/// <summary>
    ///     Lee los datos de un <see cref="System.Data.IDataReader"/> y crea un Json de salida
    /// </summary>
    public string TransformToJson(string templateId, Request request, System.Data.IDataReader reader)
	{
		return JsonConversorService.TransformToJson(templateId, request, reader);
	}
	
	/// <summary>
    ///     Lee los datos de un <see cref="System.Data.IDataReader"/> y crea un Excel de salida
    /// </summary>
    public void TransformToExcel(string templateId, string fileName, System.Data.IDataReader reader)
	{
		ExcelConversorService.Transform(templateId, fileName, reader);
	}

	/// <summary>
	///		Configuración
	/// </summary>
	public Configuration Configuration { get; }

	/// <summary>
	///		Conversor de <see cref="System.Data.IDataReader"/> a Json
	/// </summary>
	internal Services.ReaderToJson.ReaderToJsonService JsonConversorService { get; }

	/// <summary>
	///		Conversor de <see cref="System.Data.IDataReader"/> a Excel
	/// </summary>
	internal Services.ReaderToExcel.ReaderToExcelService ExcelConversorService { get; }
}