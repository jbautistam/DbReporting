using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;

namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Colunma solicitada de una dimensión
/// </summary>
internal class RequestDimensionModel
{
	internal RequestDimensionModel(BaseDimensionModel dimension)
	{
		Dimension = dimension;
	}

	/// <summary>
	///		Obtiene la columna asociada a una dimensión
	/// </summary>
	internal RequestDimensionColumnModel? GetRequestColumn(string id) => Columns.FirstOrDefault(item => item.Column.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase));

	/// <summary>
	///		Código de dimensión
	/// </summary>
	internal BaseDimensionModel Dimension { get; }

	/// <summary>
	///		Columnas
	/// </summary>
	internal List<RequestDimensionColumnModel> Columns { get; } = [];
}
