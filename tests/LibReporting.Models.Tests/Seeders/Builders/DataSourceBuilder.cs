using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

namespace LibReporting.Models.Tests.Seeders.Builders;

/// <summary>
///		Generador de <see cref="BaseDataSourceModel"/>
/// </summary>
public class DataSourceBuilder
{
	public DataSourceBuilder(DataWarehouseBuilder dataWarehouseBuilder, string id, string schema, string? sql)
	{
		// Asigna los objetos
		DataWarehouseBuilder = dataWarehouseBuilder;
		// Crea el origen de datos
		if (string.IsNullOrWhiteSpace(sql))
			DataSource = new DataSourceTableModel(dataWarehouseBuilder.DataWarehouse)
									{
										Id = id,
										Schema = schema,
										Table = id
									};
		else
			DataSource = new DataSourceSqlModel(dataWarehouseBuilder.DataWarehouse)
									{
										Id = id,
										Sql = sql
									};
		// Añade el origen de datos al almacén
		DataWarehouseBuilder.DataWarehouse.DataSources.Add(DataSource);
	}

	/// <summary>
	///		Añade una columna al origen de datos
	/// </summary>
	public DataSourceBuilder WithColumn(string name, string? alias, DataSourceColumnModel.FieldType type,
										bool isPrimaryKey, bool isVisible, bool isRequired, string? formulaSql)
	{
		// Añade la columna al origen de datos
		DataSource.Columns.Add(new DataSourceColumnModel(DataSource)
										{
											Id = name,
											Alias = alias ?? string.Empty,
											Type = type,
											IsPrimaryKey = isPrimaryKey,
											Visible = isVisible,
											Required = isRequired,
											FormulaSql = formulaSql
										}
							  );
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Añade un parámetro al origen de datos
	/// </summary>
	public DataSourceBuilder WithParameter(string name, DataSourceColumnModel.FieldType type,
										   string? defaultValue)
	{
		// Añade la columna al origen de datos
		if (DataSource is DataSourceSqlModel dataSource)
			dataSource.Parameters.Add(new DataSourceSqlParameterModel
											{
												Name = name,
												Type = type,
												DefaultValue = defaultValue
											}
								  );
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Devuelve el <see cref="DataWarehouseBuilder"/> padre
	/// </summary>
	public DataWarehouseBuilder Back() => DataWarehouseBuilder;

	/// <summary>
	///		Generador padre
	/// </summary>
	public DataWarehouseBuilder DataWarehouseBuilder { get; }

	/// <summary>
	///		Origen de datos
	/// </summary>
	public BaseDataSourceModel DataSource { get; }
}
