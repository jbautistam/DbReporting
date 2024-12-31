namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;

/// <summary>
///		Claves de una relación con una dimensión
/// </summary>
public class DimensionRelationModel
{
	public DimensionRelationModel(DataWarehouseModel dataWarehouse)
	{
		DataWarehouse = dataWarehouse;
	}

	/// <summary>
	///		Clona los datos del objeto
	/// </summary>
	public DimensionRelationModel Clone()
	{
		DimensionRelationModel target = new(DataWarehouse)
											{
												DimensionId = DimensionId
											};

			// Clona las claves foráneas
			foreach (RelationForeignKey foreignKey in ForeignKeys)
				target.ForeignKeys.Add(foreignKey.Clone());
			// Devuelve el objeto clonado
			return target;
	}

	/// <summary>
	///		Datawarehouse al que pertenece la dimensión
	/// </summary>
	public DataWarehouseModel DataWarehouse { get; }

	/// <summary>
	///		Código de la dimensión relacionada
	/// </summary>
	public string DimensionId { get; set; } = string.Empty;

	/// <summary>
	///		Dimensión con la que se establece esta relación
	/// </summary>
	public Dimensions.BaseDimensionModel? Dimension => DataWarehouse.Dimensions[DimensionId];

	/// <summary>
	///		Campos de la relación
	/// </summary>
	public List<RelationForeignKey> ForeignKeys { get; } = [];
}
