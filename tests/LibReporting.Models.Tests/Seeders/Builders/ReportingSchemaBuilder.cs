using Bau.Libraries.LibReporting.Models;

namespace LibReporting.Models.Tests.Seeders.Builders;

/// <summary>
///		Generador de <see cref="ReportingSchemaModel"/>
/// </summary>
public class ReportingSchemaBuilder
{
	/// <summary>
	///		Asigna un <see cref="Bau.Libraries.LibReporting.Models.DataWarehouses.DataWarehouseModel"/>
	/// </summary>
	public DataWarehouseBuilder WithDataWarehouse(string id) => new DataWarehouseBuilder(this, id);

	/// <summary>
	///		Genera el esquema
	/// </summary>
	public ReportingSchemaModel Build() => Schema;

	/// <summary>
	///		Conversor de tipos
	/// </summary>
	public static implicit operator ReportingSchemaModel(ReportingSchemaBuilder builder) => builder.Schema;

	/// <summary>
	///		Esquema definido
	/// </summary>
	public ReportingSchemaModel Schema { get; } = new();
}
