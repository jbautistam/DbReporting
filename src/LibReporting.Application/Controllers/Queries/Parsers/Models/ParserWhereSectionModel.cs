namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Sección con datos de una sección WHERE
/// </summary>
/// <example>
/// Where 
///		-DataSource: Name
///		    --Table: TableAlias
///		-Operator: AND | OR | ...
///		-Sql: A = B
/// </example>
internal class ParserWhereSectionModel : ParserConditionSectionModel
{
    /// <summary>
    ///     Bloques con los orígenes de datos
    /// </summary>
    internal List<ParserDataSourceModel> DataSources { get; } = [];
}
