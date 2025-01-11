using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Requests.Models;

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
	///		Asigna los datos de la columna solicitada
	/// </summary>
	internal void AssignColumnRequestData(BaseColumnRequestModel request, RequestColumnBaseModel target)
	{
		target.Visible = request.Visible;
		target.OrderIndex = request.OrderIndex;
		target.OrderBy = Convert(request.OrderBy);
		target.FiltersWhere.AddRange(request.FiltersWhere);
		target.FiltersHaving.AddRange(request.FiltersHaving);

		// Convierte la ordenación
		SortOrder Convert(BaseColumnRequestModel.SortOrder type)
		{
			return type switch
					{
						BaseColumnRequestModel.SortOrder.Ascending => SortOrder.Ascending,
						BaseColumnRequestModel.SortOrder.Descending => SortOrder.Descending,
						_ => SortOrder.Undefined,
					};
		}
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
	public RequestFilterCollectionModel FiltersWhere { get; } = [];

	/// <summary>
	///		Filtro para la cláusula HAVING
	/// </summary>
	public RequestFilterCollectionModel FiltersHaving { get; } = [];
}