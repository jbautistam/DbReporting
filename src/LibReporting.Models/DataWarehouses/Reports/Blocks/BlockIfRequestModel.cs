namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;

/// <summary>
///		Bloque que comprueba si se ha solicitado una dimensión
/// </summary>
public class BlockIfRequest : BaseBlockModel
{
    public BlockIfRequest(string key) : base(key) {}

    /// <summary>
    ///     Añade una serie de dimensiones
    /// </summary>
    public void AddDimensions(string key)
    {
        if (!string.IsNullOrWhiteSpace(key))
            foreach (string part in key.Split(';'))
                if (!string.IsNullOrWhiteSpace(key))
                    DimensionKeys.Add(part.Trim());
    }

    /// <summary>
    ///     Añade una serie de expresiones
    /// </summary>
    public void AddExpressions(string key)
    {
        if (!string.IsNullOrWhiteSpace(key))
            foreach (string part in key.Split(';'))
                if (!string.IsNullOrWhiteSpace(key))
                    ExpressionKeys.Add(part.Trim());
    }

    /// <summary>
    ///		Claves de las dimensiones que se comprueban
    /// </summary>
    public List<string> DimensionKeys { get; } = [];

    /// <summary>
    ///     Claves de las expresiones que se comprueban
    /// </summary>
    public List<string> ExpressionKeys { get; } = [];

    /// <summary>
    ///     Indica si se tiene en cuenta si se han pedido totales
    /// </summary>
    public bool WhenTotals { get; set; }

    /// <summary>
    ///     Indica si se tienen que tener en cuenta todas las dimensiones (si es false, con una dimensión
    /// solicitada, sería correcto)
    /// </summary>
    public bool AllDimensions { get; set; }

    /// <summary>
    ///     Indica si se tienen que tener en cuenta todas las expresiones (si es false, con una expresión
    /// solicitada, sería correcto)
    /// </summary>
    public bool AllExpressions { get; set; }

    /// <summary>
    ///     Bloques a generar cuando se cumple la condición
    /// </summary>
    public List<BaseBlockModel> Then { get; } = [];

    /// <summary>
    ///     Bloques a generar cuando no se cumple la condición
    /// </summary>
    public List<BaseBlockModel> Else { get; } = [];
}
