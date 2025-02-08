namespace Bau.Libraries.LibReporting.Conversors.Models;

/// <summary>
///     Clase con los datos de una solicitud
/// </summary>
public class Request
{
    public Request(int page, long size)
    {
        Page = page;
        Size = size;
    }

    /// <summary>
    ///     Página solicitada
    /// </summary>
    public int Page { get; }

    /// <summary>
    ///     Tamaño de página
    /// </summary>
    public long Size { get; }
}
