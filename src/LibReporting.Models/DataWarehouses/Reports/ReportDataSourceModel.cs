namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;

/// <summary>
///		Origen de datos asociado a un informe
/// </summary>
public class ReportDataSourceModel
{
	public ReportDataSourceModel(ReportModel report, DataSets.BaseDataSourceModel dataSource)
	{
		Report = report;
		DataSource = dataSource;
	}

	/// <summary>
	///		Informe al que se asocia el origen de datos
	/// </summary>
	public ReportModel Report { get; }

	/// <summary>
	///		Origen de datos
	/// </summary>
	public DataSets.BaseDataSourceModel DataSource { get; }

	/// <summary>
	///		Relaciones del origen de datos con las dimensiones
	/// </summary>
	public List<Relations.DimensionRelationModel> Relations { get; } = new();
}
