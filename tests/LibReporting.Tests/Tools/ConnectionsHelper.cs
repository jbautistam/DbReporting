namespace LibReporting.Tests.Tools;

/// <summary>
///		Clase de ayuda para tratamiento de conexiones
/// </summary>
internal static class ConnectionsHelper
{
	// Cadena de conexión a la base de datos
	private static Dictionary<string, string> Connections = new()
														{
															{ "Test-Sales.Reporting.Reporting.xml", 
															  "Server=(local);Database=SalesSample;Trusted_Connection=True;MultipleActiveResultSets=True"
															}
													    };

														
	/// <summary>
	///		Obtiene la cadena de conexión correspondiente a un archivo de esquema
	/// </summary>
	public static string GetConnectionStringForSchema(string schemaFile)
	{
		if (Connections.TryGetValue(Path.GetFileName(schemaFile), out string? connectionString))
			return connectionString;
		else
			return string.Empty;
	}
}
