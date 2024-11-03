namespace Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

/// <summary>
///		Origen de datos con una cadena SQL
/// </summary>
public class DataSourceSqlModel : BaseDataSourceModel
{
	public DataSourceSqlModel(DataWarehouseModel dataWarehouse) : base(dataWarehouse) {}

	/// <summary>
	///		Compara el valor de dos elementos para ordenarlo
	/// </summary>
	public override int CompareTo(Base.BaseReportingModel item)
	{
		if (item is DataSourceSqlModel dataSource)
			return Id.CompareTo(dataSource.Id);
		else
			return -1;
	}

	/// <summary>
	///		Obtiene el nombre completo de la tabla o la SQL del contenido si es una vista
	/// </summary>
	public override string GetTableFullNameOrContent() => $"({Sql})";

	/// <summary>
	///		Obtiene el nombre de tabla
	/// </summary>
	public override string GetTableAlias() => Id;

	/// <summary>
	///		Comando SQL de consulta
	/// </summary>
	public string Sql { get; set; } = string.Empty;

	/// <summary>
	///		Parámetros asociados a la consulta
	/// </summary>
	public List<DataSourceSqlParameterModel> Parameters { get; } = new();
}
