using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Generators;

/// <summary>
///		Clase para generar la SQL de <see cref="ParserOrderBySectionModel"/>
/// </summary>
internal class QueryOrderByGenerator : QueryBaseGenerator
{
	internal QueryOrderByGenerator(ReportQueryGenerator manager, ParserOrderBySectionModel section, QueryDimensionsCollection queryDimensions) : base(manager)
	{
		Section = section;
		QueryDimensions = queryDimensions;
	}

	/// <summary>
	///		Obtiene la SQL
	/// </summary>
	internal override string GetSql()
	{
		List<(string table, string field, int orderIndex, RequestColumnBaseModel.SortOrder sortOrder)> fieldsSort = [];
		string sql = string.Empty;

			// Obtiene los campos para ORDER BY
			foreach (ParserDimensionModel parserDimension in Section.Dimensions)
			{
				List<QueryFieldModel> fields = QueryDimensions.GetFieldsRequest(parserDimension.DimensionKey);

					// Añade los datos de ordenación
					foreach (QueryFieldModel field in fields)
						if (MustIncludeField(field, parserDimension.WithPrimaryKeys, parserDimension.WithRequestedFields, true))
							if (field.OrderBy != RequestColumnBaseModel.SortOrder.Undefined)
								fieldsSort.Add((parserDimension.Table, field.Alias, field.OrderIndex, field.OrderBy));
			}
			// Ordena por el índice
			fieldsSort.Sort((first, second) => first.orderIndex.CompareTo(second.orderIndex));
			// Obtiene la cadena SQL
			foreach ((string table, string field, int orderIndex, RequestColumnBaseModel.SortOrder sortOrder) in fieldsSort)
				sql = sql.AddWithSeparator($"{Manager.SqlTools.GetFieldName(table, field)} {GetSorting(sortOrder)}", ",");
			// Si hay una cadena SQL adicional en la sección, se añade
			sql = sql.AddWithSeparator(Section.Sql, ",");
			// Si es obligatorio y está vacío, ordena por el primer campo
			if (Section.Required && string.IsNullOrWhiteSpace(sql))
				sql = "1";
			// Añade el ORDER BY
			if (!string.IsNullOrWhiteSpace(sql))
				sql = $"ORDER BY {sql}";
			// Devuelve la cadena SQL
			return sql;

		// Obtiene la cadena con el tipo de ordenación
		string GetSorting(RequestColumnBaseModel.SortOrder sortOrder)
		{
			if (sortOrder == RequestColumnBaseModel.SortOrder.Descending)
				return "DESC";
			else
				return "ASC";
		}
	}

	/// <summary>
	///		Sección que se está generando
	/// </summary>
	internal ParserOrderBySectionModel Section { get; }

	/// <summary>
	///		Consultas de dimensiones
	/// </summary>
	internal QueryDimensionsCollection QueryDimensions { get; }
}
