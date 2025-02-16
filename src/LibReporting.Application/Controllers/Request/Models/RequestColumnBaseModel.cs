using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Clase base para las columnas
/// </summary>
internal abstract class RequestColumnBaseModel
{
	internal RequestColumnBaseModel(DataSourceColumnModel column)
	{
		Column = column;
	}

	/// <summary>
	///		Asigna los datos de la columna solicitada
	/// </summary>
	internal void AssignColumnRequestData(ColumnRequestModel request)
	{
		Visible = request.Visible;
		OrderIndex = request.OrderIndex;
		OrderBy = Convert(request.OrderBy);
		FiltersWhere.AddRange(request.FiltersWhere);
		FiltersHaving.AddRange(request.FiltersHaving);

		// Convierte la ordenación
		RequestColumnModel.SortOrder Convert(ColumnRequestModel.SortOrder type)
		{
			return type switch
					{
						ColumnRequestModel.SortOrder.Ascending => RequestColumnModel.SortOrder.Ascending,
						ColumnRequestModel.SortOrder.Descending => RequestColumnModel.SortOrder.Descending,
						_ => RequestColumnModel.SortOrder.Undefined,
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
	internal RequestColumnModel.SortOrder OrderBy { get; set; }

	/// <summary>
	///		Filtro para la cláusula WHERE
	/// </summary>
	internal RequestFilterCollectionModel FiltersWhere { get; } = [];

	/// <summary>
	///		Filtro para la cláusula HAVING
	/// </summary>
	internal RequestFilterCollectionModel FiltersHaving { get; } = [];
}