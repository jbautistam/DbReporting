namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Sección con datos de una sección WHERE
/// </summary>
/// <example>
/// Having 
///		-Expression: Name
///		    --Aggregation: SUM | MAX | AVG
///		    --Table: Table
///		-Operator: AND | OR | ...
///		-Sql: A = B
/// </example>
internal class ParserHavingSectionModel : ParserCondiciontSectionModel
{
    /// <summary>
    ///     Bloques con las expresiones a filtrar
    /// </summary>
    internal List<ParserExpressionModel> Expressions { get; } = new();
}