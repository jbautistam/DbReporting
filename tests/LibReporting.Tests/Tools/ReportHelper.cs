using Bau.Libraries.LibReporting.Requests.Models;

namespace LibReporting.Tests.Tools;

/// <summary>
///		Clase de ayuda para informes
/// </summary>
internal static class ReportHelper
{
	/// <summary>
	///		Obtiene la SQL de respuesta de un archivo
	/// </summary>
	internal static string GetSqlResponse(string schemaFile, string requestFile, int page = 0)
	{
		ReportingSolutionManager manager = new();
		ReportRequestModel request = manager.LoadRequest(requestFile);

			if (request is null)
				throw new Exception($"Can't load the request: {requestFile}");
			else
			{
				// Agrega el dataWarehouse
				manager.AddDataWarehouse(FileHelper.GetFullFileName(schemaFile));
				// Cambia la paginación de la solicitud
				request.Pagination.MustPaginate = page > 0;
				request.Pagination.Page = page;
				request.Pagination.RecordsPerPage = 100;
				// Graba el archivo
				return manager.GetSqlResponse(request);
			}
	}
}