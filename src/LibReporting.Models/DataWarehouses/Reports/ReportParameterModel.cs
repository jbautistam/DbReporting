namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;

/// <summary>
///		Definición de un parámetro para un <see cref="ReportModel"/>
/// </summary>
public class ReportParameterModel : Base.BaseReportingModel
{
	/// <summary>
	///		Compara el valor de dos elementos para ordenarlo
	/// </summary>
	public override int CompareTo(Base.BaseReportingModel item)
	{
		if (item is ReportParameterModel parameter)
			return Id.CompareTo(parameter.Id);
		else
			return -1;
	}

	/// <summary>
	///		Tipo de parámetro
	/// </summary>
	public DataSets.DataSourceColumnModel.FieldType Type { get; set; }

	/// <summary>
	///		Valor predeterminado
	/// </summary>
	public string DefaultValue { get; set; } = default!;
}
