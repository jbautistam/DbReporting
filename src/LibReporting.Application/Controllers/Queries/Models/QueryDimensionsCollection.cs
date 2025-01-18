namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

/// <summary>
///		Lista de <see cref="QueryDimensionModel"/>
/// </summary>
internal class QueryDimensionsCollection : List<QueryDimensionModel>
{
	/// <summary>
	///		Obtiene los campos solicitados para una dimensión (si no se ha solicitado la dimensión, no lanza una excepción si no que no
	///	defuelve ningún campo)
	/// </summary>
	internal List<string> GetFieldsRequest(string dimensionId, bool includeRequestFields, bool includePrimaryKey)
	{
		List<string> fields = [];
		QueryDimensionModel? queryDimension = Get(dimensionId);

			// Se obtienen los datos
			if (queryDimension is not null)
				foreach (string field in GetListFields(queryDimension, includeRequestFields, includePrimaryKey))
					fields.Add(field);
			// Devuelve la lista de campos
			return fields;
	}

	/// <summary>
	///		Obtiene los campos asociados a una consulta
	/// </summary>
	private List<string> GetListFields(QueryDimensionModel query, bool includeRequestFields, bool includePrimaryKey)
	{
		List<string> fields = [];

			// Añade los campos de la consulta
			foreach (QueryFieldModel field in query.Fields)
				if (!field.IsPrimaryKey || includePrimaryKey || includeRequestFields)
					fields.Add(field.Alias);
			// Añade los campos hijo
			foreach (QueryJoinModel child in query.Joins)
				fields.AddRange(GetListFields(child.Query, false, includePrimaryKey));
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