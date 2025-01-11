using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Lista de <see cref="RequestDataSourceColumnModel"/>
/// </summary>
public class RequestDataSourceColumnCollectionModel : List<RequestDataSourceColumnModel>
{
	/// <summary>
	///		Convierte las solicitudes de <see cref="RequestDataSourceColumnModel"/>
	/// </summary>
	internal void AddRange(ReportingManager manager, List<DataSourceRequestModel> requestDataSources)
	{
		List<RequestDataSourceColumnModel> converted = [];

			// Convierte los orígenes de datos
			foreach (DataSourceRequestModel requestDataSource in requestDataSources)
			{
				BaseDataSourceModel? dataSource = manager.Schema.DataWarehouses.GetDataSource(requestDataSource.ReportDataSourceId);

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
										requestDataSourceColumn.AssignColumnRequestData(requestColumn, requestDataSourceColumn);
										// Convierte los datos
										converted.Add(requestDataSourceColumn);
								}
						}	
			}

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
	///		Obtiene las columnas solicitadas para un origen de datos
	/// </summary>
	internal List<RequestDataSourceColumnModel> GetRequestedColumns(BaseDataSourceModel dataSource)
	{
		List<RequestDataSourceColumnModel> columns = [];

			// Obtiene las columnas solicitadas
			foreach (RequestDataSourceColumnModel column in this)
				if (column.Column.DataSource.Id.Equals(dataSource.Id, StringComparison.CurrentCultureIgnoreCase))
					columns.Add(column);
			// Devuelve las columnas solicitadas
			return columns;
	}
}