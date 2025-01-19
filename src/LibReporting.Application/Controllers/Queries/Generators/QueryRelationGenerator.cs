using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Generators;

/// <summary>
///		Clase para generar la SQL de <see cref="ParserJoinSectionModel"/>
/// </summary>
internal class QueryRelationGenerator : QueryBaseGenerator
{
	// Registros privadas
	private record JoinField(string FieldSource, string FieldTarget);

	internal QueryRelationGenerator(ReportQueryGenerator manager, ParserJoinSectionModel section, QueryDimensionsCollection queryDimensions) : base(manager)
	{
		Section = section;
		QueryDimensions = queryDimensions;
	}

	/// <summary>
	///		Obtiene la SQL
	/// </summary>
	internal override string GetSql()
	{
		if (Section.JoinDimensions.Count == 0)
			throw new Exceptions.ReportingParserException($"Can't find relations at join {Section.Join.ToString()} with table {Section.Table}");
		else
		{
			string sql = GetSql(Section);

				// Añade la SQL adicional o bien la SQL definida para cuando no se encuentran datos de la dimensión
				if (!string.IsNullOrWhiteSpace(sql))
				{
					if (!string.IsNullOrWhiteSpace(Section.Sql))
						sql = sql.AddWithSeparator(Section.Sql, " AND ");
				}
				else if (!string.IsNullOrWhiteSpace(Section.SqlNoDimension))
					sql = $" {GetJoinClause(Section.Join)} {Section.SqlNoDimension}";
				// Devuelve la cadena SQL generada
				return sql;
		}
	}

	/// <summary>
	///		Obtiene la cadena SQL de la dimensión
	/// </summary>
	private string GetSql(ParserJoinSectionModel section)
	{
		string sql = string.Empty;

			// Obtiene las cadenas de unión con las dimensiones
			foreach (ParserJoinDimensionSectionModel joinDimension in section.JoinDimensions)
			{
				QueryDimensionModel? queryDimension = QueryDimensions.Get(joinDimension.DimensionKey);

					// Crea el join con los campos a la consulta
					if (queryDimension is not null)
						sql = sql.AddWithSeparator(GetSqlJoin(section, joinDimension, GetJoinFields(joinDimension, queryDimension)), 
												   Environment.NewLine);
			}
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Obtiene la cadena SQL del JOIN
	/// </summary>
	private string GetSqlJoin(ParserJoinSectionModel section, ParserJoinDimensionSectionModel joinDimension, List<JoinField> joinFields)
	{
		string sql = string.Empty;

			// Añade la SQL de los campos
			foreach (JoinField joinField in joinFields)
				sql = sql.AddWithSeparator(GetFieldCompare(section.Table, joinField.FieldSource, joinDimension.TableAlias, 
														   joinField.FieldTarget, joinDimension.CheckIfNull),
										   Environment.NewLine + "AND");
			// Si hay algún campo, obtiene la cadena final
			if (!string.IsNullOrWhiteSpace(sql))
				sql = $"{GetJoinClause(section.Join)} {GetJoinDimensionTableName(joinDimension)} {Environment.NewLine} ON {sql}";
			// Devuelve la cadena SQL creada
			return sql;

		//TODO: esto se podría combinar con el QueryGroupByGenerator y posiblemente con otros
		// Obtiene una cadena de comparación de campos
		string GetFieldCompare(string tableSource, string fieldSource, string tableTarget, string fieldTarget, bool checkIfNull)
		{
			if (!checkIfNull)
				return $"{Manager.SqlTools.GetFieldName(tableSource, fieldSource)} = {Manager.SqlTools.GetFieldName(tableTarget, fieldTarget)}";
			else //TODO: debería comparar con el valor predeterminado del tipo del campo
				return $"IsNull({Manager.SqlTools.GetFieldName(tableSource, fieldSource)}, '') = IsNull({Manager.SqlTools.GetFieldName(tableTarget, fieldTarget)}, '')";
		}

		// Obtiene el nombre de la tabla de la dimensión para el Join teniendo en cuenta si tiene o no alias
		string GetJoinDimensionTableName(ParserJoinDimensionSectionModel joinDimension)
		{
			if (!string.IsNullOrWhiteSpace(joinDimension.TableAlias) && 
					!joinDimension.TableAlias.Equals(joinDimension.TableAlias, StringComparison.CurrentCultureIgnoreCase))
				return $"{joinDimension.Table} AS {joinDimension.TableAlias}";
			else
				return joinDimension.Table;
		}
	}

	/// <summary>
	///		Obtiene la cadena adecuada para un tipo de JOIN
	/// </summary>
	private string GetJoinClause(ParserJoinSectionModel.JoinType join)
	{
		return join switch
					{
						ParserJoinSectionModel.JoinType.InnerJoin => " INNER JOIN ",
						ParserJoinSectionModel.JoinType.CrossJoin => " CROSS JOIN ",
						ParserJoinSectionModel.JoinType.FullJoin => " FULL OUTER JOIN ",
						ParserJoinSectionModel.JoinType.LeftJoin => " LEFT JOIN ",
						ParserJoinSectionModel.JoinType.RightJoin => " RIGHT JOIN ",
						_ => throw new Exceptions.ReportingParserException($"Join type unknown: {join.ToString()}"),
					};
	}

	/// <summary>
	///		Obtiene los campos que se van a relacionar
	/// </summary>
	private List<JoinField> GetJoinFields(ParserJoinDimensionSectionModel joinDimension, QueryDimensionModel queryDimension)
	{
		if (joinDimension.WithRequestedFields)
			return GetJoinFieldsRequested(queryDimension);
		else if (joinDimension.Fields.Count > 0)
			return GetJoinFieldsWithDimension(joinDimension.Fields);
		else
			return GetJoinFieldsPrimaryKey(queryDimension);
	}

	/// <summary>
	///		Obtiene la lista de campos solicitados para el JOIN
	/// </summary>
	private List<JoinField> GetJoinFieldsRequested(QueryDimensionModel queryDimension)
	{
		List<JoinField> fields = [];

			// Obtiene los campos solicitados
			foreach (QueryFieldModel field in queryDimension.Fields)
				if (!field.IsPrimaryKey)
					fields.Add(new JoinField(field.Alias, field.Alias));
			// Devuelve la lista de campos
			return fields;
	}

	/// <summary>
	///		Obtiene la lista de campos solicitados para el JOIN para la clave primaria de la dimensión
	/// </summary>
	private List<JoinField> GetJoinFieldsPrimaryKey(QueryDimensionModel queryDimension)
	{
		List<JoinField> fields = [];

			// Obtiene los campos solicitados
			foreach (QueryFieldModel field in queryDimension.Fields)
				if (field.IsPrimaryKey)
					fields.Add(new JoinField(field.Alias, field.Alias));
			// Devuelve la lista de campos
			return fields;
	}

	/// <summary>
	///		Obtiene las comparaciones entre campos
	/// </summary>
	private List<JoinField> GetJoinFieldsWithDimension(List<(string fieldDimension, string fieldTable)> requested)
	{
		List<JoinField> fields = [];
		string sql = string.Empty;

			// Añade las comparaciones
			foreach ((string fieldDimension, string fieldTable) in requested)
				fields.Add(new JoinField(fieldTable, fieldDimension));
			// Devuelve los campos
			return fields;			
	}

	/// <summary>
	///		Sección que se está generando
	/// </summary>
	internal ParserJoinSectionModel Section { get; }

	/// <summary>
	///		Dimensiones definidas en la consulta
	/// </summary>
	internal QueryDimensionsCollection QueryDimensions { get; }
}