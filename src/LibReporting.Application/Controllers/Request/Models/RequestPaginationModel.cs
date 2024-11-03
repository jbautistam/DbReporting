namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Definición de la paginación
/// </summary>
public class RequestPaginationModel
{
	/// <summary>
	///		Indica si se debe paginar
	/// </summary>
	public bool MustPaginate { get; set; }

	/// <summary>
	///		Página a consultar
	/// </summary>
	public int Page { get; set; }

	/// <summary>
	///		Indica si es una solicitud de la primera página
	/// </summary>
	public bool IsFirstPage => !MustPaginate || (MustPaginate && Page == 1);

	/// <summary>
	///		Registros por página
	/// </summary>
	public long RecordsPerPage { get; set; }
}
