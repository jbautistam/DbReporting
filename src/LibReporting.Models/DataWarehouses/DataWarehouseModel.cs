namespace Bau.Libraries.LibReporting.Models.DataWarehouses;

/// <summary>
///		Clase con los datos de un almacén de datos
/// </summary>
public class DataWarehouseModel : Base.BaseReportingModel
{
	public DataWarehouseModel(ReportingSchemaModel schema)
	{
		Schema = schema;
	}

	/// <summary>
	///		Compara el valor de dos elementos para ordenarlo
	/// </summary>
	public override int CompareTo(Base.BaseReportingModel item)
	{
		if (item is DataWarehouseModel dataWarehouse)
			return Name.CompareTo(dataWarehouse.Name);
		else
			return -1;
	}

	/// <summary>
	///		Obtiene un origen de datos por su nombre completo
	/// </summary>
	public DataSets.DataSourceTableModel? GetDataTableByFullName(string fullName)
	{
		// Obtiene el dataTable por su nombre completo
		foreach (DataSets.BaseDataSourceModel dataSource in DataSources.EnumerateValues())
			if (dataSource is DataSets.DataSourceTableModel dataTable && dataTable.FullName.Equals(fullName, StringComparison.CurrentCultureIgnoreCase))
				return dataTable;
		// Si ha llegado hasta aquí es porque no ha encontrado nada
		return null;
	}

	/// <summary>
	///		Esquema
	/// </summary>
	public ReportingSchemaModel Schema { get; }

	/// <summary>
	///		Nombre del elemento
	/// </summary>
	public string Name { get; set; } = default!;

	/// <summary>
	///		Descripción del elemento
	/// </summary>
	public string Description { get; set; } = default!;

	/// <summary>
	///		Orígenes de datos
	/// </summary>
	public Base.BaseReportingDictionaryModel<DataSets.BaseDataSourceModel> DataSources { get; } = new();

	/// <summary>
	///		Dimensiones del esquema
	/// </summary>
	public Base.BaseReportingDictionaryModel<Dimensions.BaseDimensionModel> Dimensions { get; } = new();

	/// <summary>
	///		Informes
	/// </summary>
	public Base.BaseReportingDictionaryModel<Reports.ReportModel> Reports { get; } = new();

	/// <summary>
	///		Reglas de transformación
	/// </summary>
	public List<Reports.Transformers.TransformRuleModel> Rules { get; } = [];
}