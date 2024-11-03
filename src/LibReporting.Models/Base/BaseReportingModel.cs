namespace Bau.Libraries.LibReporting.Models.Base;

/// <summary>
///		Base para los modelos de reporting
/// </summary>
public abstract class BaseReportingModel
{
	/// <summary>
	///		Comparación entre elementos
	/// </summary>
	public abstract int CompareTo(BaseReportingModel item);

	/// <summary>
	///		Id global del elemento
	/// </summary>
	public string Id { get; set; } = Guid.NewGuid().ToString();
}
