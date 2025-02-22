using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibReporting.Models.DataWarehouses;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;

namespace Bau.Libraries.LibReporting.Repository.Xml.Repositories;

/// <summary>
///		Repositorio de <see cref="ReportModel"/> con archivos XML
/// </summary>
public class ReportRepository : BaseRepository, Application.Interfaces.IReportRepository
{
	// Constantes privadas
	private const string TagRoot = "Report";
	private const string TagName = "Name";
	private const string TagDescription = "Description";
	private const string TagParameter = "Parameter";
	private const string TagType = "Type";
	private const string TagDefault = "Default";
	private const string TagBlocks = "Blocks";
	private const string TagCte = "Cte";
	private const string TagExecute = "Execute";
	private const string TagQuery = "Query";
	private const string TagWith = "With";
	private const string TagDimension = "Dimension";
	private const string TagIfRequest = "IfRequest";
	private const string TagThen = "Then";
	private const string TagElse = "Else";
	private const string TagAllDimensions = "AllDimensions";
	private const string TagAllExpressions = "AllExpressions";
	private const string TagWhenTotals = "WhenTotals";
	private const string TagRelation = "Relation";
	private const string TagTable = "Table";
	private const string TagField = "Field";
	private const string TagAlias = "Alias";
	private const string TagRequired = "Required";
	private const string TagFilter = "Filter";
	private const string TagRequests = "Requests";
	private const string TagDataSource = "DataSource";
	private const string TagSourceId = "SourceId";
	private const string TagSchema = "Schema";
	private const string TagExpression = "Expression";
	private const string TagForeignKey = "ForeignKey";
	private const string TagColumn = "Column";
	private const string TagDimensionColumn = "DimensionColumn";

	public ReportRepository(ReportingRepositoryXml manager) : base(manager) {}

	/// <summary>
	///		Carga el informe de un archivo sobre un <see cref="DataWarehouseModel"/>
	/// </summary>
	public async Task<ReportModel> GetAsync(string id, DataWarehouseModel dataWarehouse, CancellationToken cancellationToken)
	{
		// Evita las advertencias
		await Task.Delay(1, cancellationToken);
		// Carga los datos
		return Get(id, dataWarehouse);
	}

	/// <summary>
	///		Carga el informe de un archivo sobre un <see cref="DataWarehouseModel"/>
	/// </summary>
	public ReportModel Get(string id, DataWarehouseModel dataWarehouse)
	{
		ReportModel report = new(dataWarehouse, id);
		MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(report.FileName);

			// Carga los datos del informe
			foreach (MLNode rootML in fileML.Nodes)
				if (rootML.Name == TagRoot)
					foreach (MLNode nodeML in rootML.Nodes)
						switch (nodeML.Name)
						{
							case TagName:
									report.Id = nodeML.Value.TrimIgnoreNull();
								break;
							case TagDescription:
									report.Description = nodeML.Value.TrimIgnoreNull();
								break;
							case TagParameter:
									report.Parameters.Add(LoadParameter(nodeML));
								break;
							case TagBlocks:
									report.Blocks.AddRange(LoadBlocks(nodeML.Nodes));
								break;
							case TagDimension:
									LoadDimension(report, nodeML);
								break;
							case TagDataSource:
									LoadDataSource(report, nodeML);
								break;
							case TagRequests:
									report.RequestDimensions.AddRange(LoadRequests(nodeML));
								break;
							case TagExpression:
									report.Expressions.AddRange(LoadExpressions(report, nodeML));
								break;
						}
			// Devuelve el informe
			return report;
	}

	/// <summary>
	///		Carga una dimensión
	/// </summary>
	private void LoadDimension(ReportModel report, MLNode rootML)
	{
		string key = rootML.Attributes[TagName].Value.TrimIgnoreNull();
		BaseDimensionModel? dimension = report.DataWarehouse.Dimensions[key];

			if (dimension is not null)
				report.Dimensions.Add(dimension);
	}

	/// <summary>
	///		Carga los datos de un parámeto de un nodo
	/// </summary>
	private ReportParameterModel LoadParameter(MLNode rootML)
	{
		return new ReportParameterModel
						{
							Id = rootML.Attributes[TagName].Value.TrimIgnoreNull(),
							Type = rootML.Attributes[TagType].Value.GetEnum(LibReporting.Models.DataWarehouses.DataSets.DataSourceColumnModel.FieldType.Unknown),
							DefaultValue = rootML.Attributes[TagDefault].Value.TrimIgnoreNull()
						};
	}

	/// <summary>
	///		Carga una serie de bloques
	/// </summary>
	private List<BaseBlockModel> LoadBlocks(MLNodesCollection nodesML)
	{
		List<BaseBlockModel> blocks = [];

			// Carga los bloques de los nodos
			foreach (MLNode nodeML in nodesML)
				switch (nodeML.Name)
				{
					case TagWith:
							blocks.Add(LoadBlockWith(nodeML));
						break;
					case TagCte:
							blocks.Add(LoadBlockCte(nodeML));
						break;
					case TagExecute:
							blocks.Add(LoadBlockExecute(nodeML));
						break;
					case TagQuery:
							blocks.Add(LoadBlockQuery(nodeML));
						break;
					case TagIfRequest:
							blocks.Add(LoadBlockIfRequest(nodeML));
						break;
				}
			// Devuelve la colección de bloques
			return blocks;
	}

	/// <summary>
	///		Carga un bloque de creación de una sentencia WITH de SQL
	/// </summary>
	private BaseBlockModel LoadBlockWith(MLNode rootML)
	{
		BlockWithModel with = new(rootML.Attributes[TagName].Value.TrimIgnoreNull());

			// Carga los nodos
			with.Blocks.AddRange(LoadBlocks(rootML.Nodes));
			// Devuelve la instrucción
			return with;
	}

	/// <summary>
	///		Carga un bloque de consulta
	/// </summary>
	private BlockQueryModel LoadBlockQuery(MLNode rootML)
	{
		BlockQueryModel query = new(rootML.Attributes[TagName].Value.TrimIgnoreNull());

			// Carga la consulta
			query.Sql = rootML.Value.TrimIgnoreNull();
			// Devuelve la consulta
			return query;
	}

	/// <summary>
	///		Carga un bloque de ejecución
	/// </summary>
	private BlockExecutionModel LoadBlockExecute(MLNode rootML)
	{
		return new BlockExecutionModel(rootML.Attributes[TagName].Value.TrimIgnoreNull())
						{
							Sql = rootML.Value
						};
	}

	/// <summary>
	///		Carga un bloque de Cte
	/// </summary>
	private BaseBlockModel LoadBlockCte(MLNode rootML)
	{
		if (!string.IsNullOrWhiteSpace(rootML.Attributes[TagDimension].Value.TrimIgnoreNull()))
			return LoadBlockCteDimension(rootML);
		else
		{
			BlockCreateCteModel block = new(rootML.Attributes[TagName].Value.TrimIgnoreNull());

				// Carga los datos de la consulta
				if (rootML.Nodes.Count == 0)
					block.Blocks.Add(LoadBlockQuery(rootML));
				else
					block.Blocks.AddRange(LoadBlocks(rootML.Nodes));
				// Devuelve los datos del bloque
				return block;
		}
	}

	/// <summary>
	///		Carga un bloque de creación de CTE para una dimensión
	/// </summary>
	private BaseBlockModel LoadBlockCteDimension(MLNode rootML)
	{
		BlockCreateCteDimensionModel block = new(rootML.Attributes[TagName].Value.TrimIgnoreNull())
												{ 
													DimensionKey = rootML.Attributes[TagDimension].Value.TrimIgnoreNull(),
													Required = rootML.Attributes[TagRequired].Value.GetBool()
												};

			// Carga los objetos asociados
			foreach (MLNode nodeML in rootML.Nodes)
				switch (nodeML.Name)
				{
					case TagField:
							block.Fields.Add(LoadField(nodeML));
						break;
					case TagFilter:
							block.Filters.Add(LoadFilter(nodeML));
						break;
				}
			// Devuelve el bloque
			return block;
	}

	/// <summary>
	///		Carga un campo adicional para una consulta
	/// </summary>
	private ClauseFieldModel LoadField(MLNode rootML)
	{
		return new ClauseFieldModel
							{
								Field =	rootML.Attributes[TagName].Value.TrimIgnoreNull(),
								Alias = rootML.Attributes[TagAlias].Value.TrimIgnoreNull()
							};
	}

	/// <summary>
	///		Carga un filtro adicional para una consulta
	/// </summary>
	private ClauseFilterModel LoadFilter(MLNode rootML)
	{
		return new ClauseFilterModel
							{
								Sql = rootML.Value.TrimIgnoreNull()
							};
	}

	/// <summary>
	///		Carga un bloque que comprueba si se ha solicitado una (o varias) dimensiones
	/// </summary>
	private BlockIfRequest LoadBlockIfRequest(MLNode rootML)
	{
		BlockIfRequest block = new(string.Empty)
									{
										AllDimensions = rootML.Attributes[TagAllDimensions].Value.GetBool(),
										AllExpressions = rootML.Attributes[TagAllExpressions].Value.GetBool(),
										WhenTotals = rootML.Attributes[TagWhenTotals].Value.GetBool()
									};

			// Añade las dimensiones y las expresiones que se comprueban
			block.AddDimensions(rootML.Attributes[TagDimension].Value.TrimIgnoreNull());
			block.AddExpressions(rootML.Attributes[TagExpression].Value.TrimIgnoreNull());
			// Carga los bloques de las cláusulas then y else (si no hay nodo Then, todo el contenido pertenece al bloque then)
			if (rootML.Nodes.Search(TagThen) is null)
				block.Then.AddRange(LoadBlocks(rootML.Nodes));
			else
				foreach (MLNode nodeML in rootML.Nodes)
					switch (nodeML.Name)
					{
						case TagThen:
								block.Then.AddRange(LoadBlocks(nodeML.Nodes));
							break;
						case TagElse:
								block.Else.AddRange(LoadBlocks(nodeML.Nodes));
							break;
					}
			// Devuelve el bloque
			return block;
	}

	/// <summary>
	///		Carga la información adicional sobre solicitudes para el informe
	/// </summary>
	private List<ReportRequestDimension> LoadRequests(MLNode rootML)
	{
		List<ReportRequestDimension> requests = [];

			// Carga las dimensiones solicitadas
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagDimension)
				{
					ReportRequestDimension dimension = new()
																	{
																		DimensionKey = nodeML.Attributes[TagName].Value.TrimIgnoreNull(),
																		Required = nodeML.Attributes[TagRequired].Value.TrimIgnoreNull().GetBool()
																	};

						// Añade los conjuntos de campos
						foreach (MLNode childML in nodeML.Nodes)
							if (childML.Name == TagField && !string.IsNullOrWhiteSpace(childML.Attributes[TagName].Value.TrimIgnoreNull()))
								foreach (string field in childML.Attributes[TagName].Value.TrimIgnoreNull().Split(';'))
									if (!string.IsNullOrWhiteSpace(field))
										dimension.Fields.Add(new ReportRequestDimensionField
																			{
																				Field = field.TrimIgnoreNull()
																			}
															);
						// Añade los datos de la dimensión
						requests.Add(dimension);
				}
			// Devuelve los datos de la solicitud
			return requests;
	}

	/// <summary>
	///		Carga un origen de datos de un informe
	/// </summary>
	private void LoadDataSource(ReportModel report, MLNode rootML)
	{
		BaseDataSourceModel? dataSource = report.DataWarehouse.DataSources[GetDataSourceId(rootML)];

			if (dataSource is not null)
			{
				ReportDataSourceModel reportDataSource = new(report, dataSource);

					// Carga las expresiones
					foreach (MLNode childML in rootML.Nodes)
						switch (childML.Name)
						{
							case TagRelation:
									reportDataSource.Relations.Add(LoadRelatedDimension(childML, report.DataWarehouse));
								break;
						}
					// Añade el origen de datos
					report.DataSources.Add(reportDataSource);
			}
	}

	/// <summary>
	///		Obtiene el Id de un origen de datos
	/// </summary>
	private string GetDataSourceId(MLNode rootML)
	{
		if (!string.IsNullOrWhiteSpace(rootML.Attributes[TagSourceId].Value))
			return rootML.Attributes[TagSourceId].Value.TrimIgnoreNull();
		else
		{
			string schema = rootML.Attributes[TagSchema].Value.TrimIgnoreNull();
			string table = rootML.Attributes[TagTable].Value.TrimIgnoreNull();

				if (string.IsNullOrWhiteSpace(schema))
					return $"[{table}]";
				else
					return $"[{schema}].[{table}]";
		}
	}

	/// <summary>
	///		Carga los datos de una dimension relacionada
	/// </summary>
	private DimensionRelationModel LoadRelatedDimension(MLNode rootML, DataWarehouseModel dataWarehouse)
	{
		DimensionRelationModel relation = new(dataWarehouse);

			// Carga los datos de la dimensión
			relation.DimensionId = rootML.Attributes[TagSourceId].Value.TrimIgnoreNull();
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
	///		Carga la lista de expresiones
	/// </summary>
	private List<ReportExpressionModel> LoadExpressions(ReportModel report, MLNode rootML)
	{
		List<ReportExpressionModel> expressions = [];
		string fields = rootML.Attributes[TagName].Value.TrimIgnoreNull();

			// Carga los campos en la lista
			if (!string.IsNullOrWhiteSpace(fields))
				foreach (string field in fields.Split(';'))
					if (!string.IsNullOrWhiteSpace(field))
						expressions.Add(new ReportExpressionModel(report, field.TrimIgnoreNull(), 
																  rootML.Attributes[TagType].Value.GetEnum(DataSourceColumnModel.FieldType.Decimal)));
			// Devuelve la lista de expresiones
			return expressions;
	}

	/// <summary>
	///		Graba los datos: aún no hace nada (BauDbStudio lo graba como un archivo XML simple)
	/// </summary>
	public void Update(string id, ReportModel request)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	///		Graba los datos: aún no hace nada (BauDbStudio lo graba como un archivo XML simple)
	/// </summary>
	public async Task UpdateAsync(string id, ReportModel request, CancellationToken cancellationToken)
	{
		// Evita las advertencias
		await Task.Delay(1, cancellationToken);
		// Graba los datos
		Update(id, request);
	}
}