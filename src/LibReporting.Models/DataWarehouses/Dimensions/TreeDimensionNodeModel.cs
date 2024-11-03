namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;

/// <summary>
///     Nodo del árbol de dimensiones
/// </summary>
public class TreeDimensionNodeModel
{
    public TreeDimensionNodeModel(TreeDimensionModel tree, TreeDimensionNodeModel? parent, BaseDimensionModel dimension)
    {
        Tree = tree;
        Parent = parent;
        Dimension = dimension;
    }

    /// <summary>
    ///     Árbol al que se asocia el nodo
    /// </summary>
    public TreeDimensionModel Tree { get; }

    /// <summary>
    ///     Nodo padre
    /// </summary>
    public TreeDimensionNodeModel? Parent { get; }

    /// <summary>
    ///     Dimensión
    /// </summary>
    public BaseDimensionModel Dimension { get; }

    /// <summary>
    ///     Nodos de dimensiones hija
    /// </summary>
    public List<TreeDimensionNodeModel> Childs { get; } = [];
}
