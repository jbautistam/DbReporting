namespace Bau.Libraries.LibReporting.Application.Interfaces;

/// <summary>
///		Interface para los repositorios de <see cref="Models.ReportingSchemaModel"/>
/// </summary>
public interface ISchemaRepository
{
	/// <summary>
	///		Obtiene un <see cref="Models.DataWarehouses.DataWarehouseModel"/>
	/// </summary>
	Models.DataWarehouses.DataWarehouseModel Get(string id, Models.ReportingSchemaModel schema);

	/// <summary>
	///		Obtiene un <see cref="Models.DataWarehouses.DataWarehouseModel"/> (asíncrono)
	/// </summary>
	Task<Models.DataWarehouses.DataWarehouseModel> GetAsync(string id, Models.ReportingSchemaModel schema, CancellationToken cancellationToken);

	/// <summary>
	///		Graba un <see cref="Models.ReportingSchemaModel"/>
	/// </summary>
	void Update(string id, Models.DataWarehouses.DataWarehouseModel dataWarehouse);

	/// <summary>
	///		Graba un <see cref="Models.ReportingSchemaModel"/> (asíncrono)
	/// </summary>
	Task UpdateAsync(string id, Models.DataWarehouses.DataWarehouseModel dataWarehouse, CancellationToken cancellationToken);
}
