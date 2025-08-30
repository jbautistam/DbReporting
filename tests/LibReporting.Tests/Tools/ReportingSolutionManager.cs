using Bau.Libraries.LibReporting.Application;
using Bau.Libraries.LibReporting.Models.DataWarehouses;
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

	/// <summary>
	///		Añade un origen de datos
	/// </summary>
	public void AddDataWarehouse(string fileName)
	{
		DataWarehouseModel dataWarehouse = GetRepository().DataWarehouseRepository.Load(fileName, Manager.Schema);

			// Añade el datawarehouse al esquema
			if (dataWarehouse is not null)
				Manager.AddDataWarehouse(dataWarehouse);
	}

	/// <summary>
	///		Graba los datos de un <see cref="DataWarehouseModel"/> en un archivo
	/// </summary>
	public void SaveDataWarehouse(DataWarehouseModel dataWarehouse, string fileName)
	{
		GetRepository().DataWarehouseRepository.Save(fileName, dataWarehouse);
	}

	/// <summary>
	///		Carga los datos de un <see cref="Requests.Models.ReportRequestModel"/> de un archivo
	/// </summary>
	public ReportRequestModel? LoadRequest(string fileName) => GetRepository().RequestRepository.Load(fileName);

	/// <summary>
	///		Graba los datos de un <see cref="Requests.Models.ReportRequestModel"/> en un archivo
	/// </summary>
	public void SaveRequest(ReportRequestModel request, string fileName)
	{
		GetRepository().RequestRepository.Save(fileName, request);
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