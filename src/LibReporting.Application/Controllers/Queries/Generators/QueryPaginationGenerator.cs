using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Generators;

/// <summary>
///		Clase para generar la SQL de <see cref="ParserPaginationSectionModel"/>
/// </summary>
internal class QueryPaginationGenerator : QueryBaseGenerator
{
	internal QueryPaginationGenerator(ReportQueryGenerator manager, ParserPaginationSectionModel section) : base(manager)
	{
		Section = section;
	}

	/// <summary>
	///		Obtiene la SQL
	/// </summary>
	internal override string GetSql()
	{
		if (Manager.Request.Pagination.MustPaginate)
			return $"""
						OFFSET {(Manager.Request.Pagination.Page - 1) * Manager.Request.Pagination.RecordsPerPage} 
						ROWS FETCH FIRST {Manager.Request.Pagination.RecordsPerPage} ROWS ONLY
					""";
		else
			return string.Empty;
	}
	
	/// <summary>
	///		Sección que se está generando
	/// </summary>
	internal ParserPaginationSectionModel Section { get; }
}
