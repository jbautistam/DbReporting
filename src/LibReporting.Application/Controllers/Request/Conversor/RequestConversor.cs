using Bau.Libraries.LibReporting.Application.Controllers.Request.Models;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Converson;

/// <summary>
///		Coversor de <see cref="ReportRequestModel"/> en <see cref="ReportRequestModel"/>
/// </summary>
internal class RequestConversor
{
	internal RequestConversor(ReportingManager manager)
	{
		Manager = manager;
	}

	/// <summary>
	///		Convierte la solicitud del usuario en el modelo utilizado por el sistema
	/// </summary>
	internal RequestModel Convert(ReportRequestModel request)
	{
		RequestModel converted = new(Manager, GetReport(request.ReportId));

			// Convierte los datos
			ConvertDimensions(converted, request.Dimensions);
			converted.DataSourceColumns.AddRange(Convert(request.DataSources));
			converted.Expressions.AddRange(Convert(request.Expressions));
			converted.Parameters.AddRange(Convert(converted.Report, request.Parameters));
			// Asigna la paginación
			converted.Pagination.MustPaginate = request.Pagination.MustPaginate;
			converted.Pagination.Page = request.Pagination.Page;
			converted.Pagination.RecordsPerPage = request.Pagination.RecordsPerPage;
			// Devuelve los datos convertidos
			return converted;
	}

	/// <summary>
	///		Obtiene el informe solicitado
	/// </summary>
	private ReportModel GetReport(string id)
	{
		ReportModel? report = Manager.Schema.DataWarehouses.GetReport(id);

			if (report is null)
				throw new Exceptions.ReportingParserException($"Can't find the report {id}");
			else
				return report;
	}

	/// <summary>
	///		Convierte las dimensiones
	/// </summary>
	private void ConvertDimensions(RequestModel request, List<DimensionRequestModel> requestDimensions)
	{
		foreach (DimensionRequestModel requestDimension in requestDimensions)
			foreach (DimensionColumnRequestModel requestColumn in requestDimension.Columns)
			{
				RequestDimensionColumnModel dimensionColumn = request.AddDimension(requestDimension.DimensionId, requestColumn.ColumnId, true);

					// Asigna los datos de la columna solicitada
					AssignColumnRequestData(requestColumn, dimensionColumn);
			}
	}

	/// <summary>
	///		Convierte los orígenes de datos
	/// </summary>
	private List<RequestDataSourceColumnModel> Convert(List<DataSourceRequestModel> requestDataSources)
	{
		List<RequestDataSourceColumnModel> converted = [];

			// Convierte los orígenes de datos
			foreach (DataSourceRequestModel requestDataSource in requestDataSources)
			{
				BaseDataSourceModel? dataSource = Manager.Schema.DataWarehouses.GetDataSource(requestDataSource.ReportDataSourceId);

					if (dataSource is null)
						throw new Exceptions.ReportingParserException($"Can't find the data source {requestDataSource.ReportDataSourceId}");
					else
						foreach (DataSourceColumnRequestModel requestColumn in requestDataSource.Columns)
						{
							DataSourceColumnModel? column = dataSource.Columns[requestColumn.ColumnId];

								if (column is null)
									throw new Exceptions.ReportingParserException($"Can't find the column {requestColumn.ColumnId} at data source {dataSource.GetTableAlias()}");
								else
								{
									RequestDataSourceColumnModel requestDataSourceColumn = new(column, Convert(requestColumn.AggregatedBy));

										// Asigna los datos de la solicitud de la columna
										AssignColumnRequestData(requestColumn, requestDataSourceColumn);
										// Convierte los datos
										converted.Add(requestDataSourceColumn);
								}
						}	
			}
			// Devuelve los datos convertidos
			return converted;

		// Convierte el tipo de agregación
		RequestDataSourceColumnModel.AggregationType Convert(DataSourceColumnRequestModel.AggregationType type)
		{
			return type switch
					{
						DataSourceColumnRequestModel.AggregationType.Sum => RequestDataSourceColumnModel.AggregationType.Sum,
						DataSourceColumnRequestModel.AggregationType.Max => RequestDataSourceColumnModel.AggregationType.Max,
						DataSourceColumnRequestModel.AggregationType.Min => RequestDataSourceColumnModel.AggregationType.Min,
						DataSourceColumnRequestModel.AggregationType.Average => RequestDataSourceColumnModel.AggregationType.Average,
						DataSourceColumnRequestModel.AggregationType.StandardDeviation => RequestDataSourceColumnModel.AggregationType.StandardDeviation,
						_ => RequestDataSourceColumnModel.AggregationType.NoAggregated
					};
		}
	}

	/// <summary>
	///		Convierte las expresiones
	/// </summary>
	private List<RequestExpressionColumnModel> Convert(List<ExpressionColumnRequestModel> expressions)
	{
		List<RequestExpressionColumnModel> converted = [];

			// Convierte los orígenes de datos
			foreach (ExpressionColumnRequestModel requestExpression in expressions)
				converted.Add(new RequestExpressionColumnModel(requestExpression.ColumnId));
			// Devuelve los datos convertidos
			return converted;
	}

	/// <summary>
	///		Asigna los datos de la columna solicitada
	/// </summary>
	private void AssignColumnRequestData(BaseColumnRequestModel request, RequestColumnBaseModel target)
	{
		target.Visible = request.Visible;
		target.OrderIndex = request.OrderIndex;
		target.OrderBy = Convert(request.OrderBy);
		target.FiltersWhere.AddRange(ConvertFilters(request.FiltersWhere));
		target.FiltersHaving.AddRange(ConvertFilters(request.FiltersHaving));

		// Convierte la ordenación
		RequestColumnBaseModel.SortOrder Convert(BaseColumnRequestModel.SortOrder type)
		{
			return type switch
					{
						BaseColumnRequestModel.SortOrder.Ascending => RequestColumnBaseModel.SortOrder.Ascending,
						BaseColumnRequestModel.SortOrder.Descending => RequestColumnBaseModel.SortOrder.Descending,
						_ => RequestColumnBaseModel.SortOrder.Undefined,
					};
		}
	}

	/// <summary>
	///		Convierte una lista de filtros
	/// </summary>
	private List<RequestFilterModel> ConvertFilters(List<FilterRequestModel> requestFilters)
	{
		List<RequestFilterModel> filters = [];

			// Convierte los filtros
			foreach (FilterRequestModel requestFilter in requestFilters)
			{
				RequestFilterModel filter = new()
												{
													Condition = Convert(requestFilter.Condition)
												};

					// Añade los valores
					filter.Values.AddRange(requestFilter.Values);
					// Añade el filtro
					filters.Add(filter);
			}
			// Devuelve los filtros
			return filters;

		// Convierte la condición
		RequestFilterModel.ConditionType Convert(FilterRequestModel.ConditionType condition)
		{
			return condition switch
						{
							FilterRequestModel.ConditionType.Equals => RequestFilterModel.ConditionType.Equals,
							FilterRequestModel.ConditionType.Less => RequestFilterModel.ConditionType.Less,
							FilterRequestModel.ConditionType.Greater => RequestFilterModel.ConditionType.Greater,
							FilterRequestModel.ConditionType.LessOrEqual => RequestFilterModel.ConditionType.LessOrEqual,
							FilterRequestModel.ConditionType.GreaterOrEqual => RequestFilterModel.ConditionType.GreaterOrEqual,
							FilterRequestModel.ConditionType.Contains => RequestFilterModel.ConditionType.Contains,
							FilterRequestModel.ConditionType.In => RequestFilterModel.ConditionType.In,
							FilterRequestModel.ConditionType.Between => RequestFilterModel.ConditionType.Between,
							_ => RequestFilterModel.ConditionType.Undefined
						};
		}
	}

	/// <summary>
	///		Convierte la lista de parámetros
	/// </summary>
	private List<RequestParameterModel> Convert(ReportModel report, List<ParameterRequestModel> parameters)
	{
		List<RequestParameterModel> converted = [];

			// Convierte los parámetros
			foreach (ParameterRequestModel parameter in parameters)
			{
				ReportParameterModel? reportParameter = report.Parameters[parameter.Key];

					if (reportParameter is not null)
						converted.Add(new RequestParameterModel(reportParameter, parameter.Value));
			}
			// Devuelve la lista convertida
			return converted;
	}

	/// <summary>
	///		Manager principal
	/// </summary>
	internal ReportingManager Manager { get; }
}