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
			// Se añade la SQL para las expresiones de totales
			if (Manager.Request.Pagination.IsRequestedTotals())
				sql = sql.AddWithSeparator(GetSql(Section.WhenRequestTotals, Manager.Request), "," + Environment.NewLine);
			// Añade una coma si es necesario
			if (Section.WithComma && !string.IsNullOrWhiteSpace(sql))
				sql += ", ";
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Obtiene la SQL de una lista de expresiones
	/// </summary>
	private string GetSql(List<ParserIfRequestExpressionSectionModel> expressions, RequestModel request)
	{
		string sql = string.Empty;

			// Añade las SQL de las expresiones solicitadas
			foreach (ParserIfRequestExpressionSectionModel expression in expressions)
				if (expression.IsDefault || Manager.Request.Expressions.IsRequested(expression.ExpressionKeys))
				{
					string separator = ", ";

						// Quita el separador si la sección indica que no se deben añadir
						if (!expression.WithComma)
							separator = string.Empty;
						// Añade la consulta SQL
						if (!string.IsNullOrWhiteSpace(expression.Sql))
							sql = sql.AddWithSeparator(expression.Sql.TrimIgnoreNull(), separator + Environment.NewLine);
				}
			// Devuelve la cadena solicitada
			return sql;
	}
	
	/// <summary>
	///		Sección que se está generando
	/// </summary>
	internal ParserIfRequestSectionModel Section { get; }
}