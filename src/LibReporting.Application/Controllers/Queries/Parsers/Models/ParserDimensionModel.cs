namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Modelo de interpretación de una dimensión
/// </summary>
internal class ParserDimensionModel : ParserBaseSectionModel
{
    // Variables privadas
    private string _tableDimensionAlias = string.Empty;

    /// <summary>
    ///     Añade los dimensiones relacionadas
    /// </summary>
    internal void AddRelatedDimensions(string content)
    {
        RelatedDimensions.AddRange(SplitContent(content));
    }

	/// <summary>
	///		Clave de la dimensión
	/// </summary>
	internal string DimensionKey { get; set; } = string.Empty;

    /// <summary>
    ///		Tabla de la dimensión
    /// </summary>
    internal string Table { get; set; } = string.Empty;

    /// <summary>
    ///		Tabla adicional de la dimensión
    /// </summary>
    internal string AdditionalTable { get; set; } = string.Empty;

    /// <summary>
    ///     Indica si se debe comprobar si es nulo con respecto a la tabla adicional
    /// </summary>
    internal bool CheckIfNull { get; set; }

    /// <summary>
    ///     Indica si se debe añadir un alias a la dimensión
    /// </summary>
	internal bool MustUseAlias => !string.IsNullOrWhiteSpace(_tableDimensionAlias);

    /// <summary>
    ///		Alias de la tabla de dimensión (para los casos en que es un JOIN contra la misma tabla)
    /// </summary>
    internal string TableAlias
    {
        get
        {
            if (MustUseAlias)
                return _tableDimensionAlias;
            else
                return Table;
        }
        set { _tableDimensionAlias = value; }
    }

    /// <summary>
    ///		Indica si se deben incluir las claves primarias en la lista de campos
    /// </summary>
    internal bool WithPrimaryKeys { get; set; }

    /// <summary>
    ///     Indica si se deben incluir los campos solicitados (sean o no claves primarias)
    /// </summary>
    internal bool WithRequestedFields { get; set; }

    /// <summary>
    ///		Indica si es obligatorio aunque no se haya solicitado
    /// </summary>
    internal bool Required { get; set; }

    /// <summary>
    ///		Dimensiones relacionadas (obliga a hacer un join por esta dimensión aunque no se haya solicitado directamente)
    /// </summary>
    internal List<string> RelatedDimensions { get; } = [];

    /// <summary>
    ///		Dimensiones que se deben comprobar que no se han solicitado
    /// </summary>
    internal List<string> IfNotRequestDimensions { get; } = [];
}