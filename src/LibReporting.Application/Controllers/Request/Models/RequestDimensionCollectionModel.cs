using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
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
			foreach (ColumnRequestModel requestColumn in requestDimension.Columns)
				Add(requestDimension.DimensionId, new RequestColumnModel(requestColumn));
	}

	/// <summary>
	///		Normaliza la solicitud añadiendo las dimensiones que aparezcan en el informe como obligatorios
	///	pero para las que no se haya añadido ningún campo
	/// </summary>
	internal void Normalize()
	{
		foreach (ReportRequestDimension fixedRequest in Request.Report.RequestDimensions)
			if (fixedRequest.Required || CheckIsRequestedAnyField(Request, fixedRequest))
				foreach (ReportRequestDimensionField field in fixedRequest.Fields)
					Request.Dimensions.Add(fixedRequest.DimensionKey,
										   new RequestColumnModel(new ColumnRequestModel(field.Field)));

		// Comprueba si se ha solicitado alguno de los campos considerados como obligatorios
		bool CheckIsRequestedAnyField(RequestModel reportRequest, ReportRequestDimension fixedRequest)
		{
			RequestDimensionModel? dimensionRequest = reportRequest.Dimensions.GetRequested(fixedRequest.DimensionKey);

				// Si se ha solicitado la dimensión
				if (dimensionRequest is not null)
					foreach (ReportRequestDimensionField field in fixedRequest.Fields)
						if (dimensionRequest.GetRequestColumn(field.Field) is not null)
							return true;
				// Si ha llegado hasta aquí es porque no se ha solicitado
				return false;
		}
	}

	/// <summary>
	///		Añade una dimensión y columna a la lista
	/// </summary>
	private RequestColumnModel Add(string dimensionId, RequestColumnModel requestColumn)
	{
		BaseDimensionModel? dimension = Request.Report.DataWarehouse.Dimensions[dimensionId];

			if (dimension is null)
				throw new Exceptions.ReportingParserException($"Can't find the dimension {dimensionId}");
			else
			{
				DataSourceColumnModel? column = dimension.GetColumn(requestColumn.Id, false);

					if (column is null)
						throw new Exceptions.ReportingParserException($"Can't find the column {requestColumn.Id} at dimension {dimension.Id}");
					else
					{
						RequestDimensionModel? convertedDimension = Search(dimension.Id);

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
	///		Solicitud a la que se asocian las dimensiones
	/// </summary>
	internal RequestModel Request { get; }
}