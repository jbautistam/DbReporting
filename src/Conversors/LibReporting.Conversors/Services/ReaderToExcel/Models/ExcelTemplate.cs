namespace Bau.Libraries.LibReporting.Conversors.Services.ReaderToExcel.Models;

/// <summary>
///     Clase con los datos de la plantilla Excel de un informe
/// </summary>
internal class ExcelTemplate
{
    internal ExcelTemplate(string id)
    {
        Id = id;
    }

    /// <summary>
    ///     Obtiene un estilo por su Id
    /// </summary>
	internal ExcelTemplateStyle? GetStyle(string styleId)
	{
        // Busca el estilo
        if (!string.IsNullOrWhiteSpace(styleId))
            foreach (ExcelTemplateStyle style in Styles)
                if (style.Id.Equals(styleId, StringComparison.CurrentCultureIgnoreCase))
                    return style;
        // Si ha llegado hasta aquí es porque no ha encontrado nada
        return null;
	}

    /// <summary>
    ///     Identificador de la plantilla
    /// </summary>
    internal string Id { get; }

    /// <summary>
    ///     Nombre de la hoja
    /// </summary>
    internal string Sheet { get; set; } = "Data";

    /// <summary>
    ///     Número máximo de registros por hoja
    /// </summary>
    internal int RecordsPerSheet { get; set; } = 999_998;

    /// <summary>
    ///     Indica si se deben fijar las filas de cabecera
    /// </summary>
    internal bool FreezeHeadersRows { get; set; } = true;

    /// <summary>
    ///     Número de columnas que se deben fijar
    /// </summary>
    internal int FreezeColumns { get; set; }

    /// <summary>
    ///     Indica si el informe tiene una fila de totales
    /// </summary>
    internal bool WithTotals { get; set; } = true;

	/// <summary>
	///     Campos de la plantilla
	/// </summary>
	internal List<ExcelTemplateColumn> Columns { get; } = new();

    /// <summary>
    ///     Estilos de la plantilla
    /// </summary>
    internal List<ExcelTemplateStyle> Styles { get; } = new();
}
