namespace Bau.Libraries.LibReporting.Application;

/// <summary>
///		Manager para reporting
/// </summary>
public class ReportingManager
{
	/// <summary>
	///		Añade un origen de datos
	/// </summary>
	public void AddDataWarehouse(Models.DataWarehouses.DataWarehouseModel dataWarehouse)
	{
		Schema.DataWarehouses.Add(dataWarehouse);
	}

	/// <summary>
	///		Elimina un origen de datos
	/// </summary>
	public void RemoveDataWarehouse(Models.DataWarehouses.DataWarehouseModel dataWarehouse)
	{
		Schema.DataWarehouses.Remove(dataWarehouse);
	}

	/// <summary>
	///		Obtiene la SQL resultante de procesar una solicitud de informe
	/// </summary>
	public string GetSqlResponse(Requests.Models.ReportRequestModel request) 
	{
		Controllers.Request.Converson.RequestConversor conversor = new(this);

			// Genera la consulta
			return new Controllers.Queries.ReportQueryGenerator(conversor.Convert(request)).GetSql();
	}

	/// <summary>
	///		<see cref="Models.ReportingSchemaModel"/> con el que trabaja la aplicación
	/// </summary>
	public Models.ReportingSchemaModel Schema { get; } = new();
}