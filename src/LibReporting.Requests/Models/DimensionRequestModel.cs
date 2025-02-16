namespace Bau.Libraries.LibReporting.Requests.Models;

/// <summary>
///		Colunma solicitada de una dimensión
/// </summary>
public class DimensionRequestModel
{
	/// <summary>
	///		Código de dimensión
	/// </summary>
	public string DimensionId { get; set; } = default!;

	/// <summary>
	///		Columnas
	/// </summary>
	public List<ColumnRequestModel> Columns { get; } = [];
}