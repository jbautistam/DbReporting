using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Generators;

/// <summary>
///		Clase para generar la SQL de <see cref="ParserFieldsSectionModel"/>
/// </summary>
internal class QueryFieldsGenerator : QueryBaseGenerator
{
	internal QueryFieldsGenerator(ReportQueryGenerator manager, ParserFieldsSectionModel section, QueryDimensionsCollection queryDimensions) : base(manager)
	{
		Section = section;
		QueryDimensions = queryDimensions;
	}

	/// <summary>
	///		Obtiene la SQL
	/// </summary>
	internal override string GetSql() 
	{
		string sql = GetSqlFieldsForDimensions(QueryDimensions, Section.ParserDimensions, true);

			// Añade una coma si es obligatoria
			if (!string.IsNullOrWhiteSpace(sql) && Section.WithComma)
				sql += ", ";
			// Devuelve la cadena SQL
			return sql;
	}
	
	/// <summary>
	///		Sección que se está generando
	/// </summary>
	internal ParserFieldsSectionModel Section { get; }

	/// <summary>
	///		Consultas de dimensiones
	/// </summary>
	internal QueryDimensionsCollection QueryDimensions { get; }
}
