using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Lista de <see cref="RequestDataSourceColumnModel"/>
/// </summary>
internal class RequestDataSourceColumnCollectionModel : List<RequestColumnModel>
{
	internal RequestDataSourceColumnCollectionModel(RequestDataSourceModel parent)
	{
		Parent = parent;
	}

	/// <summary>
	///		Convierte las solicitudes de <see cref="RequestDataSourceColumnModel"/>
	/// </summary>
	internal void AddRange(List<DataSourceRequestModel> requestDataSources)
	{
		List<RequestColumnModel> converted = [];

			foreach (DataSourceRequestModel requestDataSource in requestDataSources)
			{
				BaseDataSourceModel? dataSource = Parent.Parent.Request.Report.DataWarehouse.DataSources[requestDataSource.ReportDataSourceId];

					if (dataSource is null)
						throw new Exceptions.ReportingParserException($"Can't find the data source {requestDataSource.ReportDataSourceId}");
					else
						foreach (ColumnRequestModel requestColumn in requestDataSource.Columns)
						{
							DataSourceColumnModel? column = dataSource.Columns[requestColumn.Id];

								if (column is null)
									throw new Exceptions.ReportingParserException($"Can't find the column {requestColumn.Id} at data source {dataSource.GetTableAlias()}");
								else
									converted.Add(new RequestColumnModel(requestColumn));
						}	
			}
	}

	/// <summary>
	///		Obtiene las columnas solicitadas para un origen de datos
	/// </summary>
	internal List<RequestColumnModel> GetRequestedColumns(BaseDataSourceModel dataSource)
	{
		List<RequestColumnModel> columns = [];

			// Obtiene las columnas solicitadas
			foreach (RequestColumnModel column in this)
				if (column.Id.Equals(dataSource.Id, StringComparison.CurrentCultureIgnoreCase))
					columns.Add(column);
			// Devuelve las columnas solicitadas
			return columns;
	}

	/// <summary>
	///		Solicitud a la que se asocia la colección
	/// </summary>
	internal RequestDataSourceModel Parent { get; }
}