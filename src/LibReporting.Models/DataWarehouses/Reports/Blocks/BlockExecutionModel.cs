namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;

/// <summary>
///		Bloque de ejecución de SQL
/// </summary>
public class BlockExecutionModel : BaseBlockModel
{
    public BlockExecutionModel(string key) : base(key) {}

    /// <summary>
    ///		Sql que se debe ejecutar
    /// </summary>
    public string Sql { get; set; } = default!;
}