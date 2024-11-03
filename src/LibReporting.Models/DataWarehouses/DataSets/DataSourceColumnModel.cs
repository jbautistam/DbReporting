namespace Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

/// <summary>
///		Clase con los datos de una columna
/// </summary>
public class DataSourceColumnModel : Base.BaseReportingModel
{
	/// <summary>
	///		Tipo de campo
	/// </summary>
	public enum FieldType
	{
		/// <summary>Desconocido. No se debería utilizar</summary>
		Unknown,
		/// <summary>Cadena</summary>
		String,
		/// <summary>Fecha</summary>
		Date,
		/// <summary>Número entero</summary>
		Integer,
		/// <summary>Número decimal</summary>
		Decimal,
		/// <summary>Valor lógico</summary>
		Boolean,
		/// <summary>Datos binarios</summary>
		Binary
	}
	// Variables privadas
	private string _alias = string.Empty;

	public DataSourceColumnModel(BaseDataSourceModel dataSource)
	{
		DataSource = dataSource;
	}

	public DataSourceColumnModel Clone(BaseDataSourceModel targetDataSource)
	{
		return new DataSourceColumnModel(targetDataSource)
						{
							Id = Id,
							IsPrimaryKey = IsPrimaryKey,
							Alias = Alias,
							Type = Type,
							Visible = Visible,
							Required = Required
						};
	}

	/// <summary>
	///		Compara el valor de dos elementos para ordenarlo
	/// </summary>
	public override int CompareTo(Base.BaseReportingModel item)
	{
		if (item is DataSourceColumnModel column)
			return Id.CompareTo(column.Id);
		else
			return -1;
	}

	/// <summary>
	///		Origen de datos
	/// </summary>
	public BaseDataSourceModel DataSource { get; }

	/// <summary>
	///		Alias del campo
	/// </summary>
	public string Alias
	{
		get 
		{ 
			if (string.IsNullOrWhiteSpace(_alias)) 
				return Id;
			else
				return _alias;
		}
		set { _alias = value; }
	}

	/// <summary>
	///		Indica si esta columna es clave primaria
	/// </summary>
	public bool IsPrimaryKey { get; set; }

	/// <summary>
	///		Tipo del campo
	/// </summary>
	public FieldType Type { get; set; }

	/// <summary>
	///		Fórmula SQL: el campo no es real, es una fórmula sobre otros campos
	/// </summary>
	public string FormulaSql { get; set; } = string.Empty;

	/// <summary>
	///		Indica si la columna es visible
	/// </summary>
	public bool Visible { get; set; } = true;

	/// <summary>
	///		Indica si es obligatoria
	/// </summary>
	public bool Required { get; set; }
}
