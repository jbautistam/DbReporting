using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Request.Models;
using Bau.Libraries.LibReporting.Application.Exceptions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries;

/// <summary>
///		Generador de consultas SQL para un informe
/// </summary>
internal class ReportQueryGenerator
{
	// Variables privadas
	private QueryDimensionsCollection _queryDimensions = [];

	internal ReportQueryGenerator(RequestModel request) 
	{
		Request = request;
	}

	/// <summary>
	///		Obtiene la cadena SQL de un informe para responder a una solicitud
	/// </summary>
	internal string GetSql()
	{
		// Normaliza la solicitud de las dimensiones
		Request.Dimensions.Normalize();
		// Devuelve la SQL generada
		return GetSql(GetQueries(Request.Report.Blocks));
	}

	/// <summary>
	///		Obtiene la SQL asociada a una serie de consultas
	/// </summary>
	private string GetSql(List<QuerySqlModel> queries)
	{
		string sql = string.Empty;

			// Obtiene la cadena de la consulta
			foreach (QuerySqlModel query in queries)
				switch (query.Type)
				{
					case QuerySqlModel.QueryType.Block:
							// Si hay alguna CTE se pone el WITH (puede que no se solicite ninguna dimensión)
							if (query.ExistsCte())
								sql += "WITH" + Environment.NewLine;
							// Añade las consultas
							sql += GetSql(query.Queries);
						break;
					case QuerySqlModel.QueryType.Cte:
							int index = queries.IndexOf(query);

								// Añade la Cte a la consulta
								sql += query.Key + " AS " + Environment.NewLine;
								sql += "(" + Environment.NewLine + query.Sql + Environment.NewLine +  ")";
								// Añade una coma si es necesario
								if (index < queries.Count - 1 && queries[index + 1].Type == QuerySqlModel.QueryType.Cte)
									sql += ",";
								// Añade un salto de una línea
								sql += Environment.NewLine;
						break;
					case QuerySqlModel.QueryType.Execution:
					case QuerySqlModel.QueryType.Query:
							sql += query.Sql;
						break;
				}
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Obtiene la cadena SQL de una serie de bloques
	/// </summary>
	private List<QuerySqlModel> GetQueries(List<BaseBlockModel> blocks)
	{
		List<QuerySqlModel> queries = [];

			// Genera el SQL de los bloques
			foreach (BaseBlockModel blockBase in blocks)
				switch (blockBase)
				{
					case BlockWithModel block:
							queries.Add(GetQuery(block));
						break;
					case BlockExecutionModel block:
							queries.Add(GetQuery(block));
						break;
					case BlockQueryModel block:
							queries.Add(GetQuery(block, queries));
						break;
					case BlockCreateCteDimensionModel block:
							QuerySqlModel? query = GetQuery(block);

								if (query is not null)
									queries.Add(query);
						break;
					case BlockCreateCteModel block:
							queries.Add(GetQuery(block));
						break;
					case BlockIfRequest block:
							queries.AddRange(GetQueries(block));
						break;
					default:
						throw new ReportingParserException($"Unknown block type {blockBase.Key}. {blockBase.GetType().ToString()}");
				}
			// Devuelve la lista de consultas
			return queries;
	}

	/// <summary>
	///		Obtiene la cadena SQL de un bloque With
	/// </summary>
	private QuerySqlModel GetQuery(BlockWithModel block)
	{
		QuerySqlModel query = new(QuerySqlModel.QueryType.Block, block.Key, string.Empty);

			// Interpreta los bloques
			query.Queries.AddRange(GetQueries(block.Blocks));
			// Devuelve la consulta
			return query;
	}

	/// <summary>
	///		Obtiene la SQL de un bloque de ejecución
	/// </summary>
	private QuerySqlModel GetQuery(BlockExecutionModel block) => new(QuerySqlModel.QueryType.Execution, block.Key, block.Sql);

	/// <summary>
	///		Obtiene la SQL de un bloque de consulta
	/// </summary>
	private QuerySqlModel GetQuery(BlockQueryModel block, List<QuerySqlModel> queriesBlock)
	{
		ParserSection parser = new(block.Sql);

			// Convierte las secciones
			foreach ((string marker, ParserBaseSectionModel section) in parser.Parse())
				switch (section)
				{ 
					case ParserFieldsSectionModel item:
							parser.Replace(marker, new Generators.QueryFieldsGenerator(this, item, _queryDimensions).GetSql());
						break;
					case ParserJoinSectionModel item:
							parser.Replace(marker, new Generators.QueryRelationGenerator(this, item, _queryDimensions).GetSql());
						break;
					case ParserGroupBySectionModel item:
							parser.Replace(marker, new Generators.QueryGroupByGenerator(this, item, _queryDimensions).GetSql());
						break;
					case ParserOrderBySectionModel item:
							parser.Replace(marker, new Generators.QueryOrderByGenerator(this, item, _queryDimensions).GetSql());
						break;
					case ParserPartitionBySectionModel item:
							parser.Replace(marker, new Generators.QueryPartitionByGenerator(this, item, _queryDimensions).GetSql());
						break;
					case ParserIfRequestSectionModel item:
							parser.Replace(marker, new Generators.QueryIfRequestGenerator(this, item).GetSql());
						break;
					case ParserConditionSectionModel item:
							parser.Replace(marker, new Generators.QueryConditionsGenerator(this, item).GetSql());
						break;
					case ParserSubquerySectionModel item:
							parser.Replace(marker, new Generators.QuerySubqueryGenerator(this, item, queriesBlock).GetSql());
						break;
					case ParserPaginationSectionModel item:
							parser.Replace(marker, new Generators.QueryPaginationGenerator(this, item).GetSql());
						break;
					default:
						throw new ReportingParserException($"Unknown section: {section.GetType().ToString()}");
				}
			// Quita los marcadores vacíos
			parser.RemoveMarkers();
			// Devuelve la consulta
			return new QuerySqlModel(QuerySqlModel.QueryType.Query, block.Key, parser.Sql);
	}

	/// <summary>
	///		Crea la SQL de un bloque de CTE a partir de una dimensión
	/// </summary>
	private QuerySqlModel? GetQuery(BlockCreateCteDimensionModel block)
	{
		BaseDimensionModel? dimension = Request.Report.DataWarehouse.Dimensions[block.DimensionKey];

			// Obtiene la consulta SQL asociada a este bloque si es obligatorio o si se ha solicitado algún campo de esta dimensión
			if (dimension is null)
				throw new ReportingParserException($"Can't find the dimension {block.DimensionKey}");
			else if (block.Required || Request.Dimensions.IsRequested(block.DimensionKey))
			{
				QueryDimensionModel queryDimension = new(this, dimension, block.Fields, block.Filters);
				string sql = queryDimension.Build();
				
					// Añade la consulta de dimensión a la lista interna
					_queryDimensions.Add(queryDimension);
					// Devuelve la consulta
					return new QuerySqlModel(QuerySqlModel.QueryType.Cte, block.Key, sql);
			}
			else
				return null;
	}

	/// <summary>
	///		Crea la SQL de un bloque de CTE
	/// </summary>
	private QuerySqlModel GetQuery(BlockCreateCteModel block)
	{
		List<QuerySqlModel> queries = GetQueries(block.Blocks);
		string sql = string.Empty;

			// Añade los valores de las consultas a la CTE
			foreach (QuerySqlModel query in queries)
				if (!query.IsSubquery)
					sql = sql.AddWithSeparator(query.Sql, Environment.NewLine, false);
			// Devuelve el resultado de la consulta
			if (string.IsNullOrWhiteSpace(sql))
				throw new ReportingParserException($"There is no SQL query in CTE block '{block.Key}'");
			else
				return new QuerySqlModel(QuerySqlModel.QueryType.Cte, block.Key, sql);
	}

	/// <summary>
	///		Crea la sentencia SQL asociada a un bloque que comprueba si se ha solicitado una (o varias) dimensiones
	/// </summary>
	private List<QuerySqlModel> GetQueries(BlockIfRequest block)
	{
		bool mustExecute;

			// Comprueba si se debe ejecutar
			if (block.DimensionKeys.Count > 0)
				mustExecute = Request.Dimensions.IsRequested(block.DimensionKeys);
			else // ... si no hay dimensiones, se pone a true para que se comprueben las expresiones
				mustExecute = true;
			if (block.ExpressionKeys.Count > 0)
				mustExecute &= Request.Expressions.IsRequested(block.ExpressionKeys);
			// Obtiene las consultas
			if (mustExecute)
				return GetQueries(block.Then);
			else
				return GetQueries(block.Else);
	}
	
	/// <summary>
	///		Solicitud
	/// </summary>
	internal RequestModel Request { get; }

	/// <summary>
	///		Herramientas para generación de SQL
	/// </summary>
	internal Tools.SqlTools SqlTools { get; } = new();
}