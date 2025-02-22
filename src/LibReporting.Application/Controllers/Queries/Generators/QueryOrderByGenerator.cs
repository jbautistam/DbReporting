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
	// Datos de ordenación
	private record SortField(string Table, string Field, int SortIndex, RequestColumnModel.SortOrder Type);

	internal QueryOrderByGenerator(ReportQueryGenerator manager, ParserOrderBySectionModel section, QueryDimensionCollectionModel queryDimensions) : base(manager)
	{
		Section = section;
		QueryDimensions = queryDimensions;
	}

	/// <summary>
	///		Obtiene la SQL
	/// </summary>
	internal override string GetSql()
	{
		List<SortField> fieldsSort = [];
		string sql = string.Empty;

			// Obtiene los campos para ORDER BY asociados a las dimensiones solicitadas
			foreach (ParserDimensionModel parserDimension in Section.Dimensions)
			{
				List<QueryDimensionFieldModel> fields = QueryDimensions.GetFieldsRequest(parserDimension.DimensionKey);

					// Añade los datos de ordenación
					foreach (QueryDimensionFieldModel field in fields)
						if (MustIncludeField(field, parserDimension.WithPrimaryKeys, parserDimension.WithRequestedFields, true))
						{
							SortField? sortField = GetSortDimensionRequest(parserDimension, field);

								// Añade el campo de ordenación a la lista
								if (sortField is not null && sortField.Type != RequestColumnModel.SortOrder.Undefined)
									fieldsSort.Add(sortField);
						}
			}
			// Obtiene los campos para ORDER BY asociados a las expresiones solicitadas
			foreach (ParserExpressionModel parserExpression in Section.Expressions)
			{
				SortField? sortField = GetSortExpressionRequest(parserExpression);

					// Añade el campo de ordenación a la lista
					if (sortField is not null && sortField.Type != RequestColumnModel.SortOrder.Undefined)
						fieldsSort.Add(sortField);
			}
			// Ordena por el índice
			fieldsSort.Sort((first, second) => first.SortIndex.CompareTo(second.SortIndex));
			// Obtiene la cadena SQL
			foreach (SortField sortField in fieldsSort)
				sql = sql.AddWithSeparator(GetSqlOrderField(sortField), ",");
			// Si hay una cadena SQL adicional en la sección, se añade
			sql = sql.AddWithSeparator(Section.Sql, ",");
			// Si se debe paginar y la cadena de ordenación está vacía, ordena por el primer campo
			if (NeedPaginate() && string.IsNullOrWhiteSpace(sql))
				sql = "1";
			// Añade el ORDER BY
			if (!string.IsNullOrWhiteSpace(sql))
				sql = $"ORDER BY {sql}";
			// Añade la cadena de paginación
			sql = sql.AddWithSeparator(GetPaginationSql(NeedPaginate()), Environment.NewLine);
			// Devuelve la cadena SQL
			return sql;

	}

	/// <summary>
	///		Obtiene la ordenación solicitada para un campo de dimensión
	/// </summary>
	private SortField? GetSortDimensionRequest(ParserDimensionModel dimension, QueryDimensionFieldModel field)
	{
		RequestDimensionModel? requestDimension = Manager.Request.Dimensions.GetRequested(dimension.DimensionKey);

			// Si realmente se ha solicitado esta dimensión
			if (requestDimension is not null)
			{
				RequestColumnModel? requestColumn = requestDimension.Columns.Get(field.Field);

					if (requestColumn is not null && requestColumn.OrderBy != RequestColumnModel.SortOrder.Undefined)
						return new SortField(dimension.Table, field.Alias, requestColumn.OrderIndex, requestColumn.OrderBy);
			}
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return null;
	}

	/// <summary>
	///		Obtiene la ordenación solicitada para una expresión
	/// </summary>
	private SortField? GetSortExpressionRequest(ParserExpressionModel expression)
	{
		RequestColumnModel? requestExpression = Manager.Request.Expressions.Get(expression.Expression);

			// Si realmente se ha solicitado esta expresión
			if (requestExpression is not null && requestExpression.OrderBy != RequestColumnModel.SortOrder.Undefined)
				return new SortField(expression.Table, expression.Field, requestExpression.OrderIndex, requestExpression.OrderBy);
			else
				return null;
	}

	/// <summary>
	///		Obtiene la cadena SQL con la cláusula de ordenación del campo
	/// </summary>
	private string GetSqlOrderField(SortField sort)
	{
		string sql = GetSortType(sort.Type);

			// Añade el campo si hay algo que ordenar
			if (!string.IsNullOrWhiteSpace(sql))
				sql = $"{Manager.SqlTools.GetFieldName(sort.Table, sort.Field)} {sql}";
			// Devuelve la cadena SQL
			return sql;
			
		// Obtiene la cláusula de ordenación
		string GetSortType(RequestColumnModel.SortOrder type)
		{
			return type switch
					{ 
						RequestColumnModel.SortOrder.Descending => "DESC",
						RequestColumnModel.SortOrder.Ascending => "ASC",
						_ => string.Empty
					};
		}
	}

	/// <summary>
	///		Obtiene la SQL para la paginación
	/// </summary>
	private string GetPaginationSql(bool needPaginate)
	{
		if (needPaginate)
			return $"""
						OFFSET {(Manager.Request.Pagination.Page - 1) * Manager.Request.Pagination.RecordsPerPage} 
						ROWS FETCH FIRST {Manager.Request.Pagination.RecordsPerPage} ROWS ONLY
					""";
		else
			return string.Empty;
	}

	/// <summary>
	///		Indica si se debe paginar la consulta
	/// </summary>
	private bool NeedPaginate() => Manager.Request.Pagination.MustPaginate;

	/// <summary>
	///		Sección que se está generando
	/// </summary>
	internal ParserOrderBySectionModel Section { get; }

	/// <summary>
	///		Consultas de dimensiones
	/// </summary>
	internal QueryDimensionCollectionModel QueryDimensions { get; }
}