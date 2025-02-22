namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

/// <summary>
///		Lista de <see cref="QueryDimensionModel"/>
/// </summary>
internal class QueryDimensionCollectionModel : List<QueryDimensionModel>
{
	/// <summary>
	///		Obtiene los campos asociados a una dimensión y sus hijas
	/// </summary>
	internal List<QueryDimensionFieldModel> GetFieldsRequest(string dimensionId)
	{
		List<QueryDimensionFieldModel> fields = [];
		QueryDimensionModel? queryDimension = Get(dimensionId);

			// Se obtienen los datos
			if (queryDimension is not null)
				fields.AddRange(GetFieldsRequest(queryDimension));
			// Devuelve la lista de campos
			return fields;
	}

	/// <summary>
	///		Obtiene los campos asociados a una consulta y sus hijas
	/// </summary>
	private List<QueryDimensionFieldModel> GetFieldsRequest(QueryDimensionModel query)
	{
		List<QueryDimensionFieldModel> fields = [];

			// Añade los campos de la consulta
			foreach (QueryDimensionFieldModel field in query.Fields)
				fields.Add(field);
			// Añade los campos hijo
			foreach (QueryDimensionJoinModel child in query.Joins)
				fields.AddRange(GetFieldsRequest(child.Query));
			// Devuelve los campos
			return fields;
	}

	/// <summary>
	///		Obtiene la consulta de la dimensión
	/// </summary>
	internal QueryDimensionModel? Get(string id) => this.FirstOrDefault(item => item.Dimension.Id.Equals(id, StringComparison.CurrentCultureIgnoreCase));

	/// <summary>
	///		Indica si se ha solicitado una dimensión
	/// </summary>
	internal bool IsRequested(string id) => Get(id) is not null;
}