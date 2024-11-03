namespace Bau.Libraries.LibReporting.Repository.Xml.Repositories;

/// <summary>
///		Clase base para los repositorios
/// </summary>
public abstract class BaseRepository
{
	protected BaseRepository(ReportingRepositoryXml manager)
	{
		Manager = manager;
	}

	/// <summary>
	///		Manager principal
	/// </summary>
	public ReportingRepositoryXml Manager { get; }
}
