using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Repository.Xml.Repositories;

/// <summary>
///		Repositorio para <see cref="ReportRequestModel"/>
/// </summary>
public class RequestRepository : BaseRepository, Application.Interfaces.IRequestRepository
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
	private const string TagAggregatedBy = "AggregatedBy";
	private const string TagVisible = "Visible";
	private const string TagOrderIndex = "OrderIndex";
	private const string TagOrderBy = "OrderBy";
	private const string TagWhere = "Where";
	private const string TagHaving = "Having";
	private const string TagCondition = "Condition";
	private const string TagDimension = "Dimension";
	private const string TagDataSource = "DataSource";

	public RequestRepository(ReportingRepositoryXml manager) : base(manager) {}

	/// <summary>
	///		Carga los datos de un <see cref="ReportRequestModel"/>
	/// </summary>
	public async Task<ReportRequestModel?> GetAsync(string id, CancellationToken cancellationToken)
	{
		// Evita las advertencias
		await Task.Delay(1, cancellationToken);
		// Carga los datos
		return Get(id);
	}

	/// <summary>
	///		Carga los datos de un <see cref="ReportRequestModel"/>
	/// </summary>
	public ReportRequestModel? Get(string id)
	{
		ReportRequestModel? request = null;
		MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(id);

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
										request.Dimensions.Add(LoadDimension(nodeML));
									break;
								case TagDataSource:
										request.DataSources.Add(LoadDataSource(nodeML));
									break;
								case TagExpression:
										request.Expressions.AddRange(LoadExpressions(nodeML));
									break;
							}
					}
			// Devuelve los datos de la solicitud
			return request;
	}

	/// <summary>
	///		Carga los datos de una dimensión
	/// </summary>
	private DimensionRequestModel LoadDimension(MLNode rootML)
	{
		DimensionRequestModel dimension = new();

			// Añade los atributos de la dimensión
			dimension.DimensionId = rootML.Attributes[TagId].Value.TrimIgnoreNull();
			// Carga las columnas hija
			dimension.Columns.AddRange(LoadDimensionColumns(rootML));
			// Devuelve los datos
			return dimension;
	}

	/// <summary>
	///		Carga una lista de <see cref="DimensionColumnRequestModel"/>
	/// </summary>
	private List<DimensionColumnRequestModel> LoadDimensionColumns(MLNode rootML)
	{
		List<DimensionColumnRequestModel> columns = new();

			// Carga las columnas
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagColumn)
				{
					DimensionColumnRequestModel column = new();

						// Asigna las propiedades
						column.ColumnId = nodeML.Attributes[TagId].Value.TrimIgnoreNull();
						// Asigna las propiedades básicas
						LoadBaseColumnData(column, nodeML);
						// Añade la columna
						columns.Add(column);
				}
			// Devuelve la lista
			return columns;
	}

	/// <summary>
	///		Carga los datos de un origen de datos
	/// </summary>
	private DataSourceRequestModel LoadDataSource(MLNode rootML)
	{
		DataSourceRequestModel dataSource = new();

			// Asigna los datos de la expresión
			dataSource.ReportDataSourceId = rootML.Attributes[TagId].Value.TrimIgnoreNull();
			// Añade las columnas
			dataSource.Columns.AddRange(LoadDataSourceColumns(rootML));
			// Devuelve los datos
			return dataSource;
	}

	/// <summary>
	///		Carga las columnas de un origen de datos
	/// </summary>
	private List<DataSourceColumnRequestModel> LoadDataSourceColumns(MLNode rootML)
	{
		List<DataSourceColumnRequestModel> columns = new();

			// Carga las columnas
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagColumn)
				{
					DataSourceColumnRequestModel column = new();

						// Carga las propiedades
						column.ColumnId = nodeML.Attributes[TagId].Value.TrimIgnoreNull();
						// Carga los datos básicos
						LoadBaseColumnData(column, nodeML);
						// Añade la columna a la colección
						columns.Add(column);
				}
			// Devuelve las columnas
			return columns;
	}

	/// <summary>
	///		Carga las columnas de una expresión
	/// </summary>
	private List<ExpressionColumnRequestModel> LoadExpressions(MLNode rootML)
	{
		List<ExpressionColumnRequestModel> columns = [];

			// Carga las columnas
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagColumn)
				{
					ExpressionColumnRequestModel column = new();

						// Carga las propiedades
						column.ColumnId = nodeML.Attributes[TagId].Value.TrimIgnoreNull();
						// Añade la columna a la colección
						columns.Add(column);
				}
			// Devuelve las columnas
			return columns;
	}

	/// <summary>
	///		Carga los datos base de una columna
	/// </summary>
	private void LoadBaseColumnData(BaseColumnRequestModel column, MLNode rootML)
	{
		// Carga los datos de la columna
		column.Visible = rootML.Attributes[TagVisible].Value.GetBool();
		column.OrderIndex = rootML.Attributes[TagOrderIndex].Value.GetInt(0);
		column.OrderBy = rootML.Attributes[TagOrderBy].Value.GetEnum(BaseColumnRequestModel.SortOrder.Undefined);
		// Carga los filtros
		column.FiltersWhere.AddRange(LoadFilters(TagWhere, rootML));
		column.FiltersHaving.AddRange(LoadFilters(TagHaving, rootML));
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
	public async Task UpdateAsync(string id, ReportRequestModel request, CancellationToken cancellationToken)
	{
		// Evita las advertencias
		await Task.Delay(1, cancellationToken);
		// Graba los datos
		Update(id, request);
	}

	/// <summary>
	///		Graba los datos de un <see cref="ReportRequestModel"/>
	/// </summary>
	public void Update(string id, ReportRequestModel request)
	{
		MLFile fileML = new();
		MLNode rootML = fileML.Nodes.Add(TagRoot);

			// Añade los atributos
			rootML.Attributes.Add(TagDataWarehouseId, request.DataWarehouseId);
			rootML.Attributes.Add(TagId, request.ReportId);
			// Añade los datos de la solicitud
			rootML.Nodes.AddRange(GetNodesParameters(request.Parameters));
			rootML.Nodes.AddRange(GetNodesDimensions(request.Dimensions));
			rootML.Nodes.AddRange(GetNodesDataSources(request.DataSources));
			rootML.Nodes.AddRange(GetNodesExpressions(request.Expressions));
			// Graba el archivo
			new LibMarkupLanguage.Services.XML.XMLWriter().Save(id, fileML);
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
	///		Obtiene los nodos de las dimensions
	/// </summary>
	private MLNodesCollection GetNodesDimensions(List<DimensionRequestModel> dimensions)
	{
		MLNodesCollection nodesML = [];

			// Añade los nodos de dimensión
			foreach (DimensionRequestModel dimension in dimensions)
			{
				MLNode nodeML = new(TagDimension);

					// Añade los atributos de la dimensión
					nodeML.Attributes.Add(TagId, dimension.DimensionId);
					// Añade las columnas
					nodeML.Nodes.AddRange(GetNodesDimensionColumns(dimension.Columns));
					// Añade el nodo a la colección
					nodesML.Add(nodeML);
			}
			// Devuelve los nodos
			return nodesML;
	}

	/// <summary>
	///		Añade los nodos de las columnas de dimensión
	/// </summary>
	private MLNodesCollection GetNodesDimensionColumns(List<DimensionColumnRequestModel> columns)
	{
		MLNodesCollection nodesML = [];

			// Añade los nodos de columnas
			foreach (DimensionColumnRequestModel column in columns)
			{
				MLNode nodeML = new(TagColumn);

					// Añade los atributos
					nodeML.Attributes.Add(TagId, column.ColumnId);
					// Añade los atributos básicos de la columna
					GetBaseColumnAttributes(column, nodeML);
					// Añade el nodo a la colección
					nodesML.Add(nodeML);
			}
			// Devuelve la colección de nodos
			return nodesML;
	}
	
	/// <summary>
	///		Obtiene los nodos de los orígenes de datos
	/// </summary>
	private MLNodesCollection GetNodesDataSources(List<DataSourceRequestModel> dataSources)
	{
		MLNodesCollection nodesML = [];

			// Añade los nodos de origen de datos
			foreach (DataSourceRequestModel dataSource in dataSources)
			{
				MLNode nodeML = new(TagDataSource);

					// Añade los atributos
					nodeML.Attributes.Add(TagId, dataSource.ReportDataSourceId);
					// Añade las columnas
					foreach (DataSourceColumnRequestModel column in dataSource.Columns)
					{
						MLNode columnML = nodeML.Nodes.Add(TagColumn);

							// Añade los valores
							columnML.Attributes.Add(TagId, column.ColumnId);
							// Añade los atributos base de la columna
							GetBaseColumnAttributes(column, columnML);
					}
					// Añade los datos del nodo a la colección
					nodesML.Add(nodeML);
			}
			// Devuelve los nodos
			return nodesML;
	}

	/// <summary>
	///		Obtiene los nodos de las expresiones
	/// </summary>
	private MLNodesCollection GetNodesExpressions(List<ExpressionColumnRequestModel> expressions)
	{
		MLNodesCollection nodesML = [];

			// Añade las columnas
			foreach (ExpressionColumnRequestModel column in expressions)
			{
				MLNode columnML = nodesML.Add(TagColumn);

					// Añade los valores
					columnML.Attributes.Add(TagId, column.ColumnId);
			}
			// Devuelve los nodos
			return nodesML;
	}

	/// <summary>
	///		Añade a un nodo los datos básicos de una columna
	/// </summary>
	private void GetBaseColumnAttributes(BaseColumnRequestModel column, MLNode columnML)
	{
		// Añade los atributos de la columna
		columnML.Attributes.Add(TagVisible, column.Visible);
		columnML.Attributes.Add(TagOrderIndex, column.OrderIndex);
		columnML.Attributes.Add(TagOrderBy, column.OrderBy.ToString());
		// Añade los filtros
		columnML.Nodes.AddRange(GetNodesFilter(TagWhere, column.FiltersWhere));
		columnML.Nodes.AddRange(GetNodesFilter(TagHaving, column.FiltersHaving));
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