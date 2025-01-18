using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Generators;

/// <summary>
///		Clase para generar la SQL de <see cref="ParserPartitionBySectionModel"/>
/// </summary>
internal class QueryPartitionByGenerator : QueryBaseGenerator
{
	internal QueryPartitionByGenerator(ReportQueryGenerator manager, ParserPartitionBySectionModel section, Models.QueryDimensionsCollection queryDimensions) : base(manager)
	{
		Section = section;
		QueryDimensions = queryDimensions;
	}

	/// <summary>
	///		Obtiene la SQL
	/// </summary>
	internal override string GetSql()
	{
		string sql = GetSqlFieldsForDimensions(Section.Dimensions);

			// Añade los campos adicionales
			if (!string.IsNullOrWhiteSpace(Section.Additional))
				sql = sql.AddWithSeparator(Section.Additional, ",");
			// Añade la cláusula PARTITION BY si es necesario
			if (!string.IsNullOrWhiteSpace(sql))
				sql = $"PARTITION BY {sql}";
			// Añade la cláusula ORDER BY si es necesario
			if (!string.IsNullOrWhiteSpace(Section.OrderBy))
				sql = $"{sql} ORDER BY {Section.OrderBy}";
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Obtiene la cadena SQL para los campos solicitados de las dimensiones
	/// </summary>
	private string GetSqlFieldsForDimensions(List<ParserDimensionModel> dimensions)
	{
		string sql = string.Empty;

			// Obtiene los campos
			foreach (ParserDimensionModel dimension in dimensions)
			{
				List<string> fields = QueryDimensions.GetFieldsRequest(dimension.DimensionKey, dimension.WithRequestedFields, dimension.WithPrimaryKeys);

					// Añade los campos solicitados a la SQL
					foreach (string field in fields)
					{
						string sqlField = string.Empty;

							if (dimension.CheckIfNull)
								sqlField = $"IsNull({Manager.SqlTools.GetFieldName(dimension.TableAlias, field)}, {Manager.SqlTools.GetFieldName(dimension.AdditionalTable, field)}) AS {field}";
							else
								sqlField = Manager.SqlTools.GetFieldName(dimension.TableAlias, field);
							// Añade el campo a la cadena SQL
							sql = sql.AddWithSeparator(sqlField, ",");
					}
			}
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Sección que se está generando
	/// </summary>
	internal ParserPartitionBySectionModel Section { get; }

	/// <summary>
	///		Consultas de dimensiones
	/// </summary>
	internal Models.QueryDimensionsCollection QueryDimensions { get; }
}
