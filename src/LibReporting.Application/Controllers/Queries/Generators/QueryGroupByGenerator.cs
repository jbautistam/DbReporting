using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Generators;

/// <summary>
///		Clase para generar la SQL de <see cref="ParserGroupBySectionModel"/>
/// </summary>
internal class QueryGroupByGenerator : QueryBaseGenerator
{
	internal QueryGroupByGenerator(ReportQueryGenerator manager, ParserGroupBySectionModel section, QueryDimensionCollectionModel queryDimensions) : base(manager)
	{
		Section = section;
		QueryDimensions = queryDimensions;
	}

	/// <summary>
	///		Obtiene la SQL
	/// </summary>
	internal override string GetSql()
	{
		string sql = GetSqlFieldsForDimensions(QueryDimensions, Section.Dimensions, true);

			// Añade la SQL adicional
			if (!string.IsNullOrWhiteSpace(Section.Sql))
				sql = sql.AddWithSeparator(Section.Sql, ",");
			// Obtiene la cadena de salida
			if (!string.IsNullOrWhiteSpace(sql))
				sql = $" GROUP BY {sql}";
			// Devuelve la cadena con los campos
			return sql;
	}

	/// <summary>
	///		Sección que se está generando
	/// </summary>
	internal ParserGroupBySectionModel Section { get; }

	/// <summary>
	///		Colección de consultas de dimensiones 
	/// </summary>
	internal QueryDimensionCollectionModel QueryDimensions { get; }
}
