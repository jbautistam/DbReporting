namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;

/// <summary>
///		Bloque de creación de una CTE con una dimensión para un informe
/// </summary>
public class BlockCreateCteDimensionModel : BaseBlockModel
{
    public BlockCreateCteDimensionModel(string key) : base(key) {}

    /// <summary>
    ///     Clave de la dimensión
    /// </summary>
    public string DimensionKey { get; set; } = default!;

    /// <summary>
    ///     Campos adicionales de la consulta
    /// </summary>
    public List<ClauseFieldModel> Fields { get; } = [];

    /// <summary>
    ///     Filtros adicionales
    /// </summary>
    public List<ClauseFilterModel> Filters { get; } = [];
}
