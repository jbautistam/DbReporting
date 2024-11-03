using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Generators;

/// <summary>
///		Clase para generar la SQL de <see cref="ParserPartitionBySectionModel"/>
/// </summary>
internal class QueryPartitionByGenerator : QueryBaseGenerator
{
	internal QueryPartitionByGenerator(ReportQueryGenerator manager, ParserPartitionBySectionModel section) : base(manager)
	{
		Section = section;
	}

	/// <summary>
	///		Obtiene la SQL
	/// </summary>
	internal override string GetSql()
	{
		return string.Empty;
/*
		string sql = GetSqlFieldsForDimensions(Section.Dimensions);

			// Añade los campos adicionales
			if (!string.IsNullOrWhiteSpace(Section.Additional))
				sql = sql.AddWithSeparator(Section.Additional, ",");
			// Añade la cláusula PARTITION BY si es necesario
			if (!string.IsNullOrWhiteSpace(sql))
			{
				// Añade la cláusula PARTITION BY
					sql = $"PARTITION BY {sql}";
				// Añade la cláusula ORDER BY si es necesario
				if (!string.IsNullOrWhiteSpace(Section.OrderBy))
					sql = $"{sql} ORDER BY {Section.OrderBy}";
			}
			// Devuelve la cadena SQL
			return sql;
*/
	}

	/// <summary>
	///		Sección que se está generando
	/// </summary>
	internal ParserPartitionBySectionModel Section { get; }
}
