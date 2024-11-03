namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;

/// <summary>
///		Bloque de consulta de un informe
/// </summary>
public class BlockQueryModel : BaseBlockModel
{
    public BlockQueryModel(string key) : base(key) {}

    /// <summary>
    ///		Consulta del bloque
    /// </summary>
    public string Sql { get; set; } = string.Empty;
}
