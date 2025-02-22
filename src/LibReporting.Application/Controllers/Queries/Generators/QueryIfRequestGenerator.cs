using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Generators;

/// <summary>
///		Clase para generar la SQL de <see cref="ParserIfRequestSectionModel"/>
/// </summary>
internal class QueryIfRequestGenerator : QueryBaseGenerator
{
	internal QueryIfRequestGenerator(ReportQueryGenerator manager, ParserIfRequestSectionModel section) : base(manager)
	{
		Section = section;
	}

	/// <summary>
	///		Obtiene la SQL adecuada para esta sección
	/// </summary>
	internal override string GetSql()
	{
		string sql;

			// Añade la SQL para las expresiones normales
			sql = GetSql(Section.Expressions, Manager.Request);
			// Añade una coma si es necesario
			if (Section.WithComma && !string.IsNullOrWhiteSpace(sql))
				sql += ", ";
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Obtiene la SQL de una lista de expresiones
	/// </summary>
	private string GetSql(List<ParserIfRequestSectionExpressionModel> expressions, RequestModel request)
	{
		string sql = string.Empty;

			// Añade las SQL de las expresiones solicitadas
			if (expressions.Count == 0 && !request.Pagination.IsRequestedTotals())
				throw new Exceptions.ReportingParserException("IfRequest hasn't expression keys");
			else
				foreach (ParserIfRequestSectionExpressionModel sectionExpression in expressions)
					if (Manager.Request.Expressions.IsRequested(sectionExpression.Expressions))
					{
						if (!string.IsNullOrWhiteSpace(sectionExpression.Sql))
							sql = sql.AddWithSeparator(sectionExpression.Sql.TrimIgnoreNull(), "," + Environment.NewLine);
						else if (Manager.Request.Pagination.IsRequestedTotals() && !string.IsNullOrWhiteSpace(sectionExpression.SqlTotals))
							sql = sql.AddWithSeparator(sectionExpression.SqlTotals, "," + Environment.NewLine);
					}
					else if (!string.IsNullOrWhiteSpace(sectionExpression.SqlWhenNotRequest))
						sql = sql.AddWithSeparator(sectionExpression.SqlWhenNotRequest, "," + Environment.NewLine);
			// Devuelve la cadena solicitada
			return sql;
	}

	/// <summary>
	///		Sección que se está generando
	/// </summary>
	internal ParserIfRequestSectionModel Section { get; }
}