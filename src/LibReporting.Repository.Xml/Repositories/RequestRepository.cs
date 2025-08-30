using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Repository.Xml.Repositories;

/// <summary>
///		Repositorio para <see cref="ReportRequestModel"/>
/// </summary>
public class RequestRepository
{
	// Constantes privadas
	private const string TagRoot = "ReportRequest";
	private const string TagId = "Id";
	private const string TagDataWarehouseId = "DataWarehouseId";
	private const string TagParameter = "Parameter";
	private const string TagKey = "Key";
	private const string TagType = "Type";
	private const string TagValue = "Value";
	private const string TagExpression = "Expression";
	private const string TagColumn = "Column";
	private const string TagVisible = "Visible";
	private const string TagOrderIndex = "OrderIndex";
	private const string TagOrderBy = "OrderBy";
	private const string TagWhere = "Where";
	private const string TagHaving = "Having";
	private const string TagCondition = "Condition";
	private const string TagDimension = "Dimension";
	private const string TagDataSource = "DataSource";
	private const string TagPagination = "Pagination";
	private const string TagPage = "Page";
	private const string TagRecordsPerPage = "RecordsPerPage";

	/// <summary>
	///		Carga los datos de un <see cref="ReportRequestModel"/>
	/// </summary>
	public ReportRequestModel? Load(string fileName)
	{
		ReportRequestModel? request = null;
		MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);

			// Carga los datos del archivo
			if (fileML is not null)
				foreach (MLNode rootML in fileML.Nodes)
					if (rootML.Name == TagRoot)
					{
						// Crea el informe
						request = new ReportRequestModel(rootML.Attributes[TagDataWarehouseId].Value.TrimIgnoreNull(),
														 rootML.Attributes[TagId].Value.TrimIgnoreNull());
						// Carga los parámetros
						foreach (MLNode nodeML in rootML.Nodes)
							switch (nodeML.Name)
							{
								case TagParameter:
										LoadParameter(nodeML, request.Parameters);
									break;
								case TagDimension:
										request.Dimensions.Add(LoadDataRequest(nodeML));
									break;
								case TagDataSource:
										request.DataSources.Add(LoadDataRequest(nodeML));
									break;
								case TagExpression:
										ColumnRequestModel? column = LoadExpression(nodeML);

											if (column is not null)
												request.Expressions.Add(column);
									break;
								case TagColumn:
										request.Expressions.Add(LoadColumn(nodeML));
									break;
								case TagPagination:
										LoadPagination(request.Pagination, nodeML);
									break;
							}
					}
			// Devuelve los datos de la solicitud
			return request;
	}

	/// <summary>
	///		Carga los datos de paginación
	/// </summary>
	private void LoadPagination(PaginationRequestModel pagination, MLNode nodeML)
	{
		pagination.Page = nodeML.Attributes[TagPage].Value.GetInt(0);
		pagination.RecordsPerPage = nodeML.Attributes[TagRecordsPerPage].Value.GetInt(0);
		pagination.MustPaginate = pagination.Page > 0;
	}

	/// <summary>
	///		Carga una lista de <see cref="ColumnRequestModel"/>
	/// </summary>
	private List<ColumnRequestModel> LoadColumns(MLNode rootML)
	{
		List<ColumnRequestModel> columns = [];

			// Carga las columnas
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagColumn)
					columns.Add(LoadColumn(nodeML));
			// Devuelve la lista
			return columns;
	}

	/// <summary>
	///		Carga los datos de la expresión: esto es sólo para compatibilidad con las solicitudes antiguas
	/// </summary>
	//TODO Quitar esto
	private ColumnRequestModel? LoadExpression(MLNode rootML)
	{
		// Busca la primera columna
		foreach (MLNode nodeML in rootML.Nodes)
			if (nodeML.Name == TagColumn)
				return LoadColumn(nodeML);
		// Si ha llegado hasta aquí es porque no ha encontrado nada
		return null;
	}

	/// <summary>
	///		Carga los datos de la columna
	/// </summary>
	private ColumnRequestModel LoadColumn(MLNode rootML)
	{
		ColumnRequestModel column = new(rootML.Attributes[TagId].Value.TrimIgnoreNull());

			// Carga los datos de la columna
			column.Visible = rootML.Attributes[TagVisible].Value.GetBool();
			column.OrderIndex = rootML.Attributes[TagOrderIndex].Value.GetInt(0);
			column.OrderBy = rootML.Attributes[TagOrderBy].Value.GetEnum(ColumnRequestModel.SortOrder.Undefined);
			// Carga los filtros
			column.FiltersWhere.AddRange(LoadFilters(TagWhere, rootML));
			column.FiltersHaving.AddRange(LoadFilters(TagHaving, rootML));
			// Devuelve la columna
			return column;
	}

	/// <summary>
	///		Carga los datos de un origen de datos
	/// </summary>
	private DataRequestModel LoadDataRequest(MLNode rootML)
	{
		DataRequestModel dataSource = new(rootML.Attributes[TagId].Value.TrimIgnoreNull());

			// Añade las columnas
			dataSource.Columns.AddRange(LoadColumns(rootML));
			// Devuelve los datos
			return dataSource;
	}

	/// <summary>
	///		Carga los filtros asociados a una columna
	/// </summary>
	private List<FilterRequestModel> LoadFilters(string tag, MLNode rootML)
	{
		List<FilterRequestModel> filters = [];

			// Añade los datos de los filtros
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == tag)
				{
					FilterRequestModel filter = new();

						// Asigna los atributos
						filter.Condition = nodeML.Attributes[TagCondition].Value.GetEnum(FilterRequestModel.ConditionType.Undefined);
						// Asigna los valores
						foreach (MLNode childML in nodeML.Nodes)
							if (childML.Name == TagParameter)
								filter.Values.Add(ConvertParameter(childML.Attributes[TagType].Value.GetEnum(ParameterRequestModel.ParameterType.String),
																   childML.Attributes[TagValue].Value.TrimIgnoreNull()));																  
						// Añade el filtro a la colección
						if (filter.Condition != FilterRequestModel.ConditionType.Undefined)
							filters.Add(filter);
				}
			// Devuelve la colección de filtros
			return filters;
	}

	/// <summary>
	///		Carga los datos de un parámetro
	/// </summary>
	private void LoadParameter(MLNode rootML, List<ParameterRequestModel> parameters)
	{
		ParameterRequestModel parameter = new()
											{
												Key = rootML.Attributes[TagKey].Value.TrimIgnoreNull(), 
												Type = rootML.Attributes[TagType].Value.GetEnum(ParameterRequestModel.ParameterType.String)
											};

			// Obtiene el valor
			parameter.Value = ConvertParameter(parameter.Type, rootML.Attributes[TagValue].Value.TrimIgnoreNull());
			// Añade el parámetro a la lista
			parameters.Add(parameter);
	}

	/// <summary>
	///		Convierte el parámetro
	/// </summary>
	private object? ConvertParameter(ParameterRequestModel.ParameterType type, string value)
	{
		return type switch
				{
					ParameterRequestModel.ParameterType.Date => value.GetDateTime(),
					ParameterRequestModel.ParameterType.Integer or ParameterRequestModel.ParameterType.Decimal => value.GetDouble(),
					ParameterRequestModel.ParameterType.Boolean => value.GetBool(),
					_ => value
				};
	}

	/// <summary>
	///		Graba los datos de un <see cref="ReportRequestModel"/>
	/// </summary>
	public void Save(string fileName, ReportRequestModel request)
	{
		MLFile fileML = new();
		MLNode rootML = fileML.Nodes.Add(TagRoot);

			// Añade los atributos
			rootML.Attributes.Add(TagDataWarehouseId, request.DataWarehouseId);
			rootML.Attributes.Add(TagId, request.ReportId);
			// Añade los datos de la solicitud
			rootML.Nodes.Add(GetNodePagination(request.Pagination));
			rootML.Nodes.AddRange(GetNodesParameters(request.Parameters));
			rootML.Nodes.AddRange(GetNodesDataRequest(TagDimension, request.Dimensions));
			rootML.Nodes.AddRange(GetNodesDataRequest(TagDataSource, request.DataSources));
			rootML.Nodes.AddRange(GetNodesExpressions(request.Expressions));
			// Graba el archivo
			new LibMarkupLanguage.Services.XML.XMLWriter().Save(fileName, fileML);
	}

	/// <summary>
	///		Obtiene el nodo de paginación
	/// </summary>
	private MLNode GetNodePagination(PaginationRequestModel pagination)
	{
		MLNode nodeML = new(TagPagination);

			// Normaliza la página
			if (!pagination.MustPaginate)
				pagination.Page = 0;
			// Añade los atributos al nodo
			nodeML.Attributes.Add(TagPagination, pagination.Page);
			nodeML.Attributes.Add(TagRecordsPerPage, pagination.RecordsPerPage);
			// Devuelve el nodo
			return nodeML;
	}

	/// <summary>
	///		Obtiene los nodos XML de parámetros
	/// </summary>
	private MLNodesCollection GetNodesParameters(List<ParameterRequestModel> parameters)
	{
		MLNodesCollection nodesML = [];

			// Añade los parámetros
			foreach (ParameterRequestModel parameter in parameters)
				nodesML.Add(GetNodeParameter(parameter.Key, parameter.Type, parameter.Value));
			// Devuelve la colección de nodos
			return nodesML;
	}

	/// <summary>
	///		Obtiene el nodo de un parámetro
	/// </summary>
	private MLNode GetNodeParameter(string key, ParameterRequestModel.ParameterType type, object? value)
	{
		MLNode nodeML = new(TagParameter);

			// Asigna los datos
			nodeML.Attributes.Add(TagKey, key);
			nodeML.Attributes.Add(TagType, type.ToString());
			// Asigna el tipo y el valor
			switch (value)
			{
				case null:
						nodeML.Attributes.Add(TagValue, string.Empty);
					break;
				case string valueString:
						nodeML.Attributes.Add(TagValue, valueString);
					break;
				case DateTime valueDate:
						nodeML.Attributes.Add(TagValue, $"{valueDate:yyyy-MM-dd HH:mm:ss}");
					break;
				case bool valueBool:
						nodeML.Attributes.Add(TagValue, valueBool.ToString());
					break;
				default:
						nodeML.Attributes.Add(TagValue, (value as double?)?.ToString(System.Globalization.CultureInfo.InvariantCulture));
					break;
			}
			// Devuelve el nodo
			return nodeML;
	}

	/// <summary>
	///		Obtiene los nodos de los datos solicitados
	/// </summary>
	private MLNodesCollection GetNodesDataRequest(string tag, List<DataRequestModel> dataRequests)
	{
		MLNodesCollection nodesML = [];

			// Añade los nodos de datos solicitados
			foreach (DataRequestModel dataRequest in dataRequests)
			{
				MLNode nodeML = new(tag);

					// Añade los atributos de la dimensión
					nodeML.Attributes.Add(TagId, dataRequest.Id);
					// Añade las columnas
					nodeML.Nodes.AddRange(GetNodesColumns(dataRequest.Columns));
					// Añade el nodo a la colección
					nodesML.Add(nodeML);
			}
			// Devuelve los nodos
			return nodesML;
	}

	/// <summary>
	///		Obtiene los nodos de las expresiones
	/// </summary>
	private MLNodesCollection GetNodesExpressions(List<ColumnRequestModel> expressions)
	{
		MLNodesCollection nodesML = [];

			// Añade las columnas
			foreach (ColumnRequestModel expression in expressions)
				nodesML.Add(GetNodeColumn(expression));
			// Devuelve los nodos
			return nodesML;
	}

	/// <summary>
	///		Añade a un nodo los datos básicos de una columna
	/// </summary>
	private MLNodesCollection GetNodesColumns(List<ColumnRequestModel> columns)
	{
		MLNodesCollection nodesML = [];

			// Añade los atributos de la columna
			foreach (ColumnRequestModel column in columns)
				nodesML.Add(GetNodeColumn(column));
			// Devuelve los nodos
			return nodesML;
	}

	/// <summary>
	///		Añade a un nodo los datos básicos de una columna
	/// </summary>
	private MLNode GetNodeColumn(ColumnRequestModel column)
	{
		MLNode nodeML = new(TagColumn);

			// Añade los atributos de la columna
			nodeML.Attributes.Add(TagId, column.Id);
			nodeML.Attributes.Add(TagVisible, column.Visible);
			nodeML.Attributes.Add(TagOrderIndex, column.OrderIndex);
			nodeML.Attributes.Add(TagOrderBy, column.OrderBy.ToString());
			// Añade los filtros
			nodeML.Nodes.AddRange(GetNodesFilter(TagWhere, column.FiltersWhere));
			nodeML.Nodes.AddRange(GetNodesFilter(TagHaving, column.FiltersHaving));
			// Devuelve el nodo
			return nodeML;
	}

	/// <summary>
	///		Obtiene los nodos del filtro
	/// </summary>
	private MLNodesCollection GetNodesFilter(string tag, List<FilterRequestModel> filters)
	{
		MLNodesCollection nodesML = [];

			// Añade las condiciones
			foreach (FilterRequestModel filter in filters)
			{
				MLNode nodeML = new(tag);

					// Añade los atributos
					nodeML.Attributes.Add(TagCondition, filter.Condition.ToString());
					// Añade los valores del filtro
					foreach (object? value in filter.Values)
						nodeML.Nodes.Add(GetNodeParameter("Value", ParameterRequestModel.ParameterType.String, value));
					// Añade el nodo a la colección
					nodesML.Add(nodeML);
			}
			// Devuelve la colección de nodos
			return nodesML;
	}
}