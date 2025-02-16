namespace Bau.Libraries.LibReporting.Requests.Models;

/// <summary>
///		Clase con los datos solicitados para un listado
/// </summary>
public class DataRequestModel
{
	public DataRequestModel(string id)
	{
		Id = id;
	}

	/// <summary>
	///		Clave del tipo solicititado
	/// </summary>
	public string Id { get; }

	/// <summary>
	///		Columnas solicitadas
	/// </summary>
	public List<ColumnRequestModel> Columns { get; } = [];
}