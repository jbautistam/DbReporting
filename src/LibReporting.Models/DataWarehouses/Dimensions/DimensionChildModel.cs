using Bau.Libraries.LibReporting.Models.Base;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;

namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;

/// <summary>
///		Clase con los datos de una dimensión declarada en función de otra (por ejemplo, almacén de destino y
///	almacén de origen creados a partir de la dimensión genérica almacén)
/// </summary>
public class DimensionChildModel : BaseDimensionModel
{
	public DimensionChildModel(DataWarehouseModel dataWarehouse, string id, string sourceDimensionId, string columnsPrefix) : base(dataWarehouse)
	{
		Id = id;
		SourceDimensionId = sourceDimensionId;
		ColumnsPrefix = columnsPrefix;
	}

	/// <summary>
	///		Obtiene la dimensión origen
	/// </summary>
	public DimensionModel GetSourceDimension()
	{
		return DataWarehouse.Dimensions[SourceDimensionId] switch
					{
						DimensionModel dimension => dimension,
						null => throw new ArgumentException($"Error at dimension {Id}. Can't find the dimension source {SourceDimensionId}"),
						_ => throw new ArgumentException($"Error at dimension {Id}. The type of dimension source {SourceDimensionId} is not {nameof(DimensionModel)}"),
					};
	}

	/// <summary>
	///		Comprueba si una columna está en esta dimensión o en alguna de sus hijas
	/// </summary>
	public override bool HasColumn(string dimensionId, string columnId)
	{
		DimensionModel dimension = GetSourceDimension();

			// Si se está preguntando por esta dimensión, se pregunta por la dimensión origen
			if (dimensionId.Equals(dimensionId, StringComparison.CurrentCultureIgnoreCase))
				dimensionId = SourceDimensionId;
			// Si la columna comienza por el prefijo, le quita el prefijo
			if (columnId.StartsWith(ColumnsPrefix, StringComparison.CurrentCultureIgnoreCase))
				columnId = columnId[ColumnsPrefix.Length..];
			// Obtiene si la columna está en la dimensión hija
			return dimension.HasColumn(dimensionId, columnId);
	}

	/// <summary>
	///		Obtiene una columna por su Id o por su alias
	/// </summary>
	public override DataSourceColumnModel? GetColumn(string columnId, bool compareWithAlias)
	{
		DimensionModel dimension = GetSourceDimension();

			// Devuelve la columna de la dimension base
			return dimension.GetColumn(columnId, compareWithAlias);
	}

	/// <summary>
	///		Obtiene el nombre de la tabla origen
	/// </summary>
	public override string GetTableFullName() => GetSourceDimension().GetTableFullName();

	/// <summary>
	///		Obtiene el nombre del alias de la tabla origen
	/// </summary>
	public override string GetTableAlias() => ComputePrefixAlias(GetSourceDimension().GetTableAlias());

	/// <summary>
	///		Obtiene el Id del origen de datos
	/// </summary>
	public override string GetDataSourceId() => GetSourceDimension().GetDataSourceId();

	/// <summary>
	///		Obtiene las columnas de la dimensión
	/// </summary>
	public override BaseReportingDictionaryModel<DataSourceColumnModel> GetColumns()
	{
		DimensionModel dimension = GetSourceDimension();
		BaseReportingDictionaryModel<DataSourceColumnModel> sourceColumns = dimension.GetColumns();
		BaseReportingDictionaryModel<DataSourceColumnModel> targetColumns = new();

			// Cambia el alias de las columnas
			foreach (DataSourceColumnModel column in sourceColumns.EnumerateValues())
			{
				DataSourceColumnModel target = column.Clone(dimension.DataSource);

					// Cambia el alias de la columna
					target.Alias = ComputePrefixAlias(column.Alias);
					// y lo añade a la colección de destino
					targetColumns.Add(target);
			}
			// Devuelve las columnas convertidas
			return targetColumns;
	}

	/// <summary>
	///		Obtiene las relaciones asociadas a la dimensión
	/// </summary>
	public override List<DimensionRelationModel> GetRelations()
	{
		DimensionModel dimension = GetSourceDimension();
		List<DimensionRelationModel> sourceRelations = dimension.GetRelations();
		List<DimensionRelationModel> targetRelations = new();

			// Crea las relaciones cambiando los alias
			foreach (DimensionRelationModel relation in sourceRelations)
			{
				DimensionRelationModel target = relation.Clone();

					// Cambia la dimensión de la relación por esta dimensión
					//? ¿Necesitamos cambiar las columnas origen de las claves foráneas? Van por Id, debería ser el mismo
					//target.DimensionId = Id;
					// y la añade a la colección de relaciones
					targetRelations.Add(target);
			}
			// Devuelve las relaciones convertidas
			return targetRelations;
	}

	/// <summary>
	///		Obtiene el alias de una columna
	/// </summary>
	private string ComputePrefixAlias(string field) => $"{ColumnsPrefix}{field}";

	/// <summary>
	///		Id de la dimensión original
	/// </summary>
	public string SourceDimensionId { get; }

	/// <summary>
	///		Prefijos que se ponen a los campos
	/// </summary>
	public string ColumnsPrefix { get; }
}
