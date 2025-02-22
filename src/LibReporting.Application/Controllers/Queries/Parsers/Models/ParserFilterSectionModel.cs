using Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Sección con datos de una sección WHERE o HAVING. Añade los filtros de la solicitud a las expresiones
/// </summary>
/// <example>
/// Where 
///		-Expression: Name
///		    --Table: TableAlias
///		    --Field: Field
///		    --Aggregation: SUM | AVG | ...
///		-DataSource: Name
///		    --Table: TableAlias
///		    --Field: Field
///		-Operator: AND | OR | ...
///		-Sql: A = B
/// </example>
internal class ParserFilterSectionModel : ParserBaseSectionModel
{
    /// <summary>
    ///     Tipo de filtro
    /// </summary>
    internal enum FilterType
    {
        /// <summary>Filtro para la cláusula WHERE</summary>
        Where = 1,
        /// <summary>Filtro para la cláusula HAVING</summary>
        Having
    }

    internal ParserFilterSectionModel(FilterType type)
    {
        Type = type;
    }

    /// <summary>
    ///     Tipo de filtro
    /// </summary>
    internal FilterType Type { get; }

    /// <summary>
    ///     Bloques con los orígenes de datos
    /// </summary>
    internal List<ParserDataSourceModel> DataSources { get; } = [];

    /// <summary>
    ///     Bloques con las expresiones
    /// </summary>
    internal List<ParserExpressionModel> Expressions { get; } = [];

	/// <summary>
	///		Agregación (SUM, MAX...) en caso que estemos en una cláusula HAVING
	/// </summary>
	internal string? Aggregation { get; set; }

    /// <summary>
    ///     Sql adicional
    /// </summary>
    internal string? Sql { get; set; }

    /// <summary>
    ///     Operador con el que se va a asociar la SQL
    /// </summary>
    internal string? Operator { get; set; }
}
