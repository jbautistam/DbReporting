using FluentAssertions;
using LibReporting.Models.Tests.Seeders;
using Bau.Libraries.LibReporting.Models;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

namespace LibReporting.Models.Tests;

/// <summary>
///		Pruebas de <see cref="ReportingSchemaModel"/>
/// </summary>
public class Schema_Should
{
	/// <summary>
	///		Genera el <see cref="ReportingSchemaModel"/>
	/// </summary>
	[Fact]
	public void generate_schema()
	{
		ReportingSchemaModel schema = With.A.Schema
												.WithDataWarehouse("Sales")
													.WithTable("Products", "Dim")
															.WithColumn("Id", null, DataSourceColumnModel.FieldType.Integer,
																		true, false, true, null)
															.WithColumn("Code", "ProductCode", DataSourceColumnModel.FieldType.String,
																		false, true, true, null)
														.Back()
													.WithView("ClassificationLevels", "SELECT * FROM ClassificationLevels")
															.WithColumn("Id", null, DataSourceColumnModel.FieldType.Integer,
																		true, false, true, null)
															.WithColumn("ClassificationLevel1", "ProductClassificationLevel1", DataSourceColumnModel.FieldType.String,
																		false, true, true, null)
															.WithParameter("Start", DataSourceColumnModel.FieldType.Date, null)
														.Back()
											.Back();

			// Comprueba el esquema generado
			schema.DataWarehouses.Count.Should().Be(1);
			schema.DataWarehouses["Sales"].Should().NotBeNull();
			schema.DataWarehouses["Sales"]?.DataSources.Count.Should().Be(2);
	}
}