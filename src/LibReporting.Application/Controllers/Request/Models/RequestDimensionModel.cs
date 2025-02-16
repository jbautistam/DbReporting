using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;

namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Solicitud para una dimensión
/// </summary>
internal class RequestDimensionModel
{
	internal RequestDimensionModel(BaseDimensionModel dimension)
	{
		Dimension = dimension;
	}

	/// <summary>
	///		Datos de dimensión
	/// </summary>
	internal BaseDimensionModel Dimension { get; }

	/// <summary>
	///		Columnas
	/// </summary>
	internal RequestColumnCollectionModel Columns { get; } = [];
}
