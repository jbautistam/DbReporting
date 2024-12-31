using Bau.Libraries.LibReporting.Models.DataWarehouses;

namespace LibReporting.Models.Tests.Seeders.Builders;

/// <summary>
///		Generador de <see cref="DataWarehouseModel"/>
/// </summary>
public class DataWarehouseBuilder
{
	public DataWarehouseBuilder(ReportingSchemaBuilder builder, string id)
	{
		// Asigna los objetos
		ReportingSchemaBuilder = builder;
		DataWarehouse = new DataWarehouseModel(builder.Schema)
									{
										Id = id,
										Name = id
									};
		// Añade el almacén al esquema
		ReportingSchemaBuilder.Schema.DataWarehouses.Add(DataWarehouse);
	}

	/// <summary>
	///		Añade una tabla
	/// </summary>
	public DataSourceBuilder WithTable(string id, string schema) => new DataSourceBuilder(this, id, schema, null);

	/// <summary>
	///		Añade una vista
	/// </summary>
	public DataSourceBuilder WithView(string id, string sql) => new DataSourceBuilder(this, id, string.Empty, sql);

	/// <summary>
	///		Añade una dimensión
	/// </summary>
	public DimensionBuilder WithDimension(string id, string dataSource) => new DimensionBuilder(this, id, dataSource);

	/// <summary>
	///		Añade una dimensión hija
	/// </summary>
	public DimensionBuilder WithChildDimension(string id, string sourceDimensionId, string columnsPrefix) => new DimensionBuilder(this, id, sourceDimensionId, columnsPrefix);

	/// <summary>
	///		Devuelve el generador de esquema
	/// </summary>
	public ReportingSchemaBuilder Back() => ReportingSchemaBuilder;

	/// <summary>
	///		Generador de esquema
	/// </summary>
	public ReportingSchemaBuilder ReportingSchemaBuilder { get; }

	/// <summary>
	///		Almacén de datos
	/// </summary>
	public DataWarehouseModel DataWarehouse { get; }
}
