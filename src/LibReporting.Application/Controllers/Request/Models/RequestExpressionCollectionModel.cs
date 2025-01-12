using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Clase con una colección de <see cref="RequestExpressionModel"/>
/// </summary>
internal class RequestExpressionCollectionModel : List<RequestExpressionModel>
{
	/// <summary>
	///		Añade una serie de expresiones solicitadas
	/// </summary>
	internal void AddRange(List<ExpressionColumnRequestModel> expressions)
	{
		foreach (ExpressionColumnRequestModel requestExpression in expressions)
			Add(new RequestExpressionModel(requestExpression.ColumnId));
	}

	/// <summary>
	///		Comprueba si se ha solicitado una expresión
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
	///		Comprueba si se ha solicitado una expresión
	/// </summary>
	internal bool IsRequested(string id) => GetRequested(id) is not null;

	/// <summary>
	///		Obtiene la expresión solicitada
	/// </summary>
	internal RequestExpressionModel? GetRequested(string id) => this.FirstOrDefault(item => item.ExpressionId.Equals(id, StringComparison.CurrentCultureIgnoreCase));
}