namespace LibReporting.Tests.Tools;

/// <summary>
///		Clase de ayuda para tratamiento de archivos
/// </summary>
internal static class FileHelper
{
	/// <summary>
	///		Obtiene el nombre completo de un archivo
	/// </summary>
	internal static string GetFullFileName(string fileName) => Path.Combine(GetDataPath(), fileName);

	/// <summary>
	///		Busca el directorio "Data" a partir del directorio ejecutable
	/// </summary>
	internal static string GetDataPath()
	{
		string path = Path.GetDirectoryName(GetExecutionPath())!;

			// Busca el directorio Data
			while (!Directory.Exists(Path.Combine(path!, "Data")))
				path = Path.GetDirectoryName(path)!;
			// Devuelve el directorio Data
			return Path.Combine(path, "Data");
	}

	/// <summary>
	///		Obtiene el directorio de ejecución del proyecto
	/// </summary>
	private static string GetExecutionPath() => Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? default!;

	/// <summary>
	///		Obtiene el directorio de desarrollo del proyecto
	/// </summary>
	internal static string GetDevelopmentDataPath()
	{
		string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? default!;

			// Busca el directorio del proyecto
			while (!string.IsNullOrWhiteSpace(path) && !File.Exists(Path.Combine(path, "LibReporting.Tests.csproj")))
				path = Path.GetDirectoryName(path) ?? string.Empty;
			// Combina el directorio de datos
			if (!string.IsNullOrWhiteSpace(path))
				path = Path.Combine(path, "Data");
			// Devuelve el directorio
			return path;
	}

	/// <summary>
	///		Obtiene la lista de archivos de esquemas del directorio Data
	/// </summary>
	public static List<string> GetSchemasFiles()
	{
		List<string> files = [];

			// Busca los directorios del directorio Data
			foreach (string path in Directory.GetDirectories(GetDataPath()))
			{
				string schema = string.Empty;

					// Obtiene el archivo de esquema
					if (Directory.Exists(Path.Combine(path, "Reports")))
						foreach (string file in Directory.GetFiles(path))
							if (file.EndsWith(".reporting.xml", StringComparison.CurrentCultureIgnoreCase))
								schema = file;
					// Añade el directorio de esquema
					if (!string.IsNullOrWhiteSpace(schema))
						files.Add(schema);
			}
			// Devuelve los informes
			return files;
	}

	/// <summary>
	///		Obtiene la lista de esquemas e informes del directorio Data
	/// </summary>
	public static Dictionary<string, List<string>> GetReports()
	{
		Dictionary<string, List<string>> reports = [];

			// Busca los directorios del directorio Data
			foreach (string path in Directory.GetDirectories(GetDataPath()))
			{
				string schema = string.Empty;
				List<string> reportFiles = [];

					// Obtiene el archivo de esquema
					foreach (string file in Directory.GetFiles(path))
						if (file.EndsWith(".reporting.xml", StringComparison.CurrentCultureIgnoreCase))
							schema = file;
					// Obtiene los informes
					if (Directory.Exists(Path.Combine(path, "Reports")))
						foreach (string file in Directory.GetFiles(Path.Combine(path, "Reports")))
							if (file.EndsWith(".report.xml", StringComparison.CurrentCultureIgnoreCase))
								reportFiles.Add(file);
					// Crea el diccionario
					if (!string.IsNullOrWhiteSpace(schema) && reportFiles.Count > 0)
						reports.Add(schema, reportFiles);
			}
			// Devuelve los informes
			return reports;
	}

	/// <summary>
	///		Obtiene el nombre del archivo de respuesta
	/// </summary>
	internal static string GetResponseFile(string requestFile, int page = 0) 
	{
		string extension;

			// Añade el número de página de la solicitud
			if (page > 0)
				extension = $".response.{page.ToString()}.sql";
			else
				extension = ".response.sql";
			// Cambia la extensión del archivo
			return ChangeFileExtension(requestFile, ".request.xml", extension);
	}

	/// <summary>
	///		Obtiene el nombre de archivo de plantilla a partir del archivo de solicitud
	/// </summary>
	internal static string GetOutputTemplateFileName(string reportFile)
	{
		// Cambia el directorio añadiéndole la carpeta _OutputTemplates
		reportFile = Path.Combine(Path.GetDirectoryName(reportFile) ?? string.Empty, 
								  "_OutputTemplates", Path.GetFileName(reportFile));
		// Cambia la extensión del archivo
		return ChangeFileExtension(reportFile, ".report.xml", ".template.output.xml");
	}

	/// <summary>
	///		Obtiene el nombre de archivo JSON de salida a partir del archivo de solicitud
	/// </summary>
	internal static string GetOutputResponseFileName(string requestFile) => ChangeFileExtension(requestFile, ".request.xml", ".output.json");

	/// <summary>
	///		Obtiene el nombre de archivo JSON de salida a partir del archivo de solicitud
	/// </summary>
	private static string ChangeFileExtension(string sourceFile, string extension, string targetExtensions)
	{
		string file = Path.GetFileName(sourceFile);

			// Obtiene el nombre del archivo de respuesta
			if (file.EndsWith(extension, StringComparison.CurrentCultureIgnoreCase))
			{
				// Cambia la extensión por la extensión final
				file = file.Replace(extension, targetExtensions, StringComparison.CurrentCultureIgnoreCase);
				// Devuelve el nombre completo del archivo
				return Path.Combine(Path.GetDirectoryName(sourceFile) ?? string.Empty, file);
			}
			else
				return string.Empty;
	}
}
