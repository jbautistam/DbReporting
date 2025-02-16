namespace Bau.Libraries.LibReporting.Requests.Models;

/// <summary>
///		Datos de solicitud de una columna
/// </summary>
public class ColumnRequestModel
{
	/// <summary>
	///		Modo de ordenación
	/// </summary>
	public enum SortOrder
	{
		/// <summary>No se define ningún orden</summary>
		Undefined,
		/// <summary>Orden ascendente</summary>
		Ascending,
		/// <summary>Orden descendente</summary>
		Descending
	}

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

	public ColumnRequestModel(string id)
	{
		Id = id;
	}

	/// <summary>
	///		Código de columna
	/// </summary>
	public string Id { get; }

	/// <summary>
	///		Indica si esta columna es visible en la consulta final o sólo para los filtros
	/// </summary>
	public bool Visible { get; set; } = true;

	/// <summary>
	///		Modo de agregación
	/// </summary>
	public AggregationType AggregatedBy { get; set; }

	/// <summary>
	///		Indice para la ordenación del campo
	/// </summary>
	public int OrderIndex { get; set; }

	/// <summary>
	///		Modo de ordenación
	/// </summary>
	public SortOrder OrderBy { get; set; }

	/// <summary>
	///		Filtro para la cláusula WHERE
	/// </summary>
	public List<FilterRequestModel> FiltersWhere { get; } = [];

	/// <summary>
	///		Filtro para la cláusula HAVING
	/// </summary>
	public List<FilterRequestModel> FiltersHaving { get; } = [];
}