using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

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
	///		Obtiene la SQL adecuada para esta sección
	/// </summary>
	internal abstract string GetSql();

	/// <summary>
	///		Manager
	/// </summary>
	internal ReportQueryGenerator Manager { get; }
}
