using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;

namespace Bau.Libraries.LibReporting.Models.DataWarehouses;

/// <summary>
///		Diccionario de <see cref="DataWarehouseModel"/>
/// </summary>
public class DataWarehouseDictionaryModel : Base.BaseReportingDictionaryModel<DataWarehouseModel>
{
	/// <summary>
	///		Obtiene un origen de datos
	/// </summary>
	public BaseDataSourceModel? GetDataSource(string id)
	{
		// Busca el origen de datos
		foreach (DataWarehouseModel dataWarehouse in EnumerateValues())
		{
			BaseDataSourceModel? dataSource = dataWarehouse.DataSources[id];

				// Si se ha encontrado los datos, se devuelve
				if (dataSource is not null)
					return dataSource;
		}
		// Si ha llegado hasta aquí es porque no ha encontrado nada
		return null;
	}

	/// <summary>
	///		Obtiene una dimensión
	/// </summary>
	public BaseDimensionModel? GetDimension(string id)
	{
		// Busca la dimensión
		foreach (DataWarehouseModel dataWarehouse in EnumerateValues())
		{
			BaseDimensionModel? dimension = dataWarehouse.Dimensions[id];

				// Si se ha encontrado los datos, se devuelve
				if (dimension is not null)
					return dimension;
		}
		// Si ha llegado hasta aquí es porque no ha encontrado nada
		return null;
	}

	/// <summary>
	///		Obtiene un informe
	/// </summary>
	public ReportModel? GetReport(string id)
	{
		// Busca el informe
		foreach (DataWarehouseModel dataWarehouse in EnumerateValues())
		{
			ReportModel? report = dataWarehouse.Reports[id];

				// Si se ha encontrado los datos, se devuelve
				if (report is not null)
					return report;
		}
		// Si ha llegado hasta aquí es porque no ha encontrado nada
		return null;
	}
}
