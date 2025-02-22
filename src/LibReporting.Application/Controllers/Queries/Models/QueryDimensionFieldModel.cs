using Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

/// <summary>
///		Campo de una <see cref="QueryDimensionModel"/>
/// </summary>
internal class QueryDimensionFieldModel
{
	// Variables privadas
	private string _alias = string.Empty;

	internal QueryDimensionFieldModel(QueryDimensionModel query, bool primaryKey, string table, string field, string? alias, bool visible)
	{
		Query = query;
		IsPrimaryKey = primaryKey;
		Table = table;
		Field = field;
		if (string.IsNullOrWhiteSpace(alias))
			Alias = field;
		else
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

				// Si no es una clave primaria y estamos en una dimensión derivada, añadimos el prefijo al alias
				if (!IsPrimaryKey && Query.Dimension is LibReporting.Models.DataWarehouses.Dimensions.DimensionChildModel dimension)
					alias = $"{dimension.ColumnsPrefix}{alias}";
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