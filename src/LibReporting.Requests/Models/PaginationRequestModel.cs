namespace Bau.Libraries.LibReporting.Requests.Models;

/// <summary>
///		Definición de la paginación
/// </summary>
public class PaginationRequestModel
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
