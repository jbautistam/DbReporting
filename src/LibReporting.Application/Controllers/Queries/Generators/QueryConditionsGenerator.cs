using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Generators;

/// <summary>
///		Clase con los datos para generar la SQL de <see cref="ParserFilterSectionModel"/>
/// </summary>
internal class QueryConditionsGenerator : QueryBaseGenerator
{
	internal QueryConditionsGenerator(ReportQueryGenerator manager, ParserFilterSectionModel section) : base(manager)
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
			sql = GetSql(Section, Section.DataSources);
			sql = sql.AddWithSeparator(GetSql(Section, Section.Expressions), Environment.NewLine + "AND");
			// Añade la SQL adicional si existe
			if (!string.IsNullOrWhiteSpace(Section.Sql))
			{
				// Asigna el operador predeterminado
				if (string.IsNullOrWhiteSpace(Section.Operator))
					Section.Operator = "AND";
				// Añade la cadena adicional a la SQL
				sql = sql.AddWithSeparator(Section.Sql, $"{Environment.NewLine} {Section.Operator} ");
			}
			// Añade la cláusula WHERE / HAVING si es necesario
			if (!string.IsNullOrWhiteSpace(sql))
				sql = $"{GetClause(Section)} {sql}";
			// Devuelve la cadena SQL
			return sql;

		// Obtiene la cláusula de la sección
		string GetClause(ParserFilterSectionModel section)
		{
			return section.Type switch
						{
							ParserFilterSectionModel.FilterType.Where => "WHERE",
							ParserFilterSectionModel.FilterType.Having => "HAVING",
							_ => throw new Exceptions.ReportingParserException($"Condition type unknown {section.Type.ToString()}"),
						};
		}
	}

	/// <summary>
	///		Obtiene la cadena SQL para las condiciones sobre <see cref="ParserDataSourceModel"/>
	/// </summary>
	private string GetSql(ParserFilterSectionModel section, List<ParserDataSourceModel> dataSources)
	{
		string sql = string.Empty;

/*
			// Añade las consultas del origen de datos
			foreach (ParserDataSourceModel parserDataSource in dataSources)
			{
				BaseDataSourceModel? dataSource = Manager.Request.Report.DataWarehouse.DataSources[parserDataSource.DataSourceKey];

					if (dataSource is not null)
						foreach (RequestColumnModel requestColumn in Manager.Request.GetHashCode.GetRequestedColumns(dataSource))
							if (requestColumn.FiltersWhere.Count > 0)
								sql = sql.AddWithSeparator(Manager.SqlTools.SqlFilterGenerator.GetSql(parserDataSource.Table, requestColumn.Column.Id, 
																									  requestColumn.FiltersWhere), 
															" AND ");
			}
*/
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Obtiene la cadena SQL para las condiciones sobre <see cref="ParserExpressionModel"/>
	/// </summary>
	private string GetSql(ParserFilterSectionModel section, List<ParserExpressionModel> expressions)
	{
		string sql = string.Empty;

			// Obtiene las comparaciones de los campos
			foreach (ParserExpressionModel parserExpression in expressions)
			{
				RequestColumnModel? column = Manager.Request.Expressions.Get(parserExpression.Expression);

					// Añade las condiciones del filtro
					if (column is not null)
						sql = sql.AddWithSeparator(Manager.SqlTools.SqlFilterGenerator.GetSql(parserExpression.Table, parserExpression.Field, 
																							  GetAggregation(section.Type, section.Aggregation),
																							  GetFilters(section.Type, column)), 
												   Environment.NewLine + "AND");
			}
			// Devuelve la cadena SQL
			return sql;

		// Obtiene la cláusula de agregación
		string GetAggregation(ParserFilterSectionModel.FilterType type, string? aggregation)
		{
			if (type == ParserFilterSectionModel.FilterType.Where)
				return string.Empty;
			else
			{
				if (string.IsNullOrWhiteSpace(aggregation))
					return "SUM";
				else
					return aggregation;
			}
		}

		// Obtiene la cláusula de agregación
		List<RequestFilterModel> GetFilters(ParserFilterSectionModel.FilterType type, RequestColumnModel column)
		{
			if (type == ParserFilterSectionModel.FilterType.Where)
				return column.FiltersWhere;
			else
				return column.FiltersHaving;
		}
	}
	
	/// <summary>
	///		Sección que se está generando
	/// </summary>
	internal ParserFilterSectionModel Section { get; }
}