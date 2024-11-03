namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Clase con los datos de una columna solicitada para un listado
/// </summary>
public class RequestExpressionColumnModel
{
	public RequestExpressionColumnModel(string expressionId)
	{
		ExpressionId = expressionId;
	}

	/// <summary>
	///		Código de expresión
	/// </summary>
	public string ExpressionId { get; }
}