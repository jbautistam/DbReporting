using Bau.Libraries.LibReporting.Application;
using Bau.Libraries.LibReporting.Models.DataWarehouses;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Requests.Models;

namespace LibReporting.Tests.Generator.Managers;

/// <summary>
///		Generador de archivos de solicitud y respuesta de informes
/// </summary>
internal class FilesTestGenerator
{
	/// <summary>
	///		Genera los archivos
	/// </summary>
	internal void Generate(string schemaFile, string outputFolder)
	{
		// Carga el archivo de esquema
		LoadSchema(schemaFile);
		// Genera los archivos de solicitud / respuesta 
		foreach (DataWarehouseModel dataWarehouse in Manager.Schema.DataWarehouses.EnumerateValues())
			foreach (ReportModel report in dataWarehouse.Reports.EnumerateValues())
				GenerateFiles(report, Path.Combine(outputFolder, report.Id));
	}

	/// <summary>
	///		Genera los archivos
	/// </summary>
	private void GenerateFiles(ReportModel report, string folder)
	{
		List<ReportRequestModel> requests = GenerateRequests(report);

			// Crea el directorio
			Directory.CreateDirectory(folder);
			// Graba las solicitudes
			foreach (ReportRequestModel request in requests)
			{
				string sql = Manager.GetSqlResponse(request);

					// Graba la solicitud
					SaveRequest(request, sql, folder, requests.IndexOf(request).ToString());
			}
	}

	/// <summary>
	///		Genera las solicitudes de un informe
	/// </summary>
	private List<ReportRequestModel> GenerateRequests(ReportModel report)
	{
		List<ReportRequestModel> requests = [];

			// Genera las solicitudes
			for (int index = 0; index < 3; index++)
				requests.AddRange(GeneratePagedRequests(report, index));
			// Devuelve las solicitudes
			return requests;
	}

	/// <summary>
	///		Genera las solicitudes paginadas
	/// </summary>
	private List<ReportRequestModel> GeneratePagedRequests(ReportModel report, int page)
	{
		List<ReportRequestModel> requests = [];
		ReportRequestModel request = new(report.DataWarehouse.Id, report.Id);

			// Asigna la paginación
			request.Pagination.MustPaginate = page > 0;
			if (page > 0)
				request.Pagination.Page = page;
			// Añade la solicitud a la lista
			requests.Add(request);
			// Devuelve las solicitudes
			return requests;
	}

	/// <summary>
	///		Graba las solicitudes y respuestas
	/// </summary>
	private void SaveRequest(ReportRequestModel request, string response, string folder, string filePrefix)
	{
		string fileName = Path.Combine(folder, request.ReportId + $"_{filePrefix}.request.xml");

			// Graba el archivo de solicitud
			new Bau.Libraries.LibReporting.Repository.Xml.Repositories.RequestRepository(new Bau.Libraries.LibReporting.Repository.Xml.ReportingRepositoryXml())
					.Update(fileName, request);
			// Graba el archivo SQL
			fileName = Path.Combine(folder, request.ReportId + $"_{filePrefix}.response.sql");
			File.WriteAllText(fileName, response);
	}

	/// <summary>
	///		Carga el esquema de informes
	/// </summary>
	private void LoadSchema(string fileName)
	{
		DataWarehouseModel dataWarehouse = new Bau.Libraries.LibReporting.Repository.Xml.ReportingRepositoryXml().DataWarehouseRepository.Get(fileName, Manager.Schema);

			// Añade el datawarehouse del esquema
			if (dataWarehouse is not null)
				Manager.AddDataWarehouse(dataWarehouse);
	}

	/// <summary>
	///		Manager de reporting
	/// </summary>
	private ReportingManager Manager { get; } = new();
}