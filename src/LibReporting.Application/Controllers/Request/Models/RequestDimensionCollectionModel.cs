using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;
using Bau.Libraries.LibReporting.Models.Base;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Lista de <see cref="RequestDimensionModel"/>
/// </summary>
internal class RequestDimensionCollectionModel : List<RequestDimensionModel>
{
	internal RequestDimensionCollectionModel(RequestModel request)
	{
		Request = request;
	}

	/// <summary>
	///		Añade las dimensiones solicitadas
	/// </summary>
	internal void AddRange(List<DimensionRequestModel> requestDimensions)
	{
		foreach (DimensionRequestModel requestDimension in requestDimensions)
			foreach (DimensionColumnRequestModel requestColumn in requestDimension.Columns)
			{
				RequestDimensionColumnModel dimensionColumn = Add(requestDimension.DimensionId, requestColumn.ColumnId, true);

					// Asigna los datos de la columna solicitada
					dimensionColumn.AssignColumnRequestData(requestColumn);
			}
	}

	/// <summary>
	///		Añade una dimensión y columna a la lista
	/// </summary>
	internal RequestDimensionColumnModel Add(string dimensionId, string columnId, bool requestByUser)
	{
		BaseDimensionModel? dimension = Request.Report.DataWarehouse.Dimensions[dimensionId];

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
								Add(convertedDimension);
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
			foreach (RequestDimensionModel dimension in this)
				if (dimension.Dimension.Id.Equals(dimensionId, StringComparison.CurrentCultureIgnoreCase))
					return dimension;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return null;
		}
	}

	/// <summary>
	///		Comprueba si se ha solicitado una dimensión
	/// </summary>
	internal bool IsRequested(List<string>? ids)
	{
		// Comprueba que se hayan solicitado todas las dimensiones
		if (ids is not null)
			foreach (string id in ids)
				if (!IsRequested(id) && !IsRelatedDimensionRequested(id))
					return false;
		// Si ha llegado hasta aquí es porque todos las dimensiones existen
		return true;
	}

	/// <summary>
	///		Comprueba si se ha solicitado una dimensión
	/// </summary>
	internal bool IsRequested(string id) => GetRequested(id) is not null;

	/// <summary>
	///		Obtiene la dimensión solicitada
	/// </summary>
	internal RequestDimensionModel? GetRequested(string dimensionId) 
	{
		RequestDimensionModel? requestDimension = this.FirstOrDefault(item => item.Dimension.Id.Equals(dimensionId, StringComparison.CurrentCultureIgnoreCase));

			// Si no se ha solicitado la dimensión, se busca si se ha solicitado alguna dimensión relacionada
			if (requestDimension is null)
			{
				BaseDimensionModel? dimension = Request.Report.DataWarehouse.Dimensions[dimensionId];

					if (dimension is not null)
						foreach (DimensionRelationModel relation in dimension.GetRelations())
							if (requestDimension is null && relation.Dimension is not null)
								requestDimension = GetRequested(relation.DimensionId);
			}
			// Devuelve la dimensión solicitada
			return requestDimension;
	}

	/// <summary>
	///		Comprueba si se ha seleccionado un campo de una dimensión relacionada
	/// </summary>
	private bool IsRelatedDimensionRequested(string dimensionId)
	{
		bool isRelated = false;
		BaseDimensionModel? dimension = Request.Report.DataWarehouse.Dimensions[dimensionId];

			// Obtiene la dimensión
			if (dimension is null)
				throw new Exceptions.ReportingParserException($"Can't find the dimension {dimensionId}");
			else if (IsRequested(dimensionId))
				isRelated = true;
			else
				foreach (DimensionRelationModel relation in dimension.GetRelations())
					if (!isRelated && relation.Dimension is not null)
						isRelated = IsRelatedDimensionRequested(relation.Dimension.Id);
			// Devuelve el valor que indica si alguna de las dimensiones relacionadas se ha solicitado
			return isRelated;
	}

	/// <summary>
	///		Obtiene los datos de una dimensión si se ha solicitado
	/// </summary>
	internal BaseDimensionModel? GetIfRequest(ParserDimensionModel parserDimension)
	{
		return GetIfRequest(parserDimension.DimensionKey, parserDimension.Required, parserDimension.RelatedDimensions, 
							parserDimension.IfNotRequestDimensions);
	}

	/// <summary>
	///		Obtiene los datos de una dimensión si se ha solicitado
	/// </summary>
	internal BaseDimensionModel? GetIfRequest(string dimensionKey, bool required, List<string>? relatedDimensions, List<string>? notRequestedDimensions)
	{
		RequestDimensionModel? requestDimension = GetRequested(dimensionKey);

			if (requestDimension is not null && 
					(required || 
					 (IsRequested(relatedDimensions) && !IsRequested(notRequestedDimensions))))
				return requestDimension.Dimension;
			else
				return null;
	}

	/// <summary>
	///		Obtiene los campos solicitados para una dimensión
	/// </summary>
	internal QueryTableModel? GetRequestedTable(string table, string alias, string dimensionKey, bool includePrimaryKeys, bool includeRequestFields)
	{
		List<DataSourceColumnModel>? columns = GetRequestedFields(dimensionKey, includePrimaryKeys, includeRequestFields);

			if (columns is null)
				return null;
			else
			{
				QueryTableModel queryTable = new(table, alias);

					// Añade las columnas
					foreach (DataSourceColumnModel column in columns)
						queryTable.AddColumn(column.IsPrimaryKey, column.Id, column.Alias, column.Type);
					// Devuelve la tabla generada
					return queryTable;
			}
	}

	/// <summary>
	///		Obtiene los campos solicitados para una dimensión
	/// </summary>
	private List<DataSourceColumnModel>? GetRequestedFields(string dimensionKey, bool includePrimaryKeys, bool includeRequestFields)
	{
		RequestDimensionModel? dimensionRequest = GetRequested(dimensionKey);

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
			// Devuelve las columnas
			return columns;
	}

/*
	/// <summary>
	///		Obtiene las solicitudes hija de una solicitud de dimensión (las solicitudes asociadas a dimensiones hija)
	/// </summary>
	private List<RequestDimensionModel> GetChildRequestedDimensions(RequestDimensionModel dimensionRequest)
	{
		List<RequestDimensionModel> requestDimensions = [];

			// Obtiene las solicitudes
			foreach (DimensionRelationModel relation in dimensionRequest.Dimension.GetRelations())
				foreach (RequestDimensionModel requestDimension in this)
					if (relation.Dimension is not null &&
							relation.Dimension.Id.Equals(requestDimension.Dimension.Id, StringComparison.CurrentCultureIgnoreCase))
						requestDimensions.Add(requestDimension);
			// Devuelve la lista encontrada
			return requestDimensions;
	}
*/

	/// <summary>
	///		Solicitud a la que se asocian las dimensiones
	/// </summary>
	internal RequestModel Request { get; }
}