using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Colunma solicitada de una dimensión
/// </summary>
public class RequestDimensionColumnModel : RequestColumnBaseModel
{
	public RequestDimensionColumnModel(DataSourceColumnModel column, bool requestedByUser) : base(column) 
	{
		RequestedByUser = requestedByUser;
	}

	/// <summary>
	///		Indica si se ha solicitado por el usuario o se ha añadido por el motor al ser una columna obligatoria
	/// </summary>
	public bool RequestedByUser { get; }
}
