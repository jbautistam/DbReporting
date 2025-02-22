using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Generators;

/// <summary>
///		Clase para generar la SQL de <see cref="ParserPartitionBySectionModel"/>
/// </summary>
internal class QueryPartitionByGenerator : QueryBaseGenerator
{
	internal QueryPartitionByGenerator(ReportQueryGenerator manager, ParserPartitionBySectionModel section, QueryDimensionCollectionModel queryDimensions) : base(manager)
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

			// Añade los campos adicionales
			if (!string.IsNullOrWhiteSpace(Section.Sql))
				sql = sql.AddWithSeparator(Section.Sql, ",");
			// Añade la cláusula PARTITION BY si es necesario
			if (!string.IsNullOrWhiteSpace(sql))
				sql = $"PARTITION BY {sql}";
			// Añade la cláusula ORDER BY si es necesario
			if (!string.IsNullOrWhiteSpace(Section.OrderBy))
				sql = $"{sql} ORDER BY {Section.OrderBy}";
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Sección que se está generando
	/// </summary>
	internal ParserPartitionBySectionModel Section { get; }

	/// <summary>
	///		Consultas de dimensiones
	/// </summary>
	internal QueryDimensionCollectionModel QueryDimensions { get; }
}
