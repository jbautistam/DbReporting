using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Lista de <see cref="RequestParameterModel"/>
/// </summary>
internal class RequestParameterCollectionModel : List<RequestParameterModel>
{
	internal RequestParameterCollectionModel(RequestModel request)
	{
		Request = request;
	}

	/// <summary>
	///		Convierte la lista de parámetros
	/// </summary>
	internal void AddRange(List<ParameterRequestModel> parameters)
	{
		foreach (ParameterRequestModel parameter in parameters)
		{
			ReportParameterModel? reportParameter = Request.Report.Parameters[parameter.Key];

				if (reportParameter is not null)
					Add(new RequestParameterModel(reportParameter, parameter.Value));
		}
	}

	/// <summary>
	///		Datos de la solicitud
	/// </summary>
	internal RequestModel Request { get; }
}
