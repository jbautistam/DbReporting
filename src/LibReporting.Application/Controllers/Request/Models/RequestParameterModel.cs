using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;

namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Datos del parámetro solicitado en un informe
/// </summary>
public class RequestParameterModel
{
	public RequestParameterModel(ReportParameterModel parameter, object? value)
	{
		Parameter = parameter;
		Value = value;
	}

	/// <summary>
	///		Tipo de parámetro
	/// </summary>
	public ReportParameterModel Parameter { get; }

	/// <summary>
	///		Valor solicitado en el parámetro
	/// </summary>
	public object? Value { get; }
}
