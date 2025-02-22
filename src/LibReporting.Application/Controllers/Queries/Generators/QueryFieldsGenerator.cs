using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Generators;

/// <summary>
///		Clase para generar la SQL de <see cref="ParserFieldsSectionModel"/>
/// </summary>
internal class QueryFieldsGenerator : QueryBaseGenerator
{
	internal QueryFieldsGenerator(ReportQueryGenerator manager, ParserFieldsSectionModel section, QueryDimensionCollectionModel queryDimensions) : base(manager)
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

			// Añade los campos de las expresiones
			sql = sql.AddWithSeparator(GetSqlForExpressions(Section.ParserExpressions), ", ", true);
			// Añade la consulta de totales
			if (!string.IsNullOrWhiteSpace(Section.SqlTotals) && Manager.Request.Pagination.IsRequestedTotals())
				sql = sql.AddWithSeparator(Section.SqlTotals, "," + Environment.NewLine);
			// Añade una coma si es obligatoria
			if (!string.IsNullOrWhiteSpace(sql))
			{
				if (Section.WithComma)
					sql += ", ";
				else if (Section.WithPreviousComma)
					sql = $", {sql}";
			}
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Obtiene la SQL de una lista de expresiones
	/// </summary>
	private string GetSqlForExpressions(List<ParserIfRequestSectionExpressionModel> expressions)
	{
		string sql = string.Empty;

			// Añade las SQL de las expresiones solicitadas
			foreach (ParserIfRequestSectionExpressionModel sectionExpression in expressions)
				if (Manager.Request.Expressions.IsRequested(sectionExpression.Expressions))
				{
					// Añade la consulta de la expresión
					if (!string.IsNullOrWhiteSpace(sectionExpression.Sql))
						sql = sql.AddWithSeparator(sectionExpression.Sql, "," + Environment.NewLine);
					// Añade la consulta de la expresión para totales
					if (!string.IsNullOrWhiteSpace(sectionExpression.SqlTotals) && Manager.Request.Pagination.IsRequestedTotals())
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
	internal ParserFieldsSectionModel Section { get; }

	/// <summary>
	///		Consultas de dimensiones
	/// </summary>
	internal QueryDimensionCollectionModel QueryDimensions { get; }
}
