namespace Bau.Libraries.LibReporting.Models;

/// <summary>
///		Clase con los datos del esquema de informes de BI
/// </summary>
public class ReportingSchemaModel
{
	/// <summary>
	///		Inicializa el esquema
	/// </summary>
	public void Initialize()
	{
		foreach (DataWarehouses.DataWarehouseModel dataWarehouse in DataWarehouses.EnumerateValues())
			dataWarehouse.Initialize();
	}

	/// <summary>
	///		Limpia el esquema
	/// </summary>
	public void Clear()
	{
		DataWarehouses.Clear();
	}

	/// <summary>
	///		Almacenes de datos
	/// </summary>
	public DataWarehouses.DataWarehouseDictionaryModel DataWarehouses { get; } = new();
}
