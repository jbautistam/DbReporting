namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;

/// <summary>
///		Arbol de <see cref="BaseDimensionModel"/>
/// </summary>
public class TreeDimensionModel
{
	public TreeDimensionModel(DataWarehouseModel dataWarehouse)
	{
		DataWarehouse = dataWarehouse;
	}

	/// <summary>
	///		Rellena el árbol de dimensiones
	/// </summary>
	public void Fill()
	{
		// Limpia los datos
		Dimensions.Clear();
		// Añade las dimensiones
		foreach (BaseDimensionModel dimension in DataWarehouse.Dimensions.EnumerateValues())
			Dimensions.Add(CreateNode(null, dimension));
	}

	/// <summary>
	///		Crea un nodo
	/// </summary>
	private TreeDimensionNodeModel CreateNode(TreeDimensionNodeModel? parent, BaseDimensionModel dimension)
	{
		TreeDimensionNodeModel node = new(this, parent, dimension);

			// Crea los nodos
			foreach (Relations.DimensionRelationModel relation in dimension.GetRelations())
				if (relation.Dimension is not null)
					node.Childs.Add(CreateNode(node, relation.Dimension));
			// Devuelve el nodo creado
			return node;
	}

	/// <summary>
	///		Almacén de datos
	/// </summary>
	public DataWarehouseModel DataWarehouse { get; }

	/// <summary>
	///		Dimensiones en el árbol
	/// </summary>
	public List<TreeDimensionNodeModel> Dimensions { get; } = [];
}
