using Bau.Libraries.LibReporting.Application;
using Bau.Libraries.LibReporting.Models.DataWarehouses;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Requests.Models;

namespace LibReporting.Tests.Tools;

/// <summary>
///		Manager para las soluciones de reporting
/// </summary>
public class ReportingSolutionManager
{
	public ReportingSolutionManager()
	{
		Manager = new ReportingManager();
	}

	///// <summary>
	/////		Carga una solución
	///// </summary>
	//public void LoadSolution(string fileName)
	//{
	//	// Limpia los archivos de la solución
	//	ReportingSolution.Clear();
	//	// Carga los nuevos archivos de solución
	//	new Repositories.SolutionRepository(this).Load(fileName);
	//	// Carga los esquemas de la solución
	//	foreach (string file in ReportingSolution.Files)
	//		AddDataWarehouse(file);
	//}

	/// <summary>
	///		Añade un origen de datos
	/// </summary>
	public void AddDataWarehouse(string fileName)
	{
		DataWarehouseModel dataWarehouse = GetRepository().DataWarehouseRepository.Get(fileName, Manager.Schema);

			// Añade el datawarehouse al esquema
			if (dataWarehouse is not null)
				Manager.AddDataWarehouse(dataWarehouse);
	}

	///// <summary>
	/////		Recarga un informe
	///// </summary>
	//public void RefreshAdvancedReport(DataWarehouseModel dataWarehouse, string fileName)
	//{
	//	ReportModel report = GetRepository().ReportRepository.Get(fileName, dataWarehouse);

	//		if (report is not null)
	//			dataWarehouse.Reports.Add(report);
	//}

	///// <summary>
	/////		Elimina un origen de datos
	///// </summary>
	//public void RemoveDataWarehouse(DataWarehouseModel dataWarehouse)
	//{
	//	// Elimina el archivo de la lista
	//	for (int index = ReportingSolution.DataWarehousesFiles.Count - 1; index >= 0; index--)
	//	{
	//		(string dataWarehouseId, string file) = ReportingSolution.DataWarehousesFiles[index];

	//			// Elimina el archivo
	//			if (dataWarehouse.Id.Equals(dataWarehouseId, StringComparison.CurrentCultureIgnoreCase) && !string.IsNullOrWhiteSpace(file))
	//			{
	//				// Elimina el archivo
	//				for (int indexFile = ReportingSolution.Files.Count - 1; indexFile >= 0; indexFile--)
	//					if (ReportingSolution.Files[indexFile].Equals(file, StringComparison.CurrentCultureIgnoreCase))
	//						ReportingSolution.Files.RemoveAt(indexFile);
	//				// Elimina el origen de datos del diccionario
	//				ReportingSolution.DataWarehousesFiles.RemoveAt(index);
	//			}
	//	}
	//	// Elimina el origen de datos del esquema
	//	Manager.RemoveDataWarehouse(dataWarehouse);
	//}

	///// <summary>
	/////		Graba una solución
	///// </summary>
	//public void SaveSolution(string fileName)
	//{
	//	new Repositories.SolutionRepository(this).Save(fileName);
	//}

	///// <summary>
	/////		Carga los <see cref="DataWarehouseModel"/> de un archivo
	///// </summary>
	//public void LoadDataWarehouse(string fileName)
	//{
	//	DataWarehouseModel dataWarehouse = GetRepository().DataWarehouseRepository.Get(fileName, Manager.Schema);

	//		// Añade el dashboard al esquema
	//		if (dataWarehouse != null)
	//			Manager.Schema.DataWarehouses.Add(dataWarehouse);
	//}

	///// <summary>
	/////		Carga un <see cref="DataWarehouseModel"/> a partir de un archivo de esquema de base de datos
	///// </summary>
	//public DataWarehouseModel ConvertSchemaDbToDataWarehouse(string name, string fileName) => new Converters.SchemaConverter().Convert(Manager.Schema, name, fileName);

	///// <summary>
	/////		Combina un <see cref="DataWarehouseModel"/> con un archivo de esquema de base de datos
	///// </summary>
	//public void Merge(DataWarehouseModel source, string schemaFile)
	//{
	//	new Converters.SchemaConverter().Merge(source, schemaFile);
	//}

	///// <summary>
	/////		Graba los datos de un <see cref="Models.DataWarehouses.DataWarehouseModel"/> en un archivo
	///// </summary>
	//public void SaveDataWarehouse(DataWarehouseModel dataWarehouse)
	//{
	//	string fileName = ReportingSolution.GetFileName(dataWarehouse);

	//		// Graba el archivo
	//		if (string.IsNullOrWhiteSpace(fileName))
	//			throw new NotImplementedException($"Cant find file name for '{dataWarehouse.Name}'");
	//		else
	//			GetRepository().DataWarehouseRepository.Update(fileName, dataWarehouse);
	//}

	/// <summary>
	///		Graba los datos de un <see cref="DataWarehouseModel"/> en un archivo
	/// </summary>
	public void SaveDataWarehouse(DataWarehouseModel dataWarehouse, string fileName)
	{
		GetRepository().DataWarehouseRepository.Update(fileName, dataWarehouse);
	}

	/// <summary>
	///		Carga los datos de un <see cref="Requests.Models.ReportRequestModel"/> de un archivo
	/// </summary>
	public ReportRequestModel LoadRequest(string fileName) => GetRepository().RequestRepository.Get(fileName);

	/// <summary>
	///		Graba los datos de un <see cref="Requests.Models.ReportRequestModel"/> en un archivo
	/// </summary>
	public void SaveRequest(ReportRequestModel request, string fileName)
	{
		GetRepository().RequestRepository.Update(fileName, request);
	}

	/// <summary>
	///		Obtiene el repositorio
	/// </summary>
	private Bau.Libraries.LibReporting.Repository.Xml.ReportingRepositoryXml GetRepository() => new(); 

	/// <summary>
	///		Obtiene la SQL resultante de procesar una solicitud de informe
	/// </summary>
	public string GetSqlResponse(ReportRequestModel request) => Manager.GetSqlResponse(request);

	/// <summary>
	///		<see cref="ReportingManager"/> con el que trabaja la aplicación
	/// </summary>
	public ReportingManager Manager { get; }
}