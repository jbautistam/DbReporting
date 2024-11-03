using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Clase con los datos de una columna solicitada para un listado
/// </summary>
public class RequestDataSourceColumnModel : RequestColumnBaseModel
{
	public RequestDataSourceColumnModel(DataSourceColumnModel column) : base(column) {}
}