namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;

/// <summary>
///		Datos de una expresión
/// </summary>
public class ReportExpressionModel
{
	public ReportExpressionModel(ReportModel report, string id, DataSets.DataSourceColumnModel.FieldType type)
	{
		Report = report;
		Id = id;
		Type = type;
	}

	/// <summary>
	///		Informe al que se asocia la expresión
	/// </summary>
	public ReportModel Report { get; }

	/// <summary>
	///		Id de la expresión 
	/// </summary>
	public string Id { get; }

	/// <summary>
	///		Tipo
	/// </summary>
	public DataSets.DataSourceColumnModel.FieldType Type { get; }
}
