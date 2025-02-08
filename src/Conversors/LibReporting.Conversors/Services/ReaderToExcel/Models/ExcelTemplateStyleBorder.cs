namespace Bau.Libraries.LibReporting.Conversors.Services.ReaderToExcel.Models;

/// <summary>
///		Estilo de borde
/// </summary>
internal class ExcelTemplateStyleBorder
{
    /// <summary>
    ///     Patrón de línea
    /// </summary>
    internal enum LinePattern
    {
        None,
		DashDot,
		DashDotDot,
		Dashed,
		Dotted,
		Double,
		Hair,
		Medium,
		MediumDashDot,
		MediumDashDotDot,
		MediumDashed,
		SlantDashDot,
		Thick,
		Thin
    }

    /// <summary>
    ///     Color del borde
    /// </summary>
    internal System.Drawing.Color? Color { get; set; }

    /// <summary>
    ///     Patrón del borde
    /// </summary>
    internal LinePattern Pattern { get; set; } = LinePattern.Thin;
}
