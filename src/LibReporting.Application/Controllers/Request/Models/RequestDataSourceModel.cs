namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Origen de datos solicitado
/// </summary>
internal class RequestDataSourceModel
{
	internal RequestDataSourceModel(RequestDataSourceCollectionModel parent, string id)
	{
		Parent = parent;
		Id = id;
		Columns = new RequestDataSourceColumnCollectionModel(this);
	}

	/// <summary>
	///		Colección padre
	/// </summary>
	internal RequestDataSourceCollectionModel Parent { get; }

	/// <summary>
	///		Id del origen de datos
	/// </summary>
	internal string Id { get; }

	/// <summary>
	///		Solicitudes de columnas para este origen de datos
	/// </summary>
	internal RequestDataSourceColumnCollectionModel Columns { get; }
}