namespace Bau.Libraries.LibReporting.Requests.Models;

/// <summary>
///		Clase con los datos de una columna solicitada para un listado
/// </summary>
public class ExpressionRequestModel
{
	/// <summary>
	///		Clona los datos de una expresión
	/// </summary>
	public ExpressionRequestModel Clone()
	{
		ExpressionRequestModel cloned = new()
											{
												ReportDataSourceId = ReportDataSourceId
											};

			// Clona las columnas
			foreach (ExpressionColumnRequestModel column in Columns)
				cloned.Columns.Add(column.Clone());
			// Devuelve el objeto clonado
			return cloned;
	}

	/// <summary>
	///		Clave del informe de origen de datos
	/// </summary>
	public string ReportDataSourceId { get; set; } = string.Empty;

	/// <summary>
	///		Columnas solicitada
	/// </summary>
	public List<ExpressionColumnRequestModel> Columns { get; } = new();
}