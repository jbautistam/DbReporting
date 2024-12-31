using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;

namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;

/// <summary>
///		Clase con los datos de un informe
/// </summary>
public class ReportModel : Base.BaseReportingModel
{
	public ReportModel(DataWarehouseModel dataWarehouse, string fileName)
	{
		DataWarehouse = dataWarehouse;
		FileName = fileName;
	}

	/// <summary>
	///		Compara el valor de dos elementos para ordenarlo
	/// </summary>
	public override int CompareTo(Base.BaseReportingModel item)
	{
		if (item is ReportModel report)
			return Id.CompareTo(report.Id);
		else
			return -1;
	}

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName { get; }

	/// <summary>
	///		Descripción del <see cref="ReportModel"/>
	/// </summary>
	public string Description { get; set; } = default!;

	/// <summary>
	///		<see cref="DataWarehouseModel"/> al que se asocia este <see cref="ReportModel"/>
	/// </summary>
	public DataWarehouseModel DataWarehouse { get; }

	/// <summary>
	///		Parámetros del informe
	/// </summary>
	public Base.BaseReportingListModel<ReportParameterModel> Parameters { get; } = [];

	/// <summary>
	///		Dimensiones asociadas al informe
	/// </summary>
	public List<Dimensions.BaseDimensionModel> Dimensions { get; } = [];

	/// <summary>
	///		Orígenes de datos asociados a un informe
	/// </summary>
	public List<ReportDataSourceModel> DataSources { get; } = [];

	/// <summary>
	///		Datos asociados a las solicitudes, por ejemplo, dimensiones que se deben solicitar o campos que se deben
	///	solicitar de forma conjunta
	/// </summary>
	public List<ReportRequestDimension> RequestDimensions { get; } = [];

	/// <summary>
	///		Expresiones del informe
	/// </summary>
	public List<string> Expressions { get; } = [];

	/// <summary>
	///		Bloques del informe
	/// </summary>
	public List<BaseBlockModel> Blocks { get; } = [];
}
