namespace Bau.Libraries.LibReporting.Conversors.Services.ReaderToExcel.Models;

/// <summary>
///		Estilo de una celda
/// </summary>
internal class ExcelTemplateStyle
{
    internal enum HorizontalAlignment
    {
        None,
        Left,
        Right,
        Center
    }

    internal enum VerticalAlignment
    {
        None,
        Top,
        Bottom,
        Middle
    }
    /// <summary>
    ///		Patrón de relleno
    /// </summary>
    internal enum Pattern
    {
        None,
        Solid,
        DarkDown,
        DarkGray,
        DarkGrid,
        DarkHorizontal,
        DarkTrellis,
        DarkUp,
        DarkVertical,
        Gray0625,
        Gray125,
        LightDown,
        LightGray,
        LightGrid,
        LightHorizontal,
        LightTrellis,
        LightUp,
        LightVertical,
        MediumGray
    }

    internal ExcelTemplateStyle(string id)
    {
        Id = id;
    }

    /// <summary>
    ///     Identificador del estilo
    /// </summary>
    internal string Id { get; }

    /// <summary>
    ///     Indica si el texto se debe ajustar
    /// </summary>
    internal bool WrapText { get; set; }

    /// <summary>
    ///     Alineación horizontal
    /// </summary>
    internal HorizontalAlignment HorizontalAlign { get; set; } = HorizontalAlignment.None;

    /// <summary>
    ///     Alineación vertical
    /// </summary>
    internal VerticalAlignment VerticalAlign { get; set; } = VerticalAlignment.None;

    /// <summary>
    ///     Color del texto
    /// </summary>
    internal System.Drawing.Color? Color { get; set; }

    /// <summary>
    ///     Color del fondo
    /// </summary>
    internal System.Drawing.Color? Background { get; set; }

    /// <summary>
    ///     Patrón de relleno del fondo
    /// </summary>
    internal Pattern Fill { get; set; } = Pattern.None;

    /// <summary>
    ///     Tamaño del texto
    /// </summary>
    internal double Size { get; set; }

    /// <summary>
    ///     Indica si el texto está en negrita
    /// </summary>
    internal bool Bold { get; set; }

    /// <summary>
    ///     Estilo del borde superior
    /// </summary>
    internal ExcelTemplateStyleBorder? BorderTop { get; set; }

    /// <summary>
    ///     Estilo del borde inferior
    /// </summary>
    internal ExcelTemplateStyleBorder? BorderBottom { get; set; }

    /// <summary>
    ///     Estilo del borde derecho
    /// </summary>
    internal ExcelTemplateStyleBorder? BorderRight { get; set; }

    /// <summary>
    ///     Estilo del borde izquierdo
    /// </summary>
    internal ExcelTemplateStyleBorder? BorderLeft { get; set; }
}
