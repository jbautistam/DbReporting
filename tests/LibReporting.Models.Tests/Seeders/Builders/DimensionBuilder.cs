using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;

namespace LibReporting.Models.Tests.Seeders.Builders;

/// <summary>
///		Generador de <see cref="BaseDimensionModel"/>
/// </summary>
public class DimensionBuilder
{
	public DimensionBuilder(DataWarehouseBuilder dataWarehouseBuilder, string id, string dataSource)
	{
		// Asigna los objetos
		DataWarehouseBuilder = dataWarehouseBuilder;
		// Crea la dimensión
		Dimension = new DimensionModel(dataWarehouseBuilder.DataWarehouse, dataWarehouseBuilder.DataWarehouse.DataSources[dataSource]!)
								{
									Id = id
								};
		// Añade la dimensión al almacén
		DataWarehouseBuilder.DataWarehouse.Dimensions.Add(Dimension);
	}

	public DimensionBuilder(DataWarehouseBuilder dataWarehouseBuilder, string id, string sourceDimensionId, string columnsPrefix)
	{
		// Asigna los objetos
		DataWarehouseBuilder = dataWarehouseBuilder;
		// Crea la dimensión
		Dimension = new DimensionChildModel(dataWarehouseBuilder.DataWarehouse, id, sourceDimensionId, columnsPrefix);
		// Añade la dimensión al almacén
		DataWarehouseBuilder.DataWarehouse.Dimensions.Add(Dimension);
	}

	/// <summary>
	///		Añade una relación a la dimensión
	/// </summary>
	public DimensionBuilder WithRelation(string targetDimensionId, List<(string columnId, string targetColumnId)> relations)
	{
		// Añade la relación a la dimensión)
		if (Dimension is DimensionModel dimension)
		{
			DimensionRelationModel relation = new (DataWarehouseBuilder.DataWarehouse)
														{
															DimensionId = targetDimensionId
														};

				// Añade las claves foráneas
				foreach ((string columnId, string targetColumnId) in relations)
					relation.ForeignKeys.Add(new RelationForeignKey
														{
															ColumnId = columnId,
															TargetColumnId = targetColumnId
														}
											);
				// Añade la relación
				dimension.Relations.Add(relation);
		}
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Devuelve el <see cref="DataWarehouseBuilder"/> padre
	/// </summary>
	public DataWarehouseBuilder Back() => DataWarehouseBuilder;

	/// <summary>
	///		Generador padre
	/// </summary>
	public DataWarehouseBuilder DataWarehouseBuilder { get; }

	/// <summary>
	///		Dimensión
	/// </summary>
	public BaseDimensionModel Dimension { get; }
}
