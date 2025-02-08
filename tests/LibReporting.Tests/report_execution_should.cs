using FluentAssertions;
using LibReporting.Tests.Tools;
using Bau.Libraries.LibReporting.Requests.Models;
using System.Data.SqlClient;

namespace LibReporting.Tests;

/// <summary>
///		Pruebas de ejecución de SQL de informes avanzados
/// </summary>
public class report_execution_should
{
	/// <summary>
	///		Comprueba si se puede cargar un esquema de base de datos y sus informes y ejecutar la cadena SQL contra la base de datos
	/// </summary>
	[Fact(Skip = "Todavía no")]
	public void execute_files_to_sql()
	{
		Dictionary<string, List<string>> reports = FileHelper.GetReports();
		string error = string.Empty;

			// Recorre los esquemas e informes procesando las solicitudes / respuestas
			foreach (KeyValuePair<string, List<string>> report in reports)
				foreach (string reportFile in report.Value)
				{
					string pathRequest = Path.Combine(Path.GetDirectoryName(reportFile) ?? string.Empty, 
													  reportFile.Substring(0, reportFile.Length - ".report.xml".Length));

						if (!Directory.Exists(pathRequest))
							error += $"Can't find request for report '{Path.GetFileName(reportFile)}'";
						else
							foreach (string requestFile in Directory.GetFiles(pathRequest, "*.request.xml"))
							{
								string executionError = ExecuteSql(report.Key, requestFile);

									if (!string.IsNullOrWhiteSpace(executionError))
										error += executionError + Environment.NewLine;
							}
				}
			// Comprueba los errores
			error.Should().BeNullOrWhiteSpace();
	}

	/// <summary>
	///		Comprueba si se puede cargar un esquema de base de datos y sus informes y ejecutar la cadena SQL contra la base de datos
	///	(el método execute_files_to_sql lo hace para todos los archivos, este es sólo por si queremos ejecutar uno en concreto)
	/// </summary>
	[Theory(Skip = "Todavía no")]
	[InlineData("Sales/Test-Sales.Reporting.xml", 
				"Sales/ManualRequests/Sales/Sales.request.xml")]
	public void execute_to_sql(string fileName, string fileRequest)
	{
		ExecuteSql(FileHelper.GetFullFileName(fileName), FileHelper.GetFullFileName(fileRequest)).Should().BeNullOrWhiteSpace();
	}

	/// <summary>
	///		Ejecuta la SQL generada por una solicitud sobre la base de datos
	/// </summary>
	private string ExecuteSql(string schemaFile, string requestFile)
	{
		string error = string.Empty;

			// Ejecuta la cadena SQL generada sobre la base de datos
			try
			{
				ReportingSolutionManager manager = new();
				ReportRequestModel request = manager.LoadRequest(requestFile);

					// Cambia la paginación
					request.Pagination.MustPaginate = true;
					request.Pagination.Page = 1;
					request.Pagination.RecordsPerPage = 20_000;
					// Agrega el dataWarehouse
					manager.AddDataWarehouse(schemaFile);
					// Comprueba que realmente se haya cargado una solicitud
					request.Should().NotBeNull();
					// Obtiene la SQL del informe
					using (SqlConnection connection = new(Tools.ConnectionsHelper.GetConnectionStringForSchema(schemaFile)))
					{
						SqlCommand command = connection.CreateCommand();

							// Añade los argumentos al comando
							foreach (ParameterRequestModel parameter in request.Parameters)
							{
								SqlParameter sqlParameter = command.CreateParameter();
								string key = parameter.Key;

									// Normaliza el parámetro
									if (!key.StartsWith("@"))
										key = $"@{key}";
									// Asigna el parámetro
									sqlParameter.ParameterName = key;
									sqlParameter.Value = parameter.Value;
									// Añade el parámetro a la colección
									command.Parameters.Add(sqlParameter);
							}
							// Asigna las cadena al comando
							command.CommandTimeout = (int) TimeSpan.FromMinutes(2).TotalSeconds;
							command.CommandText = manager.GetSqlResponse(request);
							command.CommandType = System.Data.CommandType.Text;
							// Abre la conexión
							connection.Open();
							// Ejecuta la consulta SQL
							command.ExecuteReader();
							// Cierra la conexión
							connection.Close();
					}
			}
			catch (Exception exception)
			{
				error = $"Error when execute {requestFile} for {Path.GetFileName(schemaFile)}. {Environment.NewLine} {exception.Message}";
			}
			// Devuelve la cadena de error
			return error;
	}
}