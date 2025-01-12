namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Clase con los datos de una columna solicitada para un listado
/// </summary>
internal class RequestExpressionModel
{
	internal RequestExpressionModel(string expressionId)
	{
		ExpressionId = expressionId;
	}

	/// <summary>
	///		Código de expresión
	/// </summary>
	internal string ExpressionId { get; }
}