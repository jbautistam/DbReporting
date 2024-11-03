namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;

/// <summary>
///		Campo que se deben incluir en las consultas de una dimensión
/// </summary>
public class ReportRequestDimensionField
{
	/// <summary>
	///		Campo que se debe solicitar
	/// </summary>
	public string Field { get; set; } = string.Empty;
}
