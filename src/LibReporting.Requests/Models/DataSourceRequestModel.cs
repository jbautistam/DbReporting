namespace Bau.Libraries.LibReporting.Requests.Models;

/// <summary>
///		Clase con los datos de un origen de datos solicitado para un listado
/// </summary>
public class DataSourceRequestModel
{
	/// <summary>
	///		Clona los datos de un <see cref="DataSourceRequestModel"/>
	/// </summary>
	public DataSourceRequestModel Clone()
	{
		DataSourceRequestModel cloned = new()
											{
												ReportDataSourceId = ReportDataSourceId
											};

			// Clona las columnas
			foreach (DataSourceColumnRequestModel column in Columns)
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
	public List<DataSourceColumnRequestModel> Columns { get; } = new();
}