using Bau.Libraries.LibReporting.Application;
using Bau.Libraries.LibReporting.Requests.Models;

namespace LibReporting.Test.Console.Managers;

/// <summary>
///		Manager de pruebas
/// </summary>
internal class SqlTestManager
{
	internal SqlTestManager(ConfigurationModel configuration)
	{
		Configuration = configuration;
	}

	/// <summary>
	///		Obtiene la cadena SQL de la solicitud
	/// </summary>
	internal string GetSqlFile(ConfigurationModel.ReportType type, int page = 0)
	{
		ReportingManager manager = new();
		Bau.Libraries.LibReporting.Repository.Xml.ReportingRepositoryXml repository = new();
		ReportRequestModel? request = repository.RequestRepository.Get(Configuration.GetReportConfiguration(type).RequestFile);

			if (request is null)
				throw new Exception($"Can't load the request for report: {type}");
			else
			{
				// Agrega el dataWarehouse
				manager.AddDataWarehouse(repository.DataWarehouseRepository.Get(Configuration.SchemaFile, manager.Schema));
				// Cambia la paginación de la solicitud
				request.Pagination.MustPaginate = page > 0;
				request.Pagination.Page = page;
				request.Pagination.RecordsPerPage = 100;
				// Graba el archivo
				return manager.GetSqlResponse(request);
			}
	}

	/// <summary>
	///		Configuración
	/// </summary>
	internal ConfigurationModel Configuration { get; }
}
