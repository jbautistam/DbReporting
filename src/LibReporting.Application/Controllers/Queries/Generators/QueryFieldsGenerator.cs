using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Generators;

/// <summary>
///		Clase para generar la SQL de <see cref="ParserFieldsSectionModel"/>
/// </summary>
internal class QueryFieldsGenerator : QueryBaseGenerator
{
	internal QueryFieldsGenerator(ReportQueryGenerator manager, ParserFieldsSectionModel section, QueryDimensionsCollection queryDimensions) : base(manager)
	{
		Section = section;
		QueryDimensions = queryDimensions;
	}

	/// <summary>
	///		Obtiene la SQL
	/// </summary>
	internal override string GetSql() 
	{
		string sql = GetSqlFieldsForDimensions(Section.ParserDimensions);

			// Añade una coma si es obligatoria
			if (!string.IsNullOrWhiteSpace(sql) && Section.WithComma)
				sql += ", ";
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Obtiene la cadena SQL para los campos solicitados de las dimensiones
	/// </summary>
	private string GetSqlFieldsForDimensions(List<ParserFieldsDimensionSectionModel> dimensions)
	{
		string sql = string.Empty;

			// Obtiene los campos
			foreach (ParserFieldsDimensionSectionModel fieldDimension in dimensions)
			{
				List<QueryFieldModel> fields = QueryDimensions.GetFieldsRequest(fieldDimension.DimensionKey);

					//TODO: esto se podría combinar con el QueryGroupByGenerator y posiblemente con otros
					// Añade los campos solicitados a la SQL
					foreach (QueryFieldModel field in fields)
						if (field.IsPrimaryKey && fieldDimension.WithPrimaryKeys || (!field.IsPrimaryKey && fieldDimension.WithRequestedFields))
						{
							string sqlField = string.Empty;

								// Añade el nombre del campo
								sqlField = Manager.SqlTools.GetFieldName(fieldDimension.Table, field.Alias);
								// Añade el nombre del campo
								if (fieldDimension.CheckIfNull)
									sqlField = $"IsNull({Manager.SqlTools.GetFieldName(fieldDimension.Table, field.Alias)}, {Manager.SqlTools.GetFieldName(fieldDimension.AdditionalTable, field.Alias)}) AS {field}";
								else
									sqlField = Manager.SqlTools.GetFieldName(fieldDimension.Table, field.Alias);
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
	internal ParserFieldsSectionModel Section { get; }

	/// <summary>
	///		Consultas de dimensiones
	/// </summary>
	internal QueryDimensionsCollection QueryDimensions { get; }
}
