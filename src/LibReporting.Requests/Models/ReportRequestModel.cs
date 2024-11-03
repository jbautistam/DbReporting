namespace Bau.Libraries.LibReporting.Requests.Models;

/// <summary>
///		Clase con los datos de solicitud de informe
/// </summary>
public class ReportRequestModel
{
	/// <summary>
	///		Clona los datos del objeto
	/// </summary>
	public ReportRequestModel Clone()
	{
		ReportRequestModel cloned = new()
										{
											ReportId = ReportId
										};

			// Clona las solicitudes de dimensiones
			foreach (DimensionRequestModel dimensionRequest in Dimensions)
				cloned.Dimensions.Add(dimensionRequest.Clone());
			// Clona las solicitudes de orígenes de datos
			foreach (DataSourceRequestModel dataSourceRequest in DataSources)
				cloned.DataSources.Add(dataSourceRequest.Clone());
			// Clona las solicitudes de expresiones
			foreach (ExpressionColumnRequestModel expressionRequest in Expressions)
				cloned.Expressions.Add(expressionRequest.Clone());
			// Clona la paginación
			cloned.Pagination.Clone(Pagination);
			// Devuelve los datos clonados
			return cloned;
	}

	/// <summary>
	///		Obtiene los datos de solicitud de una dimensión
	/// </summary>
	public DimensionRequestModel? GetDimensionRequest(string dimensionKey)
	{
		// Busca la dimensión entre las solicitudes
		foreach (DimensionRequestModel request in Dimensions)
			if (request.DimensionId.Equals(dimensionKey, StringComparison.CurrentCultureIgnoreCase))
				return request;
		// Si ha llegado hasta aquí es porque no ha encontrado nada
		return null;
	}

	/// <summary>
	///		Comprueba si se ha solicitado una dimensión
	/// </summary>
	public bool IsRequestedDimension(string dimensionKey) => IsRequestedDimension([ dimensionKey ]);

	/// <summary>
	///		Comprueba si se ha solicitado alguna dimensión
	/// </summary>
	public bool IsRequestedDimension(List<string>? dimensionsKey)
	{
		// Comprueba si se ha solicitado alguna de las dimensiones
		if (dimensionsKey is not null)
			foreach (string key in dimensionsKey)
				foreach (DimensionRequestModel request in Dimensions)
					if (request.DimensionId.Equals(key, StringComparison.CurrentCultureIgnoreCase))
						return true;
		// Si ha llegado hasta aquí es porque no se ha encontrado ninguna de las dimensiones
		return false;
	}

	/// <summary>
	///		Comprueba si se ha solicitado una expresión
	/// </summary>
	public bool IsRequestedExpression(string expressionKey) => IsRequestedExpression(expressionKey);

	/// <summary>
	///		Comprueba si se ha solicitado alguna expresión
	/// </summary>
	public bool IsRequestedExpression(List<string> expressionKeys)
	{
		// Comprueba si se ha solicitado alguna de las expresiones
		if (expressionKeys is not null)
			foreach (string key in expressionKeys)
				foreach (ExpressionColumnRequestModel expression in Expressions)
					if (expression.ColumnId.Equals(key, StringComparison.CurrentCultureIgnoreCase))
						return true;
		// Si se ha llegado hasta aquí es porque no se ha encontrado ninguna de las expresiones
		return false;
	}

	/// <summary>
	///		Añade una dimensión y un campo a una solicitud
	/// </summary>
	public void Add(string dimensionId, string columnId, bool requestedByUser)
	{
		DimensionRequestModel? dimension = GetDimensionRequest(dimensionId);

			// Añade la dimensión si no se había solicitado
			if (dimension is null)
			{
				// Crea la dimensión
				dimension = new()
								{
									DimensionId = dimensionId
								};
				// Añade la dimensión a la solicitud
				Dimensions.Add(dimension);
			}
			// Añade la columna
			if (dimension.GetRequestColumn(columnId) is null)
				dimension.Columns.Add(new()
										{
											ColumnId = columnId,
											RequestedByUser = requestedByUser
										}
									 );
	}

	/// <summary>
	///		Obtiene la solicitud asociada a un origen de datos
	/// </summary>
	public DataSourceRequestModel? GetDataSourceRequest(string id)
	{
		return DataSources.FirstOrDefault(item => item.ReportDataSourceId.Equals(id, StringComparison.CurrentCultureIgnoreCase));
	}

	/// <summary>
	///		Obtiene la solicitud asociada a una expresión
	/// </summary>
	public ExpressionColumnRequestModel? GetExpressionRequest(string expression)
	{
		// Busca la expresión entre las solicitudes
		foreach (ExpressionColumnRequestModel columnRequest in Expressions)
			if (columnRequest.ColumnId.Equals(expression, StringComparison.CurrentCultureIgnoreCase))
				return columnRequest;
		// Si ha llegado hasta aquí es porque no ha encontrado nada
		return null;
	}

	/// <summary>
	///		Indica si se han solicitado totales (estamos en la primera página)
	/// </summary>
	public bool IsRequestedTotals() => Pagination.IsFirstPage;

	/// <summary>
	///		Código de informe solicitado
	/// </summary>
	public string ReportId { get; set; } =  string.Empty;

	/// <summary>
	///		Parámetros
	/// </summary>
	public List<ParameterRequestModel> Parameters { get; } = [];

	/// <summary>
	///		Dimensiones solicitadas
	/// </summary>
	public List<DimensionRequestModel> Dimensions { get; } = [];

	/// <summary>
	///		Solicitudes de orígenes de datos
	/// </summary>
	public List<DataSourceRequestModel> DataSources { get; } = [];

	/// <summary>
	///		Expresiones solicitadas
	/// </summary>
	public List<ExpressionColumnRequestModel> Expressions { get; } = [];

	/// <summary>
	///		Paginación
	/// </summary>
	public PaginationRequestModel Pagination { get; } = new();
}