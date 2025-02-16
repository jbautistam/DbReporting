using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;

namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Modelo interno con los datos de la solicitud de un usuario
/// </summary>
internal class RequestModel
{
	internal RequestModel(ReportingManager manager, ReportModel report)
	{
		// Inicializa el objeto
		Manager = manager;
		Report = report;
		// Inicializa las colecciones
		DataSources = new RequestDataSourceCollectionModel(this);
		Dimensions = new RequestDimensionCollectionModel(this);
		Parameters = new RequestParameterCollectionModel(this);
	}

	/// <summary>
	///		Manager principal
	/// </summary>
	internal ReportingManager Manager { get; }

	/// <summary>
	///		Informe sobre el que se hace la solicitud
	/// </summary>
	internal ReportModel Report { get; }

	/// <summary>
	///		Parámetros
	/// </summary>
	internal RequestParameterCollectionModel Parameters { get; }

	/// <summary>
	///		Dimensiones solicitadas
	/// </summary>
	internal RequestDimensionCollectionModel Dimensions { get; }

	/// <summary>
	///		Expresiones solicitadas
	/// </summary>
	internal RequestColumnCollectionModel Expressions { get; } = [];

	/// <summary>
	///		Solicitudes de orígenes de datos
	/// </summary>
	public RequestDataSourceCollectionModel DataSources { get; }

	/// <summary>
	///		Paginación
	/// </summary>
	public RequestPaginationModel Pagination { get; } = new();
}
