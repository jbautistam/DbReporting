namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;

/// <summary>
///		Bloque de creación de una CTE
/// </summary>
public class BlockCreateCteModel : BaseBlockModel
{
    public BlockCreateCteModel(string key) : base(key) {}

    /// <summary>
    ///     Bloques para la creación de la CTE
    /// </summary>
    public List<BaseBlockModel> Blocks { get; } = new();
}
