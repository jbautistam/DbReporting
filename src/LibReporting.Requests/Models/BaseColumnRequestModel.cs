namespace Bau.Libraries.LibReporting.Requests.Models;

/// <summary>
///		Clase base para las columnas
/// </summary>
public abstract class BaseColumnRequestModel
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
	///		Copia los datos base en un objeto clonado
	/// </summary>
	protected void CopyBase(BaseColumnRequestModel cloned)
	{
		// Copoia los datos básicos
		cloned.Visible = Visible;
		cloned.OrderIndex = OrderIndex;
		cloned.OrderBy = OrderBy;
		// Copia los filtros del WHERE
		foreach (FilterRequestModel filter in FiltersWhere)
			cloned.FiltersWhere.Add(filter.Clone());
		// Copia los filtros del HAVING
		foreach (FilterRequestModel filter in FiltersHaving)
			cloned.FiltersHaving.Add(filter.Clone());
	}

	/// <summary>
	///		Indica si esta columna es visible en la consulta final o sólo para los filtros
	/// </summary>
	public bool Visible { get; set; } = true;

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