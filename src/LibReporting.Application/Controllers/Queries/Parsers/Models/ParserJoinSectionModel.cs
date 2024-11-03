namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Sección con datos de un JOIN
/// </summary>
internal class ParserJoinSectionModel : ParserBaseSectionModel
{
	/// <summary>
	///		Tipo de join
	/// </summary>
	public enum JoinType
	{
		/// <summary>INNER JOIN</summary>
		InnerJoin,
		/// <summary>LEFT JOIN</summary>
		LeftJoin,
		/// <summary>RIGHT JOIN</summary>
		RightJoin,
		/// <summary>FULL OUTER JOIN</summary>
		FullJoin,
		/// <summary>CROSS JOIN</summary>
		CrossJoin
	}

    /// <summary>
    ///		Obtiene la cadena adecuada para un tipo de JOIN
    /// </summary>
    internal string GetJoin()
    {
	        return Join switch
	                    {
		                    JoinType.InnerJoin => " INNER JOIN ",
		                    JoinType.CrossJoin => " CROSS JOIN ",
		                    JoinType.FullJoin => " FULL OUTER JOIN ",
		                    JoinType.LeftJoin => " LEFT JOIN ",
		                    JoinType.RightJoin => " RIGHT JOIN ",
		                    _ => throw new Exceptions.ReportingParserException($"Join type unknown: {Join.ToString()}"),
	                    };
	}

	/// <summary>
	///		Tipo de unión
	/// </summary>
	internal JoinType Join { get; set; }

    /// <summary>
    ///     Nombre de tabla
    /// </summary>
    internal string Table { get; set; } = string.Empty;

    /// <summary>
    ///     Alias de la tabla
    /// </summary>
    internal string TableAlias { get; set; } = string.Empty;

    /// <summary>
    ///     Tabla con la que se hace el join
    /// </summary>
    internal string TableJoin
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(TableAlias))
                return TableAlias;
            else if (!string.IsNullOrWhiteSpace(Table))
                return Table;
            else
                return string.Empty;
        }
    }

    /// <summary>
    ///     Relaciones asociadas al JOIN
    /// </summary>
    internal List<ParserJoinDimensionSectionModel> JoinDimensions { get; } = new();

    /// <summary>
    ///     Sql adicional
    /// </summary>
    internal string Sql { get; set; } = string.Empty;

    /// <summary>
    ///     Sql que se debe añadir cuando no hay nada en el Join
    /// </summary>
    internal string SqlNoDimension { get; set; } = string.Empty;
}
