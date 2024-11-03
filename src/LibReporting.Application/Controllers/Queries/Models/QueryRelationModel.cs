namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

/// <summary>
///		Relación de una consulta
/// </summary>
internal class QueryRelationModel
{
	internal QueryRelationModel(string column, string relatedTable, string relatedColumn)
	{
		Column = column;
		RelatedTable = relatedTable;
		RelatedColumn = relatedColumn;
	}

	/// <summary>
	///		Campo de la tabla
	/// </summary>
	internal string Column { get; }

	/// <summary>
	///		Tabla relacionada
	/// </summary>
	internal string RelatedTable { get; }

	/// <summary>
	///		Campo relacionado
	/// </summary>
	internal string RelatedColumn { get; }
}
