using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Request.Models;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Generators;

/// <summary>
///		Clase para generar la SQL de <see cref="ParserOrderBySectionModel"/>
/// </summary>
internal class QueryOrderByGenerator : QueryBaseGenerator
{
	internal QueryOrderByGenerator(ReportQueryGenerator manager, ParserOrderBySectionModel section, Models.QueryDimensionsCollection queryDimensions) : base(manager)
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
				RequestDimensionModel? requestDimension = Manager.Request.Dimensions.GetRequested(parserDimension.DimensionKey);

					// Obtiene las columnas solicitadas ordenables
					if (requestDimension is not null)
					{
						List<string> fields = QueryDimensions.GetFieldsRequest(parserDimension.DimensionKey, parserDimension.WithRequestedFields, 
																			   parserDimension.WithPrimaryKeys);

							foreach (string field in fields)
							{
								BaseDimensionModel? dimension = Manager.Request.Report.DataWarehouse.Dimensions[parserDimension.DimensionKey];

									if (dimension is not null)
									{
										DataSourceColumnModel? column = dimension.GetColumn(field, true);

											if (column is not null)
											{
												RequestDimensionColumnModel? requestColumn = requestDimension.GetRequestColumn(column.Id);

													// Añade los datos de ordenación
													if (requestColumn is not null && requestColumn.OrderBy != RequestColumnBaseModel.SortOrder.Undefined)
														fieldsSort.Add((parserDimension.Table, field, requestColumn.OrderIndex, requestColumn.OrderBy));
											}
									}
							}
					}
			}
			// Ordena por el índice
			fieldsSort.Sort((first, second) => first.orderIndex.CompareTo(second.orderIndex));
			// Obtiene la cadena SQL
			foreach ((string table, string field, int orderIndex, RequestColumnBaseModel.SortOrder sortOrder) in fieldsSort)
				sql = sql.AddWithSeparator($"{Manager.SqlTools.GetFieldName(table, field)} {GetSorting(sortOrder)}", ",");
			// Si hay una cadena SQL adicional en la sección, se añade
			sql = sql.AddWithSeparator(Section.AdditionalSql, ",");
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
	internal Models.QueryDimensionsCollection QueryDimensions { get; }
}
