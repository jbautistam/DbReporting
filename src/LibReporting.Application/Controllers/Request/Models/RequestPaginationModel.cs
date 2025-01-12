namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Definición de la paginación
/// </summary>
internal class RequestPaginationModel
{
	/// <summary>
	///		Indica si se debe paginar
	/// </summary>
	internal bool MustPaginate { get; set; }

	/// <summary>
	///		Página a consultar
	/// </summary>
	internal int Page { get; set; }

	/// <summary>
	///		Indica si es una solicitud de la primera página
	/// </summary>
	internal bool IsFirstPage => !MustPaginate || (MustPaginate && Page == 1);

	/// <summary>
	///		Registros por página
	/// </summary>
	internal long RecordsPerPage { get; set; }

	/// <summary>
	///		Indica si se han solicitado totales (estamos en la primera página)
	/// </summary>
	internal bool IsRequestedTotals() => IsFirstPage;
}
