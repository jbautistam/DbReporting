using Bau.Libraries.LibReporting.Application.Controllers.Request.Models;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
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
			ConvertDimensions(converted, request.Dimensions);
			converted.DataSourceColumns.AddRange(Manager, request.DataSources);
			converted.Expressions.AddRange(request.Expressions);
			converted.Parameters.AddRange(Convert(converted.Report, request.Parameters));
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
	///		Convierte las dimensiones
	/// </summary>
	private void ConvertDimensions(RequestModel request, List<DimensionRequestModel> requestDimensions)
	{
		foreach (DimensionRequestModel requestDimension in requestDimensions)
			foreach (DimensionColumnRequestModel requestColumn in requestDimension.Columns)
			{
				RequestDimensionColumnModel dimensionColumn = request.AddDimension(requestDimension.DimensionId, requestColumn.ColumnId, true);

					// Asigna los datos de la columna solicitada
					AssignColumnRequestData(requestColumn, dimensionColumn);
			}
	}


	/// <summary>
	///		Convierte la lista de parámetros
	/// </summary>
	private List<RequestParameterModel> Convert(ReportModel report, List<ParameterRequestModel> parameters)
	{
		List<RequestParameterModel> converted = [];

			// Convierte los parámetros
			foreach (ParameterRequestModel parameter in parameters)
			{
				ReportParameterModel? reportParameter = report.Parameters[parameter.Key];

					if (reportParameter is not null)
						converted.Add(new RequestParameterModel(reportParameter, parameter.Value));
			}
			// Devuelve la lista convertida
			return converted;
	}

	/// <summary>
	///		Manager principal
	/// </summary>
	internal ReportingManager Manager { get; }
}