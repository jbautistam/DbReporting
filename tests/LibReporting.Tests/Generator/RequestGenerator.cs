using FluentAssertions;

namespace LibReporting.Tests.Generator;

/// <summary>
///		Generador de archivos de respuesta a partir de las solicitudes
/// </summary>
public class RequestGenerator
{
	/// <summary>
	///		Genera los archivos de respuesta de todos los archivos de solicitud
	/// </summary>
	[Fact(Skip = "Sólo cuando sea necesario regenerar las respuestas")]
	public void generate_all_response_files()
	{
		Dictionary<string, List<string>> reports = Tools.FileHelper.GetReports();
		string error = string.Empty;

			// Recorre los esquemas e informes procesando las solicitudes / respuestas
			foreach (KeyValuePair<string, List<string>> report in reports)
				foreach (string reportFile in report.Value)
				{
					string pathRequest = Path.Combine(Path.GetDirectoryName(reportFile) ?? string.Empty, 
													  reportFile[..^".report.xml".Length]);

						if (!Directory.Exists(pathRequest))
							error += $"Can't find request for report '{Path.GetFileName(reportFile)}'";
						else
							error += GenerateFilesFromRequest(report.Key, pathRequest);
				}
			// Comprueba los errores
			error.Should().BeNullOrWhiteSpace();
	}

	/// <summary>
	///		Genera los archivos de respuesta de un informe en concreto (todas las solicitudes de un directorio)
	/// </summary>
	[Theory(Skip = "Sólo cuando sea necesario regenerar las respuestas de un archivo")]
	[InlineData("Sales/Test-Sales.Reporting.xml", "Sales/Sales")]
	public void generate_response_files(string schemaFile, string pathRequest)
	{
		string error;

			// Genera los datos de informe
			error = GenerateFilesFromRequest(Tools.FileHelper.GetFullFileName(schemaFile), 
											 Tools.FileHelper.GetFullFileName(pathRequest));
			// Comprueba los errores
			error.Should().BeNullOrWhiteSpace();
	}

	/// <summary>
	///		Genera el archivo de respuesta
	/// </summary>
	private string GenerateFilesFromRequest(string schemaFile, string pathRequest)
	{
		string error = string.Empty;

			// Genera las respuestas de los archivos de solicitud
			foreach (string requestFile in Directory.GetFiles(pathRequest, "*.request.xml"))
				try
				{
					GenerateResponse(schemaFile, requestFile);
				}
				catch (Exception exception)
				{
					error += $"Error when process {requestFile}. {exception.Message}" + Environment.NewLine;
				}
			// Devuelve la cadena de error
			return error;
	}

	/// <summary>
	///		Genera los archivo de respuesta de una solicitud
	/// </summary>
	private void GenerateResponse(string schemaFile, string requestFile)
	{
		for (int page = 0; page < 3; page++)
			SaveFile(Tools.FileHelper.GetResponseFile(requestFile, page), 
					 Tools.ReportHelper.GetSqlResponse(schemaFile, requestFile, page));
	}

	/// <summary>
	///		Graba el texto de un archivo de respuesta
	/// </summary>
	private void SaveFile(string fileName, string sql)
	{
		// Quita el directorio de depuración para que se graben en el archivo de proyectos
		fileName = fileName.Replace("\\bin\\Debug\\net8.0", string.Empty);
		fileName = fileName.Replace("/bin/Debug/net8.0", string.Empty);
		// Escribe el archivo
		File.WriteAllText(fileName, sql);
	}
}