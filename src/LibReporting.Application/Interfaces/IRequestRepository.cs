namespace Bau.Libraries.LibReporting.Application.Interfaces;

/// <summary>
///		Interface para los repositorios de <see cref="Requests.Models.ReportRequestModel"/>
/// </summary>
public interface IRequestRepository
{
	/// <summary>
	///		Obtiene un <see cref="Requests.Models.ReportRequestModel"/>
	/// </summary>
	Requests.Models.ReportRequestModel? Get(string id);

	/// <summary>
	///		Obtiene un <see cref="Requests.Models.ReportRequestModel"/> (asíncrono)
	/// </summary>
	Task<Requests.Models.ReportRequestModel?> GetAsync(string id, CancellationToken cancellationToken);

	/// <summary>
	///		Graba un <see cref="Requests.Models.ReportRequestModel"/>
	/// </summary>
	void Update(string id, Requests.Models.ReportRequestModel request);

	/// <summary>
	///		Graba un <see cref="Requests.Models.ReportRequestModel"/> (asíncrono)
	/// </summary>
	Task UpdateAsync(string id, Requests.Models.ReportRequestModel request, CancellationToken cancellationToken);
}
