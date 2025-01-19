using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Generators;

/// <summary>
///		Clase para generar la SQL de <see cref="ParserGroupBySectionModel"/>
/// </summary>
internal class QueryGroupByGenerator : QueryBaseGenerator
{
	internal QueryGroupByGenerator(ReportQueryGenerator manager, ParserGroupBySectionModel section, Models.QueryDimensionsCollection queryDimensions) : base(manager)
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

			// Añade la SQL adicional
			sql = sql.AddWithSeparator(Section.Sql, ",");
			// Obtiene la cadena de salida
			if (!string.IsNullOrWhiteSpace(sql))
				sql = $" GROUP BY {sql}";
			// Devuelve la cadena con los campos
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
				List<QueryFieldModel> fields = QueryDimensions.GetFieldsRequest(dimension.DimensionKey);

					//TODO: esto se podría combinar con el QueryFieldsGenerator y posiblemente con otros
					// Añade los campos solicitados a la SQL
					foreach (QueryFieldModel field in fields)
						if (field.IsPrimaryKey && dimension.WithPrimaryKeys || (!field.IsPrimaryKey && dimension.WithRequestedFields))
						{
							string sqlField = string.Empty;

								// Añade el nombre del campo
								if (dimension.CheckIfNull)
									sqlField = $"IsNull({Manager.SqlTools.GetFieldName(dimension.TableAlias, field.Alias)}, {Manager.SqlTools.GetFieldName(dimension.AdditionalTable, field.Alias)}) AS {field}";
								else
									sqlField = Manager.SqlTools.GetFieldName(dimension.TableAlias, field.Alias);
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
	internal ParserGroupBySectionModel Section { get; }

	/// <summary>
	///		Colección de consultas de dimensiones 
	/// </summary>
	internal Models.QueryDimensionsCollection QueryDimensions { get; }
}
