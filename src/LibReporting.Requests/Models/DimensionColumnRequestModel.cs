namespace Bau.Libraries.LibReporting.Requests.Models;

/// <summary>
///		Colunma solicitada de una dimensión
/// </summary>
public class DimensionColumnRequestModel : BaseColumnRequestModel
{
	/// <summary>
	///		Clona los datos de una columna
	/// </summary>
	public DimensionColumnRequestModel Clone()
	{
		DimensionColumnRequestModel cloned = new()
												{
													ColumnId = ColumnId,
													RequestedByUser = RequestedByUser
												};

			// Clona los datos base
			CopyBase(cloned);
			// Devuelve los datos clonados
			return cloned;
	}

	/// <summary>
	///		Indica si se ha solicitado por el usuario o se ha añadido por el motor al ser una columna obligatoria
	/// </summary>
	public bool RequestedByUser { get; set; } = true;

	/// <summary>
	///		Código de columna
	/// </summary>
	public string ColumnId { get; set; } = string.Empty;
}
