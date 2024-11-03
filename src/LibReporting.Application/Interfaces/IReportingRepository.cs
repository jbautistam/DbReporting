namespace Bau.Libraries.LibReporting.Application.Interfaces;

/// <summary>
///		Interface para los repositorios de reporting
/// </summary>
public interface IReportingRepository
{
	/// <summary>
	///		Repositorio de esquema de almacén de datos
	/// </summary>
	ISchemaRepository DataWarehouseRepository { get; }

	/// <summary>
	///		Repositorio de informes
	/// </summary>
	IReportRepository ReportRepository { get; }

	/// <summary>
	///		Repositorio de solicitudes
	/// </summary>
	IRequestRepository RequestRepository { get; }
}
