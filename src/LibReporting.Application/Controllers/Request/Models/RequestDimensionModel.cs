using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;

namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Columna solicitada de una dimensión
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
	internal RequestColumnModel? GetRequestColumn(string id) => Columns.FirstOrDefault(item => item.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase));

	/// <summary>
	///		Código de dimensión
	/// </summary>
	internal BaseDimensionModel Dimension { get; }

	/// <summary>
	///		Columnas
	/// </summary>
	internal List<RequestColumnModel> Columns { get; } = [];
}
