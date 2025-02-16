namespace Bau.Libraries.LibReporting.Requests.Models;

/// <summary>
///		Clase con los datos de un origen de datos solicitado para un listado
/// </summary>
public class DataSourceRequestModel
{
	/// <summary>
	///		Clave del informe de origen de datos
	/// </summary>
	public string ReportDataSourceId { get; set; } = default!;

	/// <summary>
	///		Columnas solicitadas
	/// </summary>
	public List<ColumnRequestModel> Columns { get; } = [];
}