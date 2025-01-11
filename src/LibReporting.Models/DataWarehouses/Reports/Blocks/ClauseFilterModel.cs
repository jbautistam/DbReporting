namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;

/// <summary>
///		Cláusula con filtros adicionales
/// </summary>
public class ClauseFilterModel
{
	/// <summary>
	///		Cadena SQL del filtro
	/// </summary>
	public string Sql { get; set; } = default!;
}
