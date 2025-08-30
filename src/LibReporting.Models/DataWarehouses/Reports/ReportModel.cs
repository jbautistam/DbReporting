using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
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
	///		Obtiene las dimensiones definidas en un informe (tanto las dimensiones seleccionadas en el informe como sus hijas)
	/// </summary>
	public List<BaseDimensionModel> GetAllDimensions()
	{
		List<BaseDimensionModel> dimensions = [];

			// Añade las dimensiones (intenta que no haya duplicados)
			foreach (BaseDimensionModel dimension in Dimensions)
				foreach (BaseDimensionModel reportDimension in GetReportDimensions(dimension))
					if (!dimensions.Any(item => item.Id.Equals(reportDimension.Id, StringComparison.CurrentCultureIgnoreCase)))
						dimensions.Add(reportDimension);
			// Devuelve la lista de dimensiones
			return dimensions;

		// Obtiene las dimensiones de informe y sus hijos
		List<BaseDimensionModel> GetReportDimensions(BaseDimensionModel dimension)
		{
			List<BaseDimensionModel> result = [];

				// Añade la dimensión
				result.Add(dimension);
				// Añade las dimensiones
				foreach (Relations.DimensionRelationModel relation in dimension.GetRelations())
					if (relation.Dimension is not null)
						result.AddRange(GetReportDimensions(relation.Dimension));
				// Devuelve las dimensiones
				return result;
		}
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
	public List<ReportExpressionModel> Expressions { get; } = [];

	/// <summary>
	///		Bloques del informe
	/// </summary>
	public List<BaseBlockModel> Blocks { get; } = [];
}
