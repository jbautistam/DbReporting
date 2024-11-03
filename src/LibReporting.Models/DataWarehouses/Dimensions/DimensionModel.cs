using Bau.Libraries.LibReporting.Models.Base;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;

namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;

/// <summary>
///		Clase con los datos de una dimensión
/// </summary>
public class DimensionModel : BaseDimensionModel
{
	public DimensionModel(DataWarehouseModel dataWarehouse, BaseDataSourceModel dataSource) : base(dataWarehouse)
	{
		DataSource = dataSource;
	}

	/// <summary>
	///		Comprueba si una columna está en esta dimensión o en alguna de sus hijas
	/// </summary>
	public override bool HasColumn(string dimensionId, string columnId)
	{
		// Si la columna está en esta dimensión ...
		if (Id.Equals(dimensionId, StringComparison.CurrentCultureIgnoreCase) && DataSource.HasColumn(columnId))
			return true;
		else // ... si la columna está en alguna dimensión de la jerarquía
			foreach (DimensionRelationModel relation in Relations)
				if (relation.Dimension is not null && relation.Dimension.HasColumn(dimensionId, columnId))
					return true;
		// Si ha llegado hasta aquí es porque no ha encontrado nada
		return false;
	}

	/// <summary>
	///		Obtiene una columna por su Id o por su alias
	/// </summary>
	public override DataSourceColumnModel? GetColumn(string columnId, bool compareWithAlias)
	{
		DataSourceColumnModel? column = DataSource.GetColumn(columnId, compareWithAlias);

			// Si no ha encontrado la columna, busca en las dimensiones hija
			if (column is null)
				foreach (DimensionRelationModel relation in Relations)
					if (column is null && relation.Dimension is not null)
						column = relation.Dimension.GetColumn(columnId, compareWithAlias);
			// Devuelve la columna
			return column;
	}

	/// <summary>
	///		Obtiene el nombre de la tabla origen
	/// </summary>
	public override string GetTableFullName() => DataSource.GetTableFullNameOrContent();

	/// <summary>
	///		Obtiene el nombre del alias de la tabla origen
	/// </summary>
	public override string GetTableAlias() => DataSource.GetTableAlias();

	/// <summary>
	///		Obtiene el Id del origen de datos
	/// </summary>
	public override string GetDataSourceId() => DataSource.Id;

	/// <summary>
	///		Obtiene las columnas de la dimensión
	/// </summary>
	public override BaseReportingDictionaryModel<DataSourceColumnModel> GetColumns() => DataSource.Columns;

	/// <summary>
	///		Obtiene las relaciones asociadas a la dimensión
	/// </summary>
	public override List<DimensionRelationModel> GetRelations() => Relations;

	/// <summary>
	///		Origen de datos al que se asocia esta dimensión
	/// </summary>
	public BaseDataSourceModel DataSource { get; }

	/// <summary>
	///		Relaciones de esta dimensión con su dimensión hijo
	/// </summary>
	public List<DimensionRelationModel> Relations { get; } = [];
}
