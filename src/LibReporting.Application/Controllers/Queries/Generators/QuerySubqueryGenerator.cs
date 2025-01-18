using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Generators;

/// <summary>
///		Clase para generar la SQL de <see cref="ParserSubquerySectionModel"/>
/// </summary>
internal class QuerySubqueryGenerator : QueryBaseGenerator
{
	internal QuerySubqueryGenerator(ReportQueryGenerator manager, ParserSubquerySectionModel section, List<QuerySqlModel> queriesBlock) : base(manager)
	{
		Section = section;
		QueriesBlock = queriesBlock;
	}

	/// <summary>
	///		Obtiene la SQL
	/// </summary>
	internal override string GetSql()
	{
		if (string.IsNullOrWhiteSpace(Section.Name))
			throw new NotImplementedException("Can't find name for clause 'Subquery'");
		else
		{
			QuerySqlModel? query = QueriesBlock.FirstOrDefault(item => item.Key.Equals(Section.Name, StringComparison.CurrentCultureIgnoreCase));

				// Comprueba la consulta y genera la SQL si la ha encontrado
				if (query is null)
					throw new NotImplementedException($"Can't find the query '{Section.Name}' for clause 'Subquery'");
				else
				{
					// Indica que la consulta se utiliza como subconsulta
					query.IsSubquery = true;
					// Devuelve la consulta
					return query.Sql;
				}
		}
	}

	/// <summary>
	///		Sección que se está generando
	/// </summary>
	internal ParserSubquerySectionModel Section { get; }

	/// <summary>
	///		Bloques de consultas generadas
	/// </summary>
	internal List<QuerySqlModel> QueriesBlock { get; }
}