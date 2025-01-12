using Bau.Libraries.LibReporting.Application.Controllers.Request.Models;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Conversors;

/// <summary>
///		Coversor de <see cref="ReportRequestModel"/> en <see cref="ReportRequestModel"/>
/// </summary>
internal class RequestConversor
{
	internal RequestConversor(ReportingManager manager)
	{
		Manager = manager;
	}

	/// <summary>
	///		Convierte la solicitud del usuario en el modelo utilizado por el sistema
	/// </summary>
	internal RequestModel Convert(ReportRequestModel request)
	{
		RequestModel converted = new(Manager, GetReport(request.DataWarehouseId, request.ReportId));

			// Convierte los datos
			converted.Dimensions.AddRange(request.Dimensions);
			converted.DataSourceColumns.AddRange(request.DataSources);
			converted.Expressions.AddRange(request.Expressions);
			converted.Parameters.AddRange(request.Parameters);
			// Asigna la paginación
			converted.Pagination.MustPaginate = request.Pagination.MustPaginate;
			converted.Pagination.Page = request.Pagination.Page;
			converted.Pagination.RecordsPerPage = request.Pagination.RecordsPerPage;
			// Devuelve los datos convertidos
			return converted;
	}

	/// <summary>
	///		Obtiene el informe solicitado
	/// </summary>
	private ReportModel GetReport(string dataWarehouseId, string reportId)
	{
		LibReporting.Models.DataWarehouses.DataWarehouseModel? dataWarehouse = Manager.Schema.DataWarehouses[dataWarehouseId];

			if (dataWarehouse is null)
				throw new Exceptions.ReportingParserException($"Can't find the datawarehouse {dataWarehouseId} for report {reportId}");
			else
			{
				ReportModel? report = Manager.Schema.DataWarehouses.GetReport(reportId);

					if (report is null)
						throw new Exceptions.ReportingParserException($"Can't find the report {reportId}");
					else
						return report;
			}
	}

	/// <summary>
	///		Manager principal
	/// </summary>
	internal ReportingManager Manager { get; }
}