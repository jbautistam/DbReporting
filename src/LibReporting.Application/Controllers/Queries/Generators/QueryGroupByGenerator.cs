using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Generators;

/// <summary>
///		Clase para generar la SQL de <see cref="ParserGroupBySectionModel"/>
/// </summary>
internal class QueryGroupByGenerator : QueryBaseGenerator
{
	internal QueryGroupByGenerator(ReportQueryGenerator manager, ParserGroupBySectionModel section) : base(manager)
	{
		Section = section;
	}

	/// <summary>
	///		Obtiene la SQL
	/// </summary>
	internal override string GetSql()
	{
		return string.Empty;
	}

	/// <summary>
	///		Sección que se está generando
	/// </summary>
	internal ParserGroupBySectionModel Section { get; }

}
