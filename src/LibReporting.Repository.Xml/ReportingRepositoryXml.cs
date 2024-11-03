using Bau.Libraries.LibReporting.Application.Interfaces;

namespace Bau.Libraries.LibReporting.Repository.Xml;

/// <summary>
///		Controlador del repositorio de informes utilizando XML
/// </summary>
public class ReportingRepositoryXml : IReportingRepository
{
	public ReportingRepositoryXml()
	{
		DataWarehouseRepository = new Repositories.DataWarehouseRepository(this);
		ReportRepository = new Repositories.ReportRepository(this);
		RequestRepository = new Repositories.RequestRepository(this);
	}

	/// <summary>
	///		Repositorio de <see cref="Models.DataWarehouses.DataWarehouseModel"/>
	/// </summary>
	public ISchemaRepository DataWarehouseRepository { get; }

	/// <summary>
	///		Repositorio de <see cref="Models.DataWarehouses.Reports.ReportModel"/>
	/// </summary>
	public IReportRepository ReportRepository { get; }

	/// <summary>
	///		Repositorio de <see cref="Requests.Models.ReportRequestModel"/>
	/// </summary>
	public IRequestRepository RequestRepository { get; }
}
