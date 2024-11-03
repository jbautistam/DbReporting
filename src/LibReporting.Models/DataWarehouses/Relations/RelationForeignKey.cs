namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;

/// <summary>
///		Clave foránea de una relación
/// </summary>
public class RelationForeignKey
{
	/// <summary>
	///		Clona una clave foránea
	/// </summary>
	public RelationForeignKey Clone()
	{
		return new RelationForeignKey
						{
							ColumnId = ColumnId,
							TargetColumnId = TargetColumnId
						};
	}

	/// <summary>
	///		Columna a relacionar
	/// </summary>
	public string ColumnId { get; set; } = string.Empty;

	/// <summary>
	///		Columna relacionada
	/// </summary>
	public string TargetColumnId { get; set; } = string.Empty;
}
