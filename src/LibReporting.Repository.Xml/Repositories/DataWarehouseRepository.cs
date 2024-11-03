using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibReporting.Models;
using Bau.Libraries.LibReporting.Models.DataWarehouses;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;

namespace Bau.Libraries.LibReporting.Repository.Xml.Repositories;

/// <summary>
///		Repositorio de <see cref="ReportingSchemaModel"/> con archivos XML
/// </summary>
public class DataWarehouseRepository : BaseRepository, Application.Interfaces.ISchemaRepository
{
	// Constantes privadas
	private const string TagRoot = "Dashboard";
	private const string TagId = "Id";
	private const string TagName = "Name";
	private const string TagDescription = "Description";
	private const string TagColumn = "Column";
	private const string TagVisible = "Visible";
	private const string TagSourceId = "SourceId";
	private const string TagDataSourceTable = "DataSourceTable";
	private const string TagDataSourceSql = "DataSourceSql";
	private const string TagSql = "Sql";
	private const string TagIsView = "IsView";
	private const string TagDimension = "Dimension";
	private const string TagDimensionSourceId = "DimensionSourceId";
	private const string TagPrimaryKey = "IsPrimaryKey";
	private const string TagRelation = "Relation";
	private const string TagForeignKey = "ForeignKey";
	private const string TagDimensionColumn = "DimensionColumn";
	private const string TagColumnsPrefix = "ColumnsPrefix";
	private const string TagSchema = "Schema";
	private const string TagTable = "Table";
	private const string TagType = "Type";
	private const string TagAlias = "Alias";
	private const string TagRequired = "Required";
	private const string TagParameter = "Parameter";
	private const string TagValue = "Value";
	private const string TagFormula = "Formula";

	public DataWarehouseRepository(ReportingRepositoryXml manager) : base(manager) {}

	/// <summary>
	///		Carga los datos de esquema
	/// </summary>
	public async Task<DataWarehouseModel> GetAsync(string id, ReportingSchemaModel schema, CancellationToken cancellationToken)
	{
		// Evita las advertencias
		await Task.Delay(1, cancellationToken);
		// Carga los datos
		return Get(id, schema);
	}

	/// <summary>
	///		Carga los datos de esquema
	/// </summary>
	public DataWarehouseModel Get(string id, ReportingSchemaModel schema)
	{
		DataWarehouseModel dataWarehouse = new(schema);
		MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(id);

			// Carga los datos
			if (fileML is not null)
				foreach (MLNode rootML in fileML.Nodes)
					if (rootML.Name == TagRoot)
					{
						// Asigna las propiedades
						dataWarehouse.Id = id;
						dataWarehouse.Name = rootML.Nodes[TagName].Value.TrimIgnoreNull();
						dataWarehouse.Description = rootML.Nodes[TagDescription].Value.TrimIgnoreNull();
						// Carga las dimensiones y orígenes de datos
						foreach (MLNode nodeML in rootML.Nodes)
							switch (nodeML.Name)
							{
								case TagDataSourceTable:
										dataWarehouse.DataSources.Add(LoadDataSourceTable(dataWarehouse, nodeML));
									break;
								case TagDataSourceSql:
										dataWarehouse.DataSources.Add(LoadDataSourceSql(dataWarehouse, nodeML));
									break;
								case TagDimension:
										dataWarehouse.Dimensions.Add(LoadBaseDimension(nodeML, dataWarehouse));
									break;
								//case TagReport:
								//		dataWarehouse.Reports.Add(LoadReport(nodeML, dataWarehouse));
								//	break;
							}
						// Carga los informes avanzados
						AddAdvancedReports(dataWarehouse, Path.GetDirectoryName(id));
					}
			// Devuelve los datos del almacén de datos
			return dataWarehouse;
	}

	/// <summary>
	///		Añade los informes avanzados
	/// </summary>
	private void AddAdvancedReports(DataWarehouseModel dataWarehouse, string? path)
	{
		if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
			foreach (string fileName in Directory.GetFiles(path, "*.report.xml"))
			{
				ReportModel report = Manager.ReportRepository.Get(fileName, dataWarehouse);

					if (report is not null)
						dataWarehouse.Reports.Add(report);
			}
	}

	/// <summary>
	///		Obtiene un origen de datos para una tabla
	/// </summary>
	private DataSourceTableModel LoadDataSourceTable(DataWarehouseModel dataWarehouse, MLNode rootML)
	{
		DataSourceTableModel dataSource = new(dataWarehouse)
											{
												Schema = rootML.Attributes[TagSchema].Value.TrimIgnoreNull(),
												Table = rootML.Attributes[TagTable].Value.TrimIgnoreNull(),
												IsView = rootML.Attributes[TagIsView].Value.GetBool()
											};

			// El Id del origen de datos es el nombre completo de la tabla
			dataSource.Id = dataSource.FullName;
			// Carga las columnas
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagColumn)
					dataSource.Columns.Add(LoadColumn(dataSource, nodeML));
			// Devuelve el origen de datos
			return dataSource;
	}

	/// <summary>
	///		Obtiene un origen de datos para una consulta SQL
	/// </summary>
	private DataSourceSqlModel LoadDataSourceSql(DataWarehouseModel dataWarehouse, MLNode rootML)
	{
		DataSourceSqlModel dataSource = new(dataWarehouse)
											{
												Id = rootML.Attributes[TagId].Value.TrimIgnoreNull(),
												Sql = rootML.Nodes[TagSql].Value.TrimIgnoreNull()
											};

			// Carga las columnas
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagColumn)
					dataSource.Columns.Add(LoadColumn(dataSource, nodeML));
			// Carga los parámetros de consulta
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagParameter)
					dataSource.Parameters.Add(LoadParameter(nodeML));
			// Devuelve el origen de datos
			return dataSource;
	}

	/// <summary>
	///		Carga los datos de una columna
	/// </summary>
	private DataSourceColumnModel LoadColumn(BaseDataSourceModel dataSource, MLNode rootML)
	{
		return new DataSourceColumnModel(dataSource)
				{
					Id = rootML.Attributes[TagSourceId].Value.TrimIgnoreNull(),
					Alias = rootML.Attributes[TagAlias].Value.TrimIgnoreNull(),
					Type = rootML.Attributes[TagType].Value.GetEnum(DataSourceColumnModel.FieldType.Unknown),
					Required = rootML.Attributes[TagRequired].Value.GetBool(),
					IsPrimaryKey = rootML.Attributes[TagPrimaryKey].Value.GetBool(),
					Visible = rootML.Attributes[TagVisible].Value.GetBool(true),
					FormulaSql = rootML.Nodes[TagFormula].Value.TrimIgnoreNull()
				};
	}

	/// <summary>
	///		Carga los datos de un parámetros
	/// </summary>
	private DataSourceSqlParameterModel LoadParameter(MLNode rootML)
	{
		return new DataSourceSqlParameterModel
						{
							Name = rootML.Attributes[TagName].Value.TrimIgnoreNull(),
							Type = rootML.Attributes[TagType].Value.GetEnum(DataSourceColumnModel.FieldType.Unknown),
							DefaultValue = rootML.Attributes[TagValue].Value.TrimIgnoreNull()
						};
	}

	/// <summary>
	///		Carga los datos de una dimension
	/// </summary>
	private BaseDimensionModel LoadBaseDimension(MLNode rootML, DataWarehouseModel dataWarehouse)
	{
		if (string.IsNullOrWhiteSpace(rootML.Attributes[TagDimensionSourceId].Value))
			return LoadDimension(rootML, dataWarehouse);
		else
			return LoadDimensionChild(rootML, dataWarehouse);
	}

	/// <summary>
	///		Carga los datos de una dimensión
	/// </summary>
	private DimensionModel LoadDimension(MLNode rootML, DataWarehouseModel dataWarehouse)
	{
		BaseDataSourceModel? dataSource = dataWarehouse.DataSources[rootML.Attributes[TagSourceId].Value.TrimIgnoreNull()];

			if (dataSource is null)
				throw new ArgumentException($"Can't find the datasource {rootML.Attributes[TagSourceId].Value.TrimIgnoreNull()} at datawarehouse {dataWarehouse.Name}");
			else
			{
				DimensionModel dimension = new(dataWarehouse, dataSource)
												{
													Id = rootML.Attributes[TagId].Value.TrimIgnoreNull(),
													Description = rootML.Nodes[TagDescription].Value.TrimIgnoreNull()
												};

					// Carga las dimensiones hija
					foreach (MLNode nodeML in rootML.Nodes)
						if (nodeML.Name == TagRelation)
							dimension.Relations.Add(LoadRelatedDimension(nodeML, dataWarehouse));
					// Devuelve la dimensión
					return dimension;
			}
	}

	/// <summary>
	///		Carga los datos de una dimensión
	/// </summary>
	private DimensionChildModel LoadDimensionChild(MLNode rootML, DataWarehouseModel dataWarehouse)
	{
		return new DimensionChildModel(dataWarehouse,
									   rootML.Attributes[TagId].Value.TrimIgnoreNull(),
									   rootML.Attributes[TagDimensionSourceId].Value.TrimIgnoreNull(),
									   rootML.Attributes[TagColumnsPrefix].Value.TrimIgnoreNull()
									  )
						{
							Description = rootML.Nodes[TagDescription].Value.TrimIgnoreNull()
						};
	}

	/// <summary>
	///		Carga los datos de una dimension relacionada
	/// </summary>
	private DimensionRelationModel LoadRelatedDimension(MLNode rootML, DataWarehouseModel dataWarehouse)
	{
		DimensionRelationModel relation = new(dataWarehouse)
												{
													DimensionId = rootML.Attributes[TagSourceId].Value.TrimIgnoreNull()
												};

			// Carga las columnas
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagForeignKey)
					relation.ForeignKeys.Add(new RelationForeignKey
														{
															ColumnId = nodeML.Attributes[TagColumn].Value.TrimIgnoreNull(),
															TargetColumnId = nodeML.Attributes[TagDimensionColumn].Value.TrimIgnoreNull()
														}
											);
			// Devuelve la relación
			return relation;
	}

	/// <summary>
	///		Modifica los datos de esquema
	/// </summary>
	public async Task UpdateAsync(string id, DataWarehouseModel dataWarehouse, CancellationToken cancellationToken)
	{
		// Evita las advertencias
		await Task.Delay(1, cancellationToken);
		// Graba los datos
		Update(id, dataWarehouse);
	}

	/// <summary>
	///		Modifica los datos de esquema
	/// </summary>
	public void Update(string id, DataWarehouseModel dataWarehouse)
	{
		MLFile fileML = new();
		MLNode rootML = fileML.Nodes.Add(TagRoot);

			// Añade las propiedades básicas
			rootML.Nodes.Add(TagName, dataWarehouse.Name);
			rootML.Nodes.Add(TagDescription, dataWarehouse.Description);
			// Añade los nodos de orígenes de datos
			foreach (BaseDataSourceModel baseDataSource in dataWarehouse.DataSources.EnumerateValues())
				switch (baseDataSource)
				{
					case DataSourceTableModel dataSource:
							rootML.Nodes.Add(GetNodeDataSourceTable(dataSource));
						break;
					case DataSourceSqlModel dataSource:
							rootML.Nodes.Add(GetNodeDataSourceSql(dataSource));
						break;
				}
			// Añade los nodos de dimensión
			foreach (BaseDimensionModel baseDimension in dataWarehouse.Dimensions.EnumerateValues())
				switch (baseDimension)
				{
					case DimensionModel dimension:
							rootML.Nodes.Add(GetNodeDimension(dimension));
						break;
					case DimensionChildModel dimension:
							rootML.Nodes.Add(GetNodeDimensionChild(dimension));
						break;
				}
			// Graba el archivo
			new LibMarkupLanguage.Services.XML.XMLWriter().Save(id, fileML);
	}

	/// <summary>
	///		Obtiene el nodo de un origen de datos
	/// </summary>
	private MLNode GetNodeDataSourceTable(DataSourceTableModel dataSource)
	{
		MLNode nodeML = new(TagDataSourceTable);

			// Asigna las propiedades
			nodeML.Attributes.Add(TagId, dataSource.FullName);
			nodeML.Attributes.Add(TagSchema, dataSource.Schema);
			nodeML.Attributes.Add(TagTable, dataSource.Table);
			nodeML.Attributes.Add(TagIsView, dataSource.IsView);
			// Añade las columnas visibles
			nodeML.Nodes.AddRange(GetNodesColumns(dataSource.Columns));
			// Devuelve el nodo
			return nodeML;
	}

	/// <summary>
	///		Obtiene el nodo de un origen de datos
	/// </summary>
	private MLNode GetNodeDataSourceSql(DataSourceSqlModel dataSource)
	{
		MLNode nodeML = new(TagDataSourceSql);

			// Asigna las propiedades
			nodeML.Attributes.Add(TagId, dataSource.Id);
			nodeML.Nodes.Add(TagSql, dataSource.Sql);
			// Añade las columnas y los parámetros
			nodeML.Nodes.AddRange(GetNodesColumns(dataSource.Columns));
			nodeML.Nodes.AddRange(GetNodesParameters(dataSource.Parameters));
			// Devuelve el nodo
			return nodeML;
	}

	/// <summary>
	///		Obtiene los nodos de una serie de parámetros
	/// </summary>
	private MLNodesCollection GetNodesParameters(List<DataSourceSqlParameterModel> parameters)
	{
		MLNodesCollection nodesML = new();

			// Añade los parámetros
			foreach (DataSourceSqlParameterModel parameter in parameters)
			{
				MLNode nodeML = nodesML.Add(TagParameter);

					// Asigna los atributos
					nodeML.Attributes.Add(TagName, parameter.Name);
					nodeML.Attributes.Add(TagType, parameter.Type.ToString());
					nodeML.Attributes.Add(TagValue, parameter.DefaultValue);
			}
			// Devuelve la colección de nodos
			return nodesML;
	}

	/// <summary>
	///		Obtiene los nodos de las columnas
	/// </summary>
	private MLNodesCollection GetNodesColumns(Models.Base.BaseReportingDictionaryModel<DataSourceColumnModel> columns)
	{
		MLNodesCollection nodesML = new();

			// Añade los nodos
			foreach (DataSourceColumnModel column in columns.EnumerateValues())
			{
				MLNode nodeML = nodesML.Add(TagColumn);

					// Añade los datos
					nodeML.Attributes.AddIfNotEmpty(TagAlias, column.Alias);
					nodeML.Attributes.Add(TagSourceId, column.Id);
					nodeML.Attributes.Add(TagPrimaryKey, column.IsPrimaryKey);
					nodeML.Attributes.Add(TagVisible, column.Visible);
					nodeML.Attributes.Add(TagType, column.Type.ToString());
					nodeML.Attributes.Add(TagRequired, column.Required);
					nodeML.Nodes.AddIfNotEmpty(TagFormula, column.FormulaSql);
			}
			// Devuelve la colección de nodos
			return nodesML;
	}

	/// <summary>
	///		Obtiene el nodo de una dimensión
	/// </summary>
	private MLNode GetNodeDimension(DimensionModel dimension)
	{
		MLNode nodeML = new(TagDimension);

			// Asigna las propiedades
			nodeML.Attributes.Add(TagId, dimension.Id);
			nodeML.Nodes.Add(TagDescription, dimension.Description);
			nodeML.Attributes.Add(TagSourceId, dimension.DataSource.Id);
			// Añade las relaciones hija
			foreach (DimensionRelationModel relation in dimension.Relations)
				if (relation.Dimension != null)
					nodeML.Nodes.Add(GetNodeRelation(relation));
			// Devuelve el nodo
			return nodeML;
	}

	/// <summary>
	///		Obtiene el nodo de una dimensión
	/// </summary>
	private MLNode GetNodeDimensionChild(DimensionChildModel dimension)
	{
		MLNode nodeML = new(TagDimension);

			// Asigna las propiedades
			nodeML.Attributes.Add(TagId, dimension.Id);
			nodeML.Attributes.Add(TagDimensionSourceId, dimension.SourceDimensionId);
			nodeML.Attributes.Add(TagColumnsPrefix, dimension.ColumnsPrefix);
			// Devuelve el nodo
			return nodeML;
	}

	/// <summary>
	///		Obtiene un nodo de relación
	/// </summary>
	private MLNode GetNodeRelation(DimensionRelationModel relation)
	{
		MLNode rootML = new(TagRelation);

			// Añade los atributos
			if (relation.Dimension is not null)
				rootML.Attributes.Add(TagSourceId, relation.Dimension.Id);
			// Añade las claves foráneas
			foreach (RelationForeignKey foreignKey in relation.ForeignKeys)
			{
				MLNode nodeML = rootML.Nodes.Add(TagForeignKey);

					nodeML.Attributes.Add(TagColumn, foreignKey.ColumnId);
					nodeML.Attributes.Add(TagDimensionColumn, foreignKey.TargetColumnId);
			}
			// Devuelve el nodo
			return rootML;
	}

}
