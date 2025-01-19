using Bau.Libraries.LibHelper.Extensors;

namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Modelo de interpretación de una dimensión
/// </summary>
internal class ParserJoinDimensionSectionModel : ParserBaseSectionModel
{
    // Variables privadas
    private string _tableAlias = string.Empty;

    /// <summary>
    ///     Añade los campos por los que se va a hacer el join
    /// </summary>
    internal void AddFieldsJoin(string fields)
    { 
        if (!string.IsNullOrWhiteSpace(fields))
        { 
            string[] parts = fields.Split('-');

                // Si sólo hay un campo, se utiliza el mismo nombre de campo
                if (parts.Length == 1)
                    AddField(parts[0], parts[0]);
                else
                    AddField(parts[0], parts[1]);
        }
        else
            throw new Exceptions.ReportingParserException("Undefined field in clause ON");
    }

    /// <summary>
    ///     Añade el campo
    /// </summary>
    private void AddField(string fieldDimension, string fieldTable)
    {
        // Quita los espacios
        fieldDimension = fieldDimension.TrimIgnoreNull();
        fieldTable = fieldTable.TrimIgnoreNull();
        // Añade los campos si no estaban ya
        if (!Fields.Any(item => item.fieldDimension.Equals(fieldDimension, StringComparison.CurrentCultureIgnoreCase) && 
                                item.fieldTable.Equals(fieldTable, StringComparison.CurrentCultureIgnoreCase)))
            Fields.Add((fieldDimension, fieldTable));
    }

	/// <summary>
	///		Clave de la dimensión
	/// </summary>
	internal string DimensionKey { get; set; } = default!;

    /// <summary>
    ///		Tabla de la dimensión
    /// </summary>
    internal string Table { get; set; } = default!;

    /// <summary>
    ///     Alias que se debe utilizar para tabla de dimensión
    /// </summary>
    internal string TableAlias
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_tableAlias))
                return Table;
            else
                return _tableAlias;
        }
        set { _tableAlias = value; }
    }

    /// <summary>
    ///     Indica si se debe comprobar si es nulo
    /// </summary>
    internal bool CheckIfNull { get; set; }

    /// <summary>
    ///     Indica si se debe unir por los campos solicitados
    /// </summary>
    internal bool WithRequestedFields { get; set; }

    /// <summary>
    ///		Campos por los que se hace la unión
    /// </summary>
    internal List<(string fieldDimension, string fieldTable)> Fields { get; } = [];
}
