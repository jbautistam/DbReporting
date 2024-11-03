using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Generators;

/// <summary>
///		Clase para generar la SQL de <see cref="ParserFieldsSectionModel"/>
/// </summary>
internal class QueryFieldsGenerator : QueryBaseGenerator
{
	internal QueryFieldsGenerator(ReportQueryGenerator manager, ParserFieldsSectionModel section) : base(manager)
	{
		Section = section;
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
				if (Manager.Request.IsDimensionRequested(fieldDimension.DimensionKey))
				{
					QueryTableModelNew? table = Manager.Request.GetRequestedTable(fieldDimension.Table, fieldDimension.Table, fieldDimension.DimensionKey,
																				  fieldDimension.WithPrimaryKeys, fieldDimension.WithRequestedFields);

						if (table is not null)
							sql = sql.AddWithSeparator(GetSqlFields(fieldDimension, table), ",");
				}
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Obtiene la SQL de los campos
	/// </summary>
	private string GetSqlFields(ParserFieldsDimensionSectionModel fielDimension, QueryTableModelNew table)
	{
		string sql = string.Empty;

			// Genera la SQL para las columnas de la tabla
			foreach (QueryTableColumnModelNew column in table.Columns)
				if (string.IsNullOrEmpty(fielDimension.AdditionalTable))
					sql = sql.AddWithSeparator(column.GetFieldName(), ",");
				else
					sql = sql.AddWithSeparator($"IsNull({column.GetFieldName(fielDimension.AdditionalTable)}, {column.GetFieldName()}) AS {column.NameParts.Alias}", ",");
			// Devuelve la SQL
			return sql;
	}
	
	/// <summary>
	///		Sección que se está generando
	/// </summary>
	internal ParserFieldsSectionModel Section { get; }
}
