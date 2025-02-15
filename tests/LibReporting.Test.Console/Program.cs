using Bau.Libraries.LibReporting.Application;
using Bau.Libraries.LibReporting.Requests.Models;

string schemaFile = @"C:\OneDrivePersonal\OneDrive\Documentos\Reporting\NewReports\Sql Server - Reporting - Nuñez de Arenas.reporting.xml";
// string requestFile = @"C:\OneDrivePersonal\OneDrive\Documentos\Reporting\NewReports\Requests\DailyStocks.request.xml";
// string requestFile = @"C:\OneDrivePersonal\OneDrive\Documentos\Reporting\NewReports\Requests\Curves.request.xml";
string requestFile = @"C:\OneDrivePersonal\OneDrive\Documentos\Reporting\NewReports\Requests\DatabaseAlert.request.xml";

string sql = GetSqlResponse(schemaFile, requestFile);

Console.WriteLine(sql);

/// <summary>
///		Obtiene la SQL de respuesta de un archivo
/// </summary>
string GetSqlResponse(string schemaFile, string requestFile, int page = 0)
{
	ReportingManager manager = new();
	Bau.Libraries.LibReporting.Repository.Xml.ReportingRepositoryXml repository = new();
	ReportRequestModel? request = repository.RequestRepository.Get(requestFile);

		if (request is null)
			throw new Exception($"Can't load the request: {requestFile}");
		else
		{
			// Agrega el dataWarehouse
			manager.AddDataWarehouse(repository.DataWarehouseRepository.Get(schemaFile, manager.Schema));
			// Cambia la paginación de la solicitud
			request.Pagination.MustPaginate = page > 0;
			request.Pagination.Page = page;
			request.Pagination.RecordsPerPage = 100;
			// Graba el archivo
			return manager.GetSqlResponse(request);
		}
}
