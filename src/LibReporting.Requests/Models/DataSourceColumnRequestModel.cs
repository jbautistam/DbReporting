namespace Bau.Libraries.LibReporting.Requests.Models;

/// <summary>
///		Clase con los datos de una columna de un origen de datos solicitada para un listado
/// </summary>
public class DataSourceColumnRequestModel : BaseColumnRequestModel
{
	/// <summary>
	///		Modo de agregación por esta columna
	/// </summary>
	public enum AggregationType
	{
		/// <summary>Sin agregación</summary>
		NoAggregated,
		/// <summary>Suma</summary>
		Sum,
		/// <summary>Valor máximo</summary>
		Max,
		/// <summary>Valor mínimo</summary>
		Min,
		/// <summary>Media</summary>
		Average,
		/// <summary>Desviación estándar</summary>
		StandardDeviation
	}

	/// <summary>
	///		Clona los datos
	/// </summary>
	public DataSourceColumnRequestModel Clone()
	{
		DataSourceColumnRequestModel cloned = new()
												{
													ColumnId = ColumnId,
													AggregatedBy = AggregatedBy
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

	/// <summary>
	///		Modo de agregación
	/// </summary>
	public AggregationType AggregatedBy { get; set; }
}