namespace Bau.Libraries.LibReporting.Repository.Xml;

/// <summary>
///		Controlador del repositorio de informes utilizando XML
/// </summary>
public class ReportingRepositoryXml
{
	/// <summary>
	///		Repositorio de <see cref="Models.DataWarehouses.DataWarehouseModel"/>
	/// </summary>
	public Repositories.DataWarehouseRepository DataWarehouseRepository { get; } = new();

	/// <summary>
	///		Repositorio de <see cref="Models.DataWarehouses.Reports.ReportModel"/>
	/// </summary>
	public Repositories.ReportRepository ReportRepository { get; } = new();

	/// <summary>
	///		Repositorio de <see cref="Models.DataWarehouses.Reports.Transformers.TransformRuleModel"/>
	/// </summary>
	public Repositories.TransformRuleRepository TransformRuleRepository { get; } = new();

	/// <summary>
	///		Repositorio de <see cref="Requests.Models.ReportRequestModel"/>
	/// </summary>
	public Repositories.RequestRepository RequestRepository { get; } = new();
}
