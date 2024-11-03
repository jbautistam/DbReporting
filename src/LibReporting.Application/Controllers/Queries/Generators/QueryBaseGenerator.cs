using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Generators;

/// <summary>
///		Clase base para generar laS SQL de <see cref="ParserBaseSectionModel"/>
/// </summary>
internal abstract class QueryBaseGenerator
{
	protected QueryBaseGenerator(ReportQueryGenerator manager)
	{
		Manager = manager;
	}

	/// <summary>
	///		Obtiene la cadena SQL para los campos solicitados de las dimensiones
	/// </summary>
	protected List<Models.QueryTableModelNew> GetTablesForDimensions(List<ParserDimensionModel> dimensions, bool includePrimaryKeys, bool includeRequestFields)
	{
		List<Models.QueryTableModelNew> tables = new();

			// Obtiene las tablas
			foreach (ParserDimensionModel dimension in dimensions)
			{
				BaseDimensionModel? dimensionRequested = Manager.Request.GetDimensionIfRequest(dimension);

					if (dimensionRequested is not null)
					{
						Models.QueryTableModelNew? table = Manager.Request.GetRequestedTable(dimension.Table, dimension.TableAlias, dimension.DimensionKey,
																							 includePrimaryKeys, includeRequestFields);

							if (table is not null)
								tables.Add(table);
					}
			}
			// Devuelve la lista de tablas
			return tables;
	}

	/// <summary>
	///		Obtiene la SQL adecuada para esta sección
	/// </summary>
	internal abstract string GetSql();

	/// <summary>
	///		Manager
	/// </summary>
	internal ReportQueryGenerator Manager { get; }
}
