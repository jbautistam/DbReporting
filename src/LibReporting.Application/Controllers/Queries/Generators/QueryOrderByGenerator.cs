using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Generators;

/// <summary>
///		Clase para generar la SQL de <see cref="ParserOrderBySectionModel"/>
/// </summary>
internal class QueryOrderByGenerator : QueryBaseGenerator
{
	internal QueryOrderByGenerator(ReportQueryGenerator manager, ParserOrderBySectionModel section) : base(manager)
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
	internal ParserOrderBySectionModel Section { get; }
}
