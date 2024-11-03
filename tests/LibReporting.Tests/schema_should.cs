using FluentAssertions;
using Bau.Libraries.LibReporting.Models.DataWarehouses;


namespace LibReporting.Tests;

/// <summary>
///		Pruebas de generación de informes avanzados
/// </summary>
public class schema_should
{
	/// <summary>
	///		Comprueba si puede cargar los esquemas y se cargan los informes asociados
	/// </summary>
	[Fact]
	public void load_schemas()
	{
		Dictionary<string, List<string>> reports = Tools.FileHelper.GetReports();
		string error = string.Empty;

			// Comprueba todos los esquemas e informes
			foreach (KeyValuePair<string, List<string>> schema in reports)
			{
				Tools.ReportingSolutionManager manager = new();

					// Carga el esqumea
					manager.AddDataWarehouse(schema.Key);
					// Debería tener un dataWarehouse
					if (manager.Manager.Schema.DataWarehouses.Count < 1)
						error += $"Can't find any datawarehouse at {schema.Key}";
					else 
						foreach (DataWarehouseModel dataWarehouse in manager.Manager.Schema.DataWarehouses.EnumerateValues())
							if (dataWarehouse.Reports.Count != schema.Value.Count)
							{
								error += $"The datawarehouse '{dataWarehouse.Name}' have {dataWarehouse.Reports.Count.ToString()}" + Environment.NewLine;
								error += $"and there is {schema.Value.Count.ToString()} at folder";
							}
			}
			// Comprueba los errores
			error.Should().BeNullOrEmpty();
	}
}