using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Clase base para las columnas
/// </summary>
public abstract class RequestColumnBaseModel
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

	public RequestColumnBaseModel(DataSourceColumnModel column)
	{
		Column = column;
	}

	/// <summary>
	///		Columna solicitada
	/// </summary>
	public DataSourceColumnModel Column { get; }

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
	public List<RequestFilterModel> FiltersWhere { get; } = [];

	/// <summary>
	///		Filtro para la cláusula HAVING
	/// </summary>
	public List<RequestFilterModel> FiltersHaving { get; } = [];
}