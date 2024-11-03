using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;
using Bau.Libraries.LibReporting.Models.Base;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;

namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Modelo interno con los datos de la solicitud de un usuario
/// </summary>
internal class RequestModel
{
	internal RequestModel(ReportingManager manager, ReportModel report)
	{
		Manager = manager;
		Report = report;
	}

	/// <summary>
	///		Obtiene los campos solicitados para una dimensión
	/// </summary>
	internal QueryTableModelNew? GetRequestedTable(string table, string alias, string dimensionKey, bool includePrimaryKeys, bool includeRequestFields)
	{
		List<DataSourceColumnModel>? columns = GetRequestedFields(dimensionKey, includePrimaryKeys, includeRequestFields);

			if (columns is null)
				return null;
			else
			{
				QueryTableModelNew queryTable = new(table, alias);

					// Añade las columnas
					foreach (DataSourceColumnModel column in columns)
						queryTable.AddColumn(column.IsPrimaryKey, column.Id, column.Alias, column.Type);
					// Devuelve la tabla generada
					return queryTable;
			}
	}

	/// <summary>
	///		Obtiene los datos de una dimensión si se ha solicitado
	/// </summary>
	internal BaseDimensionModel? GetDimensionIfRequest(ParserDimensionModel parserDimension)
	{
		return GetDimensionIfRequest(parserDimension.DimensionKey, parserDimension.Required, parserDimension.RelatedDimensions, 
									 parserDimension.IfNotRequestDimensions);
	}

	/// <summary>
	///		Obtiene los datos de una dimensión si se ha solicitado
	/// </summary>
	internal BaseDimensionModel? GetDimensionIfRequest(string dimensionKey, bool required, List<string>? relatedDimensions, List<string>? notRequestedDimensions)
	{
		RequestDimensionModel? requestDimension = GetRequestedDimension(dimensionKey);

			if (requestDimension is not null && 
					(required || 
					 (IsDimensionRequested(relatedDimensions) && !IsDimensionRequested(notRequestedDimensions))))
				return requestDimension.Dimension;
			else
				return null;
	}

	/// <summary>
	///		Obtiene los campos solicitados para una dimensión
	/// </summary>
	private List<DataSourceColumnModel>? GetRequestedFields(string dimensionKey, bool includePrimaryKeys, bool includeRequestFields)
	{
		RequestDimensionModel? dimensionRequest = GetRequestedDimension(dimensionKey);

			if (dimensionRequest is not null)
				return GetRequestedFields(dimensionRequest, includePrimaryKeys, includeRequestFields);
			else
				return null;
	}

	/// <summary>
	///		Obtiene los campos solicitados para una dimensión
	/// </summary>
	private List<DataSourceColumnModel> GetRequestedFields(RequestDimensionModel dimensionRequest, bool includePrimaryKeys, bool includeRequestFields)
	{
		BaseDimensionModel dimension = dimensionRequest.Dimension;
		BaseReportingDictionaryModel<DataSourceColumnModel> dimensionColumns = dimension.GetColumns();
		List<DataSourceColumnModel> columns = [];

			// Añade los campos clave
			if (includePrimaryKeys)
				foreach (DataSourceColumnModel column in dimensionColumns.EnumerateValues())
					if (column.IsPrimaryKey)
						columns.Add(column);
			// Asigna los campos
			if (includeRequestFields)
				foreach (RequestDimensionColumnModel columnRequest in dimensionRequest.Columns)
					if (!columnRequest.Column.IsPrimaryKey)
						columns.Add(columnRequest.Column);
			//// Añade las columnas solicitadas para las dimensiones hija
			//foreach (DimensionRequestModel childs in dimensionRequest.Childs)
			//	columns.AddRange(GetRequestedFields(childs, includePrimaryKeys, includeRequestFields));
			// Devuelve las columnas
			return columns;
	}

	/// <summary>
	///		Comprueba si se ha solicitado una dimensión
	/// </summary>
	internal bool IsDimensionRequested(List<string>? ids)
	{
		// Comprueba que se hayan solicitado todas las dimensiones
		if (ids is not null)
			foreach (string id in ids)
				if (!IsDimensionRequested(id))
					return false;
		// Si ha llegado hasta aquí es porque todos las dimensiones existen
		return true;
	}

	/// <summary>
	///		Comprueba si se ha solicitado una dimensión
	/// </summary>
	internal bool IsDimensionRequested(string id) => GetRequestedDimension(id) is not null;

	/// <summary>
	///		Obtiene la dimensión solicitada
	/// </summary>
	internal RequestDimensionModel? GetRequestedDimension(string id) => Dimensions.FirstOrDefault(item => item.Dimension.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase));

	/// <summary>
	///		Comprueba si se ha solicitado una expresión
	/// </summary>
	internal bool IsExpressionRequested(List<string>? ids)
	{
		// Comprueba que se hayan solicitado todas las expresiones
		if (ids is not null)
			foreach (string id in ids)
				if (!IsExpressionRequested(id))
					return false;
		// Si ha llegado hasta aquí es porque todos las expresiones existen
		return true;
	}

	/// <summary>
	///		Comprueba si se ha solicitado una expresión
	/// </summary>
	internal bool IsExpressionRequested(string id) => GetRequestedExpression(id) is not null;

	/// <summary>
	///		Obtiene la expresión solicitada
	/// </summary>
	internal RequestExpressionColumnModel? GetRequestedExpression(string id) => Expressions.FirstOrDefault(item => item.ExpressionId.Equals(id, StringComparison.CurrentCultureIgnoreCase));

	/// <summary>
	///		Añade una dimensión y columna a la lista
	/// </summary>
	internal RequestDimensionColumnModel AddDimension(string dimensionId, string columnId, bool requestByUser)
	{
		BaseDimensionModel? dimension = Manager.Schema.DataWarehouses.GetDimension(dimensionId);

			if (dimension is null)
				throw new Exceptions.ReportingParserException($"Can't find the dimension {dimensionId}");
			else
			{
				DataSourceColumnModel? column = dimension.GetColumn(columnId, false);

					if (column is null)
						throw new Exceptions.ReportingParserException($"Can't find the column {columnId} at dimension {dimension.Id}");
					else
					{
						RequestDimensionModel? convertedDimension = Search(dimension.Id);
						RequestDimensionColumnModel requestColumn = new(column, requestByUser);

							// Añade la dimensión si no estaba ya en la lista
							if (convertedDimension is null)
							{
								convertedDimension = new RequestDimensionModel(dimension);
								Dimensions.Add(convertedDimension);
							}
							// Añade la columna de la solicitud
							convertedDimension.Columns.Add(requestColumn);
							// Devuelve la columna añadida
							return requestColumn;
					}
			}

		// Busca una dimensión en la lista
		RequestDimensionModel? Search(string dimensionId)
		{
			// Busca la dimensión en la lista
			foreach (RequestDimensionModel dimension in Dimensions)
				if (dimension.Dimension.Id.Equals(dimensionId, StringComparison.CurrentCultureIgnoreCase))
					return dimension;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return null;
		}
	}

	/// <summary>
	///		Obtiene las solicitudes hija de una solicitud de dimensión (las solicitudes asociadas a dimensiones hija)
	/// </summary>
	internal List<RequestDimensionModel> GetChildRequestedDimensions(RequestDimensionModel dimensionRequest)
	{
		List<RequestDimensionModel> requestDimensions = [];

			// Obtiene las solicitudes
			foreach (DimensionRelationModel relation in dimensionRequest.Dimension.GetRelations())
				foreach (RequestDimensionModel requestDimension in Dimensions)
					if (relation.Dimension is not null &&
							relation.Dimension.Id.Equals(requestDimension.Dimension.Id, StringComparison.CurrentCultureIgnoreCase))
						requestDimensions.Add(requestDimension);
			// Devuelve la lista encontrada
			return requestDimensions;
	}

	/// <summary>
	///		Obtiene las columnas solicitadas para un origen de datos
	/// </summary>
	internal List<RequestDataSourceColumnModel> GetRequestedColumns(BaseDataSourceModel dataSource)
	{
		List<RequestDataSourceColumnModel> columns = [];

			// Obtiene las columnas solicitadas
			foreach (RequestDataSourceColumnModel column in DataSourceColumns)
				if (column.Column.DataSource.Id.Equals(dataSource.Id, StringComparison.CurrentCultureIgnoreCase))
					columns.Add(column);
			// Devuelve las columnas solicitadas
			return columns;
	}

	/// <summary>
	///		Indica si se han solicitado totales (estamos en la primera página)
	/// </summary>
	public bool IsRequestedTotals() => Pagination.IsFirstPage;

	/// <summary>
	///		Manager principal
	/// </summary>
	internal ReportingManager Manager { get; }

	/// <summary>
	///		Informe sobre el que se hace la solicitud
	/// </summary>
	internal ReportModel Report { get; }

	/// <summary>
	///		Parámetros
	/// </summary>
	internal List<RequestParameterModel> Parameters { get; } = [];

	/// <summary>
	///		Dimensiones solicitadas
	/// </summary>
	internal List<RequestDimensionModel> Dimensions { get; } = [];

	/// <summary>
	///		Expresiones solicitadas
	/// </summary>
	internal List<RequestExpressionColumnModel> Expressions { get; } = [];

	/// <summary>
	///		Solicitudes de orígenes de datos
	/// </summary>
	public List<RequestDataSourceColumnModel> DataSourceColumns { get; } = [];

	/// <summary>
	///		Paginación
	/// </summary>
	public RequestPaginationModel Pagination { get; } = new();
}
