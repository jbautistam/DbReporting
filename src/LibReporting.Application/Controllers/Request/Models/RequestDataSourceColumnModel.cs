using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Clase con los datos de una columna solicitada para un listado
/// </summary>
public class RequestDataSourceColumnModel : RequestColumnBaseModel
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
	
	public RequestDataSourceColumnModel(DataSourceColumnModel column, AggregationType aggregatedBy) : base(column) 
	{
		AggregatedBy = aggregatedBy;
	}

	/// <summary>
	///		Modo de agregación
	/// </summary>
	public AggregationType AggregatedBy { get; }
}