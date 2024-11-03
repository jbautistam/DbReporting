namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;

/// <summary>
///		Clase base para los bloques de informe
/// </summary>
public abstract class BaseBlockModel
{
	protected BaseBlockModel(string key)
	{
		Key = key;
	}

    /// <summary>
    ///		Clave del bloque
    /// </summary>
    public string Key { get; }
}