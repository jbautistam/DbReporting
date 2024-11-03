namespace Bau.Libraries.LibReporting.Requests.Models;

/// <summary>
///		Clase con los datos de una columna de un origen de datos solicitada para un listado
/// </summary>
public class DataSourceColumnRequestModel : BaseColumnRequestModel
{
	/// <summary>
	///		Clona los datos
	/// </summary>
	public DataSourceColumnRequestModel Clone()
	{
		DataSourceColumnRequestModel cloned = new()
												{
													ColumnId = ColumnId
												};

			// Clona los datos base
			CopyBase(cloned);
			// Devuelve el objeto clonado
			return cloned;
	}

	/// <summary>
	///		Código de columna solicitada
	/// </summary>
	public string ColumnId { get; set; } = string.Empty;
}