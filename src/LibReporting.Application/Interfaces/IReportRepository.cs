namespace Bau.Libraries.LibReporting.Application.Interfaces;

/// <summary>
///		Interface para los repositorios de <see cref="Models.DataWarehouses.Reports.ReportModel"/>
/// </summary>
public interface IReportRepository
{
	/// <summary>
	///		Obtiene un <see cref="Models.DataWarehouses.Reports.ReportModel"/>
	/// </summary>
	Models.DataWarehouses.Reports.ReportModel Get(string id, Models.DataWarehouses.DataWarehouseModel dataWarehouse);

	/// <summary>
	///		Obtiene un <see cref="Models.DataWarehouses.Reports.ReportModel"/> (asíncrono)
	/// </summary>
	Task<Models.DataWarehouses.Reports.ReportModel> GetAsync(string id, Models.DataWarehouses.DataWarehouseModel dataWarehouse, CancellationToken cancellationToken);

	/// <summary>
	///		Graba un <see cref="Models.DataWarehouses.Reports.ReportModel"/>
	/// </summary>
	void Update(string id, Models.DataWarehouses.Reports.ReportModel request);

	/// <summary>
	///		Graba un <see cref="Models.DataWarehouses.Reports.ReportModel"/> (asíncrono)
	/// </summary>
	Task UpdateAsync(string id, Models.DataWarehouses.Reports.ReportModel request, CancellationToken cancellationToken);
}
