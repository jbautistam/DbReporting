using Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

/// <summary>
///		Datos de un filtro para la consulta
/// </summary>
internal class QueryFilterModel
{
	internal QueryFilterModel(QueryModel query, RequestFilterModel.ConditionType condition, List<object?> values)
	{
		Query = query;
		Condition = condition;
		Values = values;
	}

	/// <summary>
	///		Obtiene el SQL de la condición
	/// </summary>
	internal string GetSql(string tableAlias, string field) => Query.Generator.SqlTools.SqlFilterGenerator.GetSql(tableAlias, field, GetFilter());

	/// <summary>
	///		Obtiene el SQL de la condición
	/// </summary>
	internal string GetSql(string aggregatedField) => Query.Generator.SqlTools.SqlFilterGenerator.GetSql(string.Empty, aggregatedField, GetFilter());

	/// <summary>
	///		Obtiene los datos de un filtro
	/// </summary>
	private RequestFilterModel GetFilter()
	{
		RequestFilterModel filter = new()
										{
											Condition = Condition
										};

			// Añade los valores
			filter.Values.AddRange(Values);
			// Devuelve el filtro
			return filter;
	}

	/// <summary>
	///		Consulta
	/// </summary>
	internal QueryModel Query { get; }

	/// <summary>
	///		Condición
	/// </summary>
	internal RequestFilterModel.ConditionType Condition { get; }

	/// <summary>
	///		Valores del filtro
	/// </summary>
	internal List<object?> Values { get; }
}