using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Clase con una colección de <see cref="RequestColumnModel"/>
/// </summary>
internal class RequestColumnCollectionModel : List<RequestColumnModel>
{
	/// <summary>
	///		Añade una serie de columnas solicitadas
	/// </summary>
	internal void AddRange(List<ColumnRequestModel> columnRequests)
	{
		foreach (ColumnRequestModel columnRequest in columnRequests)
			if (!string.IsNullOrWhiteSpace(columnRequest.Id))
				Add(new RequestColumnModel(columnRequest));
	}

	/// <summary>
	///		Comprueba si se ha solicitado una columna
	/// </summary>
	internal bool IsRequested(List<string>? ids)
	{
		// Comprueba que se hayan solicitado todas las expresiones
		if (ids is not null)
			foreach (string id in ids)
				if (!IsRequested(id))
					return false;
		// Si ha llegado hasta aquí es porque todos las expresiones existen
		return true;
	}

	/// <summary>
	///		Comprueba si se ha solicitado una columna
	/// </summary>
	internal bool IsRequested(string id) => Get(id) is not null;

	/// <summary>
	///		Obtiene la expresión solicitada
	/// </summary>
	internal RequestColumnModel? Get(string id) => this.FirstOrDefault(item => item.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase));
}