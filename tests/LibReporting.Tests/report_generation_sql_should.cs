using Shouldly;

namespace LibReporting.Tests;

/// <summary>
///		Pruebas de generación de SQL de informes avanzados
/// </summary>
public class report_generation_sql_should
{
	/// <summary>
	///		Comprueba si se puede cargar un esquema de base de datos y sus informes (para hacer uno en concreto, el método
	///		convert_to_sql_files, recoge y comprueba todos los archivos de datos)
	/// </summary>
	[Theory]
	[InlineData("Sales/Reporting-Sql Server - SalesSamples.Schema.xml", 
				"Sales/ManualRequests/Sales/Sales.request.xml")]
	public void convert_to_sql(string schema, string fileRequest)
	{
		string schemaFileName = Tools.FileHelper.GetFullFileName(schema);
		string requestFileName = Tools.FileHelper.GetFullFileName(fileRequest);
		string error = CheckResponseFile(schemaFileName, requestFileName);

			error.ShouldBeNullOrWhiteSpace();
	}

	/// <summary>
	///		Comprueba todos los archivos de informe: obtiene todos los esquemas e informes del directorio Data, obtiene todos los
	///	archivos de solicitud y los compara con las respuestas
	/// </summary>
	[Fact]
	public void convert_to_sql_files()
	{
		List<string> schemaFiles = Tools.FileHelper.GetSchemasFiles();
		string error = string.Empty;

			// Recorre los directorios de esquemas procesando las solicitudes / respuestas
			foreach (string schemaFile in schemaFiles)
			{
				string pathRequests = Path.Combine(Path.GetDirectoryName(schemaFile)!, "Requests");

					if (!Directory.Exists(pathRequests))
						error += $"Can't find the path request at folder '{Path.GetFileName(schemaFile)}'";
					else
						foreach (string requestFolder in Directory.GetDirectories(pathRequests))
							foreach (string requestFile in Directory.GetFiles(requestFolder, "*.request.xml"))
							{
								string errorCompare = CheckResponseFile(schemaFile, requestFile);

									if (!string.IsNullOrWhiteSpace(errorCompare))
										error += errorCompare + Environment.NewLine;
							}
			}
			// Comprueba los errores
			error.ShouldBeNullOrWhiteSpace();
	}

	/// <summary>
	///		Compara los archivos de respuesta con lo solicitado
	/// </summary>
	private string CheckResponseFile(string schemaFile, string requestFile)
	{
		if (!File.Exists(schemaFile))
			return $"Can't find the file {schemaFile}";
		else if (!File.Exists(requestFile))
			return $"Can't find the file {requestFile}";
		else
			return CompareReportResult(schemaFile, requestFile);
	}

	/// <summary>
	///		Compara el SQL de un archivo de resultado con la SQL generada por un archivo
	/// </summary>
	private string CompareReportResult(string schemaFile, string requestFile)
	{
		string error = string.Empty;

			// Compara la salida del informe paginado
			for (int page = 0; page < 3; page++)
			{
				string responseFile = Tools.FileHelper.GetResponseFile(requestFile, page);

					if (!File.Exists(responseFile))
					{
						// Sólo es un error para la página 0 (no paginado), el resto puede que no tengamos respuesta en tests
						if (page == 0)
							error += $"Can't find the response for {requestFile} page {page.ToString()}" + Environment.NewLine;
					}
					else
						try
						{
							string generatedSql = Tools.ReportHelper.GetSqlResponse(schemaFile, requestFile, page);

								// Compara la SQL de salida con el archivo
								if (!CompareSql(generatedSql, File.ReadAllText(responseFile)))
									error = $"The response for {requestFile} page {page.ToString()} has error";
						}
						catch (Exception exception)
						{
							error = $"Error when generate file {requestFile}. {exception.Message}";
						}
			}
			// Devuelve el error
			return error;
	}

	/// <summary>
	///		Compara las SQL
	/// </summary>
	private bool CompareSql(string generated, string sqlFile)
	{
		// Log
		System.Diagnostics.Debug.WriteLine("Compare generated " + new string('-', 80));
		System.Diagnostics.Debug.WriteLine(Normalize(generated));
		System.Diagnostics.Debug.WriteLine("Compare source " + new string('-', 80));
		System.Diagnostics.Debug.WriteLine(Normalize(sqlFile));
		System.Diagnostics.Debug.WriteLine(new string('-', 80));
		// Compara
		return Normalize(generated).Equals(Normalize(sqlFile));
	}

	/// <summary>
	///		Normaliza una cadena SQL
	/// </summary>
	private string Normalize(string sql)
	{
		// Quita saltos de línea y tabuladores
		sql = sql.Replace('\n', ' ');
		sql = sql.Replace('\r', ' ');
		sql = sql.Replace('\t', ' ');
		// Quita espacios dobles
		while (!string.IsNullOrWhiteSpace(sql) && sql.Contains("  "))
			sql = sql.Replace("  ", " ");
		// Quita los malos paréntesis, corchetes...
		sql = sql.Replace("( ", "(");
		sql = sql.Replace(" )", ")");
		sql = sql.Replace("[ ", "[");
		sql = sql.Replace(" ]", "]");
		// Quita espacios iniciales / finales
		if (!string.IsNullOrWhiteSpace(sql))
			sql = sql.Trim();
		// Devuelve la cadena normalizada
		return sql;
	}
}