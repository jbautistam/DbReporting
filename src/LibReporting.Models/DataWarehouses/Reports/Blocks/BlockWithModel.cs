namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;

/// <summary>
///		Bloque de creación de un bloque WITH
/// </summary>
public class BlockWithModel : BaseBlockModel
{
    public BlockWithModel(string key) : base(key) {}

    /// <summary>
    ///		Bloques
    /// </summary>
    public List<BaseBlockModel> Blocks { get; set; } = new();
}
