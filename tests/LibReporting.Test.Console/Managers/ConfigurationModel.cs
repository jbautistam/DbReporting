namespace LibReporting.Test.Console.Managers;

/// <summary>
///		Clase de configuración
/// </summary>
internal class ConfigurationModel
{
	/// <summary>
	///		Tipos de informes
	/// </summary>
	internal enum ReportType
	{
		Replenishment,
		Transfers,
		Capacities
	}
	// Registros
	internal record ReportConfiguration(ReportType Type, string RequestFile, string TemplateId);

	internal List<ReportConfiguration> Files = [
													new(ReportType.Replenishment,
														@"C:\OneDrivePersonal\OneDrive\Documentos\Reporting\NewReports\Requests\Replenishment.request.xml",
														"Replenishment"
													   ),
													new(ReportType.Transfers,
														@"C:\OneDrivePersonal\OneDrive\Documentos\Reporting\NewReports\Requests\Transfers.request.xml",
														"Transfers"
													   ),
													new(ReportType.Capacities,
														@"C:\OneDrivePersonal\OneDrive\Documentos\Reporting\NewReports\Requests\Capacities.request.xml",
														"Capacities"
													   )
											   ];

	internal ConfigurationModel(string schemaFile)
	{
		SchemaFile = schemaFile;
	}

	/// <summary>
	///		Obtiene la configuración del informe
	/// </summary>
	internal ReportConfiguration GetReportConfiguration(ReportType type)
	{
		ReportConfiguration? configuration = Files.FirstOrDefault(item => item.Type == type);

			if (configuration is null)
				throw new ArgumentException($"Can't find the report {type.ToString()}");
			else
				return configuration;
	}

	/// <summary>
	///		Archivo de esquema
	/// </summary>
	internal string SchemaFile { get; }
}
