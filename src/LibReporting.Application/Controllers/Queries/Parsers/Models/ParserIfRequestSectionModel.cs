﻿namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Modelo de interpretación de una condición
/// </summary>
/// <example>
///     IfRequest
///         -Expression: a, b, c
///             -Sql
///         -WhenRequestTotals
///         -WithComma
///         -Sql
/// </example>
internal class ParserIfRequestSectionModel : ParserBaseSectionModel
{
	/// <summary>
	///     Expresiones solicitadas
	/// </summary>
	internal List<ParserIfRequestSectionExpressionModel> Expressions { get; } = [];

    /// <summary>
    ///     Indica si se debe añadir una coma al generar la SQL
    /// </summary>
    internal bool WithComma { get; set; }

    /// <summary>
    ///     Sql que se debe generar independientemente de si tiene expresiones o no
    /// </summary>
    internal string? Sql { get; set; }
}