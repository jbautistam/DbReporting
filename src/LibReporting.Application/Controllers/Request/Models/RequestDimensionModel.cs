using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;

namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Colunma solicitada de una dimensión
/// </summary>
public class RequestDimensionModel
{
	public RequestDimensionModel(BaseDimensionModel dimension)
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
	public BaseDimensionModel Dimension { get; }

	/// <summary>
	///		Columnas
	/// </summary>
	public List<RequestDimensionColumnModel> Columns { get; } = [];
}
