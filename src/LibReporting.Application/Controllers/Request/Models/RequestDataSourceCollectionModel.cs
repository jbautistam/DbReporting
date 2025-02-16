using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Lista de <see cref="RequestDataSourceModel"/>
/// </summary>
internal class RequestDataSourceCollectionModel : List<RequestDataSourceModel>
{
	internal RequestDataSourceCollectionModel(RequestModel request)
	{
		Request = request;
	}

	/// <summary>
	///		Añade los datos solicitados
	/// </summary>
	internal void AddRange(List<DataRequestModel> requestDataSources)
	{
		foreach (DataRequestModel requestDataSource in requestDataSources)
		{
			RequestDataSourceModel? dataSource = Get(requestDataSource.Id);

				// Si no se había añadido ya este origen de datos			
				if (dataSource is null)
				{
					dataSource = new RequestDataSourceModel(this, requestDataSource.Id);
					Add(dataSource);
				}
				// Añade las columnas
				dataSource.Columns.AddRange(requestDataSource.Columns);
		}
	}

	/// <summary>
	///		Obtiene un origen de datos por su Id
	/// </summary>
	internal RequestDataSourceModel? Get(string id) => this.FirstOrDefault(item => item.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase));

	/// <summary>
	///		Solicitud a la que se asocian los orígenes de datos
	/// </summary>
	internal RequestModel Request { get; }
}