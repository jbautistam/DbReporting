using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;

namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Datos del parámetro solicitado en un informe
/// </summary>
internal class RequestParameterModel
{
	internal RequestParameterModel(ReportParameterModel parameter, object? value)
	{
		Parameter = parameter;
		Value = value;
	}

	/// <summary>
	///		Tipo de parámetro
	/// </summary>
	internal ReportParameterModel Parameter { get; }

	/// <summary>
	///		Valor solicitado en el parámetro
	/// </summary>
	internal object? Value { get; }
}
