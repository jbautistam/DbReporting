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
			if (Manager.Request.IsRequestedTotals())
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
	//TODO: la comprobación de expresiones solicitadas debería ir a la Request
	private string GetSql(List<ParserIfRequestExpressionSectionModel> expressions, RequestModel request)
	{
		string sql = string.Empty;

			// Añade las SQL de las expresiones solicitadas
			foreach (ParserIfRequestExpressionSectionModel expression in expressions)
				if (expression.IsDefault || IsRequested(expression, request))
				{
					string separator = ", ";

						// Quita el separador si la sección indica que no se deben añadir
						if (expression.WithoutComma)
							separator = string.Empty;
						// Añade la consulta SQL
						sql = sql.AddWithSeparator(expression.Sql.TrimIgnoreNull(), separator + Environment.NewLine);
				}
			// Devuelve la cadena solicitada
			return sql;
	}

	/// <summary>
	///		Comprueba si se ha solicitado alguna de las expresiones asociadas
	/// </summary>
	private bool IsRequested(ParserIfRequestExpressionSectionModel expression, RequestModel request)
	{
		// Comprueba si se ha solicitado alguna de las expresiones
		foreach (string key in expression.ExpressionKeys)
			if (request.IsExpressionRequested(key))
				return true;
		// Si ha llegado hasta aquí es porque no ha solicitado nada
		return false;
	}
	
	/// <summary>
	///		Sección que se está generando
	/// </summary>
	internal ParserIfRequestSectionModel Section { get; }
}