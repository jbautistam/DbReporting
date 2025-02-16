using Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

/// <summary>
///		Campo de una consulta
/// </summary>
internal class QueryFieldModel
{
	// Variables privadas
	private string _alias = string.Empty;

	internal QueryFieldModel(QueryDimensionModel query, bool primaryKey, string table, string field, string alias, bool visible)
	{
		Query = query;
		IsPrimaryKey = primaryKey;
		Table = table;
		Field = field;
		Alias = alias;
		Visible = visible;
	}

	/// <summary>
	///		Cosulta a la que se asocia el campo
	/// </summary>
	internal QueryDimensionModel Query { get; }

	/// <summary>
	///		Indica si es una clave primaria
	/// </summary>
	internal bool IsPrimaryKey { get; }

	/// <summary>
	///		Nombre o alias de la tabla
	/// </summary>
	internal string Table { get; }

	/// <summary>
	///		Nombre del campo
	/// </summary>
	internal string Field { get; }

	/// <summary>
	///		Alias
	/// </summary>
	internal string Alias 
	{ 
		get
		{
			string alias = _alias;

				// Si no se ha definido el alias, se calcula
				if (string.IsNullOrWhiteSpace(_alias))
				{
					// Genera el alias inicial
					alias = $"{Table}_{Field}";
				}
				// Devuelve el alias
				return alias;
		}
		set { _alias = value; }
	}

	/// <summary>
	///		Agregación
	/// </summary>
	internal RequestColumnModel.AggregationType Aggregation { get; }

	/// <summary>
	///		Indica si la columna es visible en la consulta
	/// </summary>
	internal bool Visible { get; }

	/// <summary>
	///		Filtros de la cláusula WHERE
	/// </summary>
	internal List<QueryFilterModel> FiltersWhere { get; } = [];

	/// <summary>
	///		Filtros de la cláusula HAVING
	/// </summary>
	internal List<QueryFilterModel> FilterHaving { get; } = [];
}