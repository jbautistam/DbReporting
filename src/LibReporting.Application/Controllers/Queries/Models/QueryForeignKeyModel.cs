using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

/// <summary>
///		Campo de una dimensión
/// </summary>
internal class QueryForeignKeyFieldModel
{
	internal QueryForeignKeyFieldModel(BaseDimensionModel dimension, string columnDimension, string columnRelated)
	{
		Dimension = dimension;
		ColumnDimension = columnDimension;
		ColumnRelated = columnRelated;
	}

	/// <summary>
	///		Datos de la dimensión
	/// </summary>
	internal BaseDimensionModel Dimension { get; }

	/// <summary>
	///		Nombre del campo en la dimensión
	/// </summary>
	internal string ColumnDimension { get; }

	/// <summary>
	///		Nombre del campo relacionado 
	/// </summary>
	internal string ColumnRelated { get; }
}
