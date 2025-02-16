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
			RequestDataSourceModel dataSource = new(this, requestDataSource.Id);

				// Añade las columnas
				foreach (ColumnRequestModel column in requestDataSource.Columns)
					dataSource.Columns.Add(new RequestColumnModel(column));
				// Añade el origen de datos
				Add(dataSource);
		}
	}

	/// <summary>
	///		Solicitud a la que se asocian los orígenes de daots
	/// </summary>
	internal RequestModel Request { get; }
}