using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Clase con los datos de una columna solicitada para un listado
/// </summary>
internal class RequestDataSourceColumnModel : RequestColumnBaseModel
{
	internal RequestDataSourceColumnModel(DataSourceColumnModel column, AggregationType aggregatedBy) : base(column) 
	{
		AggregatedBy = aggregatedBy;
	}

	/// <summary>
	///		Modo de agregación
	/// </summary>
	internal AggregationType AggregatedBy { get; }
}