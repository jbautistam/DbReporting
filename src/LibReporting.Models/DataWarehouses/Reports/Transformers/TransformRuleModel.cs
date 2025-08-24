namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Transformers;

/// <summary>
///		Reglas de transformación de una cadena SQL
/// </summary>
public class TransformRuleModel
{
	/// <summary>
	///		Origen de la transformación
	/// </summary>
	public required string Source { get; init; }

	/// <summary>
	///		Destino de la transformación
	/// </summary>
	public required string Target { get; init; }
}