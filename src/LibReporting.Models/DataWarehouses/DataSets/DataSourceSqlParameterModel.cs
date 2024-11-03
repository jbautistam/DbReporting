namespace Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

/// <summary>
///		Parámetro de un origen de datos SQL
/// </summary>
public class DataSourceSqlParameterModel
{
	/// <summary>
	///		Nombre del parámetro
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	///		Tipo del parámetro
	/// </summary>
	public DataSourceColumnModel.FieldType Type { get; set; }

	/// <summary>
	///		Valor predeterminado
	/// </summary>
	public string DefaultValue { get; set; } = string.Empty;
}