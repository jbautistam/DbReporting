using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Request.Models;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Generators;

/// <summary>
///		Clase con los datos para generar la SQL de <see cref="ParserCondiciontSectionModel"/>
/// </summary>
internal class QueryConditionsGenerator : QueryBaseGenerator
{
	internal QueryConditionsGenerator(ReportQueryGenerator manager, ParserCondiciontSectionModel section) : base(manager)
	{
		Section = section;
	}

	/// <summary>
	///		Obtiene la SQL
	/// </summary>
	internal override string GetSql()
	{
		string sql = string.Empty;

			// Añade los filtros de los orígenes de datos y expresiones
			switch (Section)
			{
				case ParserWhereSectionModel section:
						sql = GetSql(section.DataSources);
					break;
				case ParserHavingSectionModel section:
						sql = GetSql(section.Expressions);
					break;
			}
			// Añade la SQL adicional si existe
			if (!string.IsNullOrWhiteSpace(Section.Sql))
			{
				// Asigna el operador predeterminado
				if (string.IsNullOrWhiteSpace(Section.Operator))
					Section.Operator = "AND";
				// Añade la cadena adicional a la SQL
				sql = sql.AddWithSeparator(Section.Sql, $" {Section.Operator} ");
			}
			// Añade la cláusula WHERE / HAVING si es necesario
			sql = GetClause(Section, sql);
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Añade la cláusula WHERE o HAVING dependiendo del tipo de condición
	/// </summary>
	private string GetClause(ParserCondiciontSectionModel section, string sql)
	{
		if (!string.IsNullOrWhiteSpace(sql))
			return $" {GetClause(section)} {sql}";
		else
			return string.Empty;
	}

	/// <summary>
	///		Obtiene la cláusula de la sección
	/// </summary>
	private string GetClause(ParserCondiciontSectionModel section)
	{
		return section switch
					{
						ParserWhereSectionModel => "WHERE",
						ParserHavingSectionModel => "HAVING",
						_ => throw new Exceptions.ReportingParserException($"Condition type unknown {section.GetType().ToString()}"),
					};
	}

	/// <summary>
	///		Obtiene la cadena SQL de los orígenes de datos
	/// </summary>
	private string GetSql(List<ParserDataSourceModel> dataSources)
	{
		string sql = string.Empty;

			// Añade las consultas del origen de datos
			foreach (ParserDataSourceModel parserDataSource in dataSources)
			{
				BaseDataSourceModel? dataSource = Manager.Request.Report.DataWarehouse.DataSources[parserDataSource.DataSourceKey];

					if (dataSource is not null)
						foreach (RequestDataSourceColumnModel requestColumn in Manager.Request.GetRequestedColumns(dataSource))
							if (requestColumn.FiltersWhere.Count > 0)
								sql = sql.AddWithSeparator(Manager.SqlTools.SqlFilterGenerator.GetSql(parserDataSource.Table, requestColumn.Column.Id, 
																										requestColumn.FiltersWhere), 
															" AND ");
			}
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Obtiene la cadena SQL para una condición HAVING
	/// </summary>
	private string GetSql(List<ParserExpressionModel> expressions)
	{
		string sql = string.Empty;

			// Obtiene las comparaciones de los campos
			foreach (ParserExpressionModel parserExpression in expressions)
			{
				RequestExpressionColumnModel? column = Manager.Request.GetRequestedExpression(parserExpression.Expression);

					// Añade las condiciones del HAVING
					if (column is not null)
						sql = sql.AddWithSeparator(Manager.SqlTools.SqlFilterGenerator.GetSql(parserExpression.Table, column.ExpressionId, 
																							  parserExpression.Aggregation), 
												   " AND ");
			}
			// Devuelve la cadena SQL
			return sql;
	}
	
	/// <summary>
	///		Sección que se está generando
	/// </summary>
	internal ParserCondiciontSectionModel Section { get; }
}
