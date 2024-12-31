namespace Bau.Libraries.LibReporting.Requests.Models;

/// <summary>
///		Clase con los datos de una columna solicitada para un listado
/// </summary>
public class ExpressionColumnRequestModel
{
	/// <summary>
	///		Clona los datos
	/// </summary>
	public ExpressionColumnRequestModel Clone()
	{
		ExpressionColumnRequestModel cloned = new()
												{
													ColumnId = ColumnId,
												};

			// Devuelve el objeto clonado
			return cloned;
	}

	/// <summary>
	///		Código de columna solicitada
	/// </summary>
	public string ColumnId { get; set; } = default!;
}