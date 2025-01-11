namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Datos de un filtro solicitado
/// </summary>
public class RequestFilterModel
{
	/// <summary>
	///		Tipo de condición
	/// </summary>
	public enum ConditionType
	{
		/// <summary>Sin condición</summary>
		Undefined,
		/// <summary>Igual a</summary>
		Equals,
		/// <summary>Menor que</summary>
		Less,
		/// <summary>Mayor que</summary>
		Greater,
		/// <summary>Menor o igual que</summary>
		LessOrEqual,
		/// <summary>Mayor o igual que</summary>
		GreaterOrEqual,
		/// <summary>Contiene un valor</summary>
		Contains,
		/// <summary>Está en una serie de valores</summary>
		In,
		/// <summary>Entre dos valores</summary>
		Between
	}

	/// <summary>
	///		Condición que se debe utilizar
	/// </summary>
	public ConditionType Condition { get; set; }

	/// <summary>
	///		Valores del filtro
	/// </summary>
	public List<object?> Values { get; } = [];
}
