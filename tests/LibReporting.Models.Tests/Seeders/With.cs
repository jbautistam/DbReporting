using LibReporting.Models.Tests.Seeders.Builders;

namespace LibReporting.Models.Tests.Seeders;

/// <summary>
///		Base para los generadores
/// </summary>
public class With
{
	/// <summary>
	///		Método de acceso
	/// </summary>
	public static With A => new();

	/// <summary>
	///		Generador de esquema
	/// </summary>
	public ReportingSchemaBuilder Schema => new();
}
