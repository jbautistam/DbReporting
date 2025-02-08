namespace Bau.Libraries.LibReporting.Conversors.Services.ReaderToExcel.Models;

/// <summary>
///     Sección de la plantilla
/// </summary>
internal class ExcelTemplateColumn
{
    /// <summary>
    ///     Tipo de la columna
    /// </summary>
    internal enum OutputType
    {
        String,
        DateTime,
        Int,
        Decimal,
        Boolean
    }

    internal ExcelTemplateColumn(ExcelTemplate template)
    {
        Template = template;
    }

    /// <summary>
    ///     Plantilla a la que se asocia la sección
    /// </summary>
    internal ExcelTemplate Template { get; }

    /// <summary>
    ///     Indica si la columna es obligatoria (aunque no exista el valor en el <see cref="System.Data.IDataReader"/>
    /// se muestra la columna)
    /// </summary>
    internal bool Required { get; set; }

    /// <summary>
    ///     Título de la celda (localizable)
    /// </summary>
    internal string Title { get; set; } = string.Empty;

    /// <summary>
    ///     Campo
    /// </summary>
    internal string Field { get; set; } = string.Empty;

    /// <summary>
    ///     Campo de totales
    /// </summary>
    internal string TotalField { get; set; } = string.Empty;

    /// <summary>
    ///     Tipo de salida
    /// </summary>
    internal OutputType Type { get; set; } = OutputType.String;

    /// <summary>
    ///     Valor predeterminado a apuntar en el Excel cuando el valor de salida es nulo
    /// </summary>
    internal string Default { get; set; } = string.Empty;

    /// <summary>
    ///     Formato del campo
    /// </summary>
    internal string Format { get; set; } = string.Empty;

    /// <summary>
    ///     Estilo de la cabecera
    /// </summary>
    internal string HeaderStyleId { get; set; } = string.Empty;

    /// <summary>
    ///     Estilo de la celda
    /// </summary>
    internal string CellStyleId { get; set; } = string.Empty;

    /// <summary>
    ///     Estilo de la celda de totales
    /// </summary>
    internal string TotalStyleId { get; set; } = string.Empty;
}
