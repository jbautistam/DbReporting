namespace Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

/// <summary>
///		Origen de datos a partir de una tabla
/// </summary>
public class DataSourceTableModel : BaseDataSourceModel
{
	public DataSourceTableModel(DataWarehouseModel dataWarehouse) : base(dataWarehouse) {}

	/// <summary>
	///		Compara el valor de dos elementos para ordenarlo
	/// </summary>
	public override int CompareTo(Base.BaseReportingModel item)
	{
		if (item is DataSourceTableModel table)
			return FullName.CompareTo(table.FullName);
		else
			return -1;
	}

	/// <summary>
	///		Clona un <see cref="DataSourceTableModel"/>
	/// </summary>
	public DataSourceTableModel Clone(DataWarehouseModel target)
	{
		DataSourceTableModel dataSource = new(target)
												{
													Schema = Schema,
													Table = Table
												};

			// Clona las columnas
			foreach (DataSourceColumnModel column in Columns.EnumerateValues())
				dataSource.Columns.Add(column.Clone(dataSource));
			// Devuelve el origen de datos clonado
			return dataSource;
	}

	/// <summary>
	///		Obtiene el nombre completo del origen de datos
	/// </summary>
	private string GetFullName()
	{
		string charStart = "[", charEnd = "]";
		string fullName = string.Empty;

			// Añade el esquema
			fullName += NormalizeName(Schema, charStart, charEnd);
			// Añade el nombre
			if (!string.IsNullOrWhiteSpace(Table))
			{
				// Añade el separador
				if (!string.IsNullOrWhiteSpace(fullName))
					fullName += ".";
				// Añade el nombre
				fullName += NormalizeName(Table, charStart, charEnd);
			}
			// Devuelve el nombre completo
			return fullName; 

			// Normaliza un nombre
			string NormalizeName(string value, string charStart, string charEnd)
			{
				if (string.IsNullOrWhiteSpace(value))
					return string.Empty;
				else
					return $"{charStart}{value.Trim()}{charEnd}";
			}
	}

	/// <summary>
	///		Obtiene el nombre completo de la tabla o la SQL del contenido si es una vista
	/// </summary>
	public override string GetTableFullNameOrContent() => FullName;

	/// <summary>
	///		Obtiene el nombre de tabla (sin el esquema)
	/// </summary>
	public override string GetTableAlias() => Table;

	/// <summary>
	///		Esquema origen
	/// </summary>
	public string Schema { get; set; } = default!;

	/// <summary>
	///		Tabla origen
	/// </summary>
	public string Table { get; set; } = default!;

	/// <summary>
	///		Nombre completo
	/// </summary>
	public string FullName => GetFullName();

	/// <summary>
	///		Indica si es un origen de datos establecido sobre una vista
	/// </summary>
	public bool IsView { get; set; }
}
