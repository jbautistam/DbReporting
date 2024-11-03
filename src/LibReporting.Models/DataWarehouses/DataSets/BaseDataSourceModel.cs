namespace Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

/// <summary>
///		Clase base para los orígenes de datos
/// </summary>
public abstract class BaseDataSourceModel : Base.BaseReportingModel
{
	protected BaseDataSourceModel(DataWarehouseModel dataWarehouse)
	{
		DataWarehouse = dataWarehouse;
	}

	/// <summary>
	///		Obtiene el nombre completo de la tabla o la SQL del contenido si es una vista
	/// </summary>
	public abstract string GetTableFullNameOrContent();

	/// <summary>
	///		Obtiene el alias de tabla
	/// </summary>
	public abstract string GetTableAlias();

	/// <summary>
	///		Comprueba si este origen de datos tiene alguna columna con el Id solicitado
	/// </summary>
	public bool HasColumn(string columnId) => Columns[columnId] is not null;

	/// <summary>
	///		Obtiene la columna por su nombre o por su alias si es necesario
	/// </summary>
	public DataSourceColumnModel? GetColumn(string columnId, bool compareWithAlias)
	{
		DataSourceColumnModel? column = Columns[columnId];

			// Si no se ha obtenido por el nombre, busca por el alias
			if (column is null && compareWithAlias)
				foreach (DataSourceColumnModel columnSearch in Columns.EnumerateValues())
					if (column is null && columnSearch.Alias.Equals(columnId, StringComparison.CurrentCultureIgnoreCase))
						column = columnSearch;
			// Devuelve la columna localizada
			return column;
	}

	/// <summary>
	///		Almacén de datos
	/// </summary>
	public DataWarehouseModel DataWarehouse { get; }

	/// <summary>
	///		Columnas visibles de la tabla
	/// </summary>
	public Base.BaseReportingDictionaryModel<DataSourceColumnModel> Columns { get; } = new();
}
