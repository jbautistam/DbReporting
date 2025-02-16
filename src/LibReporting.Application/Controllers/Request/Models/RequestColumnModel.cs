using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Clase base para las columnas
/// </summary>
internal class RequestColumnModel
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

	/// <summary>
	///		Modo de agregación por esta columna
	/// </summary>
	internal enum AggregationType
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

	internal RequestColumnModel(ColumnRequestModel request)
	{
		Id = request.Id;
		Visible = request.Visible;
		AggregatedBy = ConvertAggregate(request.AggregatedBy);
		OrderIndex = request.OrderIndex;
		OrderBy = ConvertSort(request.OrderBy);
		FiltersWhere.AddRange(request.FiltersWhere);
		FiltersHaving.AddRange(request.FiltersHaving);

		// Convierte la ordenación
		SortOrder ConvertSort(ColumnRequestModel.SortOrder type)
		{
			return type switch
					{
						ColumnRequestModel.SortOrder.Ascending => SortOrder.Ascending,
						ColumnRequestModel.SortOrder.Descending => SortOrder.Descending,
						_ => SortOrder.Undefined,
					};
		}

		// Convierte el tipo de agregación
		AggregationType ConvertAggregate(ColumnRequestModel.AggregationType type)
		{
			return type switch
					{
						ColumnRequestModel.AggregationType.Sum => AggregationType.Sum,
						ColumnRequestModel.AggregationType.Max => AggregationType.Max,
						ColumnRequestModel.AggregationType.Min => AggregationType.Min,
						ColumnRequestModel.AggregationType.Average => AggregationType.Average,
						ColumnRequestModel.AggregationType.StandardDeviation => AggregationType.StandardDeviation,
						_ => AggregationType.NoAggregated
					};
		}
	}

	/// <summary>
	///		Código de columna
	/// </summary>
	internal string Id { get; }

	/// <summary>
	///		Indica si esta columna es visible en la consulta final o sólo para los filtros
	/// </summary>
	internal bool Visible { get; set; } = true;

	/// <summary>
	///		Tipo de agregación
	/// </summary>
	internal AggregationType AggregatedBy { get; set; }

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