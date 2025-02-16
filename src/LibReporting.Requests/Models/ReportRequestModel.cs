namespace Bau.Libraries.LibReporting.Requests.Models;

/// <summary>
///		Clase con los datos de solicitud de informe
/// </summary>
public class ReportRequestModel
{
	public ReportRequestModel(string datawarehouseId, string reportId)
	{
		DataWarehouseId = datawarehouseId;
		ReportId = reportId;
	}

	/// <summary>
	///		Indica si se han solicitado totales (estamos en la primera página)
	/// </summary>
	public bool IsRequestedTotals() => Pagination.IsFirstPage;

	/// <summary>
	///		Código de almacén solicitado
	/// </summary>
	public string DataWarehouseId { get; }

	/// <summary>
	///		Código de informe solicitado
	/// </summary>
	public string ReportId { get; }

	/// <summary>
	///		Parámetros
	/// </summary>
	public List<ParameterRequestModel> Parameters { get; } = [];

	/// <summary>
	///		Dimensiones solicitadas
	/// </summary>
	public List<DataRequestModel> Dimensions { get; } = [];

	/// <summary>
	///		Solicitudes de orígenes de datos
	/// </summary>
	public List<DataRequestModel> DataSources { get; } = [];

	/// <summary>
	///		Expresiones solicitadas
	/// </summary>
	public List<ColumnRequestModel> Expressions { get; } = [];

	/// <summary>
	///		Paginación
	/// </summary>
	public PaginationRequestModel Pagination { get; } = new();
}