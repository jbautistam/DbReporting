namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;

/// <summary>
///		Claúsula con los datos de un campo
/// </summary>
public class ClauseFieldModel
{
	// Variables privadas
	private string _alias = string.Empty;

	/// <summary>
	///		Nombre del campo
	/// </summary>
	public string Field { get; set; } = default!;

	/// <summary>
	///		Alias del campo
	/// </summary>
	public string Alias
	{
		get 
		{
			if (string.IsNullOrWhiteSpace(_alias))
				return Field;
			else
				return _alias; 
		}
		set { _alias = value; }
	}
}
