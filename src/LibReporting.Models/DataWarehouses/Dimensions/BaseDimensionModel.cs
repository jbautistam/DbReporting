using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;

namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;

/// <summary>
///		Clase base para los datos de una dimensión
/// </summary>
public abstract class BaseDimensionModel : Base.BaseReportingModel
{
	protected BaseDimensionModel(DataWarehouseModel dataWarehouse)
	{
		DataWarehouse = dataWarehouse;
	}

	/// <summary>
	///		Compara el valor de dos elementos para ordenarlo
	/// </summary>
	public override int CompareTo(Base.BaseReportingModel item)
	{
		if (item is BaseDimensionModel dimension)
			return Id.CompareTo(dimension.Id);
		else
			return -1;
	}

	/// <summary>
	///		Comprueba si una columna está en esta dimensión o en alguna de sus hijas
	/// </summary>
	public abstract bool HasColumn(string dimensionId, string columnId);

	/// <summary>
	///		Obtiene una columna por su Id o por su alias
	/// </summary>
	public abstract DataSets.DataSourceColumnModel? GetColumn(string columnId, bool compareWithAlias);

	/// <summary>
	///		Obtiene el nombre de la tabla origen
	/// </summary>
	public abstract string GetTableFullName();

	/// <summary>
	///		Obtiene el nombre del alias de la tabla origen
	/// </summary>
	public abstract string GetTableAlias();

	/// <summary>
	///		Obtiene el Id del origen de datos
	/// </summary>
	public abstract string GetDataSourceId();

	/// <summary>
	///		Obtiene las columnas asociadas a la dimensión
	/// </summary>
	public abstract Base.BaseReportingDictionaryModel<DataSets.DataSourceColumnModel> GetColumns();

	/// <summary>
	///		Obtiene las relaciones asociadas a la dimensión
	/// </summary>
	public abstract List<DimensionRelationModel> GetRelations();

	/// <summary>
	///		Descripción de la <see cref="BaseDimensionModel"/>
	/// </summary>
	public string Description { get; set; } = string.Empty;

	/// <summary>
	///		Datawarehouse al que se asocia la dimensión
	/// </summary>
	public DataWarehouseModel DataWarehouse { get; }
}
