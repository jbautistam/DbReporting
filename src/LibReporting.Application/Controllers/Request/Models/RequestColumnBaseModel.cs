using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Clase base para las columnas
/// </summary>
internal abstract class RequestColumnBaseModel
{
	/// <summary>
	///		Modo de ordenación
	/// </summary>
	internal enum SortOrder
	{
		/// <summary>No se define ningún orden</summary>
		Undefined,
		/// <summary>Orden ascendente</summary>
		Ascending,
		/// <summary>Orden descendente</summary>
		Descending
	}

	internal RequestColumnBaseModel(DataSourceColumnModel column)
	{
		Column = column;
	}

	/// <summary>
	///		Asigna los datos de la columna solicitada
	/// </summary>
	internal void AssignColumnRequestData(BaseColumnRequestModel request)
	{
		Visible = request.Visible;
		OrderIndex = request.OrderIndex;
		OrderBy = Convert(request.OrderBy);
		FiltersWhere.AddRange(request.FiltersWhere);
		FiltersHaving.AddRange(request.FiltersHaving);

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
	internal DataSourceColumnModel Column { get; }

	/// <summary>
	///		Indica si esta columna es visible en la consulta final o sólo para los filtros
	/// </summary>
	internal bool Visible { get; set; } = true;

	/// <summary>
	///		Indice para la ordenación del campo
	/// </summary>
	internal int OrderIndex { get; set; }

	/// <summary>
	///		Modo de ordenación
	/// </summary>
	internal SortOrder OrderBy { get; set; }

	/// <summary>
	///		Filtro para la cláusula WHERE
	/// </summary>
	internal RequestFilterCollectionModel FiltersWhere { get; } = [];

	/// <summary>
	///		Filtro para la cláusula HAVING
	/// </summary>
	internal RequestFilterCollectionModel FiltersHaving { get; } = [];
}