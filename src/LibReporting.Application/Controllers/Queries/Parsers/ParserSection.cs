using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers;

/// <summary>
///		Intérprete de secciones
/// </summary>
internal class ParserSection
{
	// Constantes privadas
	private const string HeaderName = "Name";
	private const string HeaderDimension = "Dimension";
	private const string HeaderOrderBy = "OrderBy";
	private const string HeaderExpression = "Expression";
	private const string HeaderSql = "Sql";
	private const string HeaderFields = "Fields";
	private const string HeaderInnerJoin = "InnerJoin";
	private const string HeaderLeftJoin = "LeftJoin";
	private const string HeaderRightJoin = "RightJoin";
	private const string HeaderFullJoin = "FullJoin";
	private const string HeaderCrossJoin = "CrossJoin";
	private const string HeaderWhere = "Where";
	private const string HeaderSubquery = "Subquery";
	private const string HeaderGroupBy = "GroupBy";
	private const string HeaderHaving = "Having";
	private const string HeaderPagination = "Pagination";
	private const string HeaderIfRequest = "IfRequest";
	private const string HeaderPartitionBy = "PartitionBy";
	private const string HeaderWithComma = "WithComma";
	private const string HeaderTable = "Table";
	private const string HeaderAlias = "Alias";
	private const string HeaderOnRequestFields = "OnRequestFields";
	private const string HeaderWithRequestedFields = "WithRequestedFields";
	private const string HeaderOn = "On";
	private const string HeaderAdditionalTable = "AdditionalTable";
	private const string HeaderWithPrimaryKeys = "WithPrimaryKeys";
	private const string HeaderRequired = "Required";
	private const string HeaderCheckIfNull = "CheckIfNull";
	private const string HeaderWhenRequestTotals = "WhenRequestTotals";
	private const string HeaderDefault = "Default";
	private const string HeaderDataSource = "DataSource";
	private const string HeaderOperator = "Operator";
	private const string HeaderAggregation = "Aggregation";
	private const string HeaderNoDimensionSql = "SqlNoDimension";

	internal ParserSection(string sql)
	{	
		Sql = sql;
	}

	/// <summary>
	///		Interpreta las secciones del SQL
	/// </summary>
	internal List<(string marker, ParserBaseSectionModel section)> Parse()
	{
		List<(string marker, ParserBaseSectionModel section)> sections = [];
		List<string> placeholders = Sql.Extract("{{", "}}", false);

			// Interpreta las secciones
			foreach (string placeholder in placeholders)
				if (!string.IsNullOrWhiteSpace(placeholder))
				{
					string alias = $"##{placeholders.IndexOf(placeholder).ToString()}##";
					List<ParserBaseSectionModel> sectionsBase = Parse(placeholder.TrimIgnoreNull());

						// Añade las secciones con su alias
						foreach (ParserBaseSectionModel sectionBase in sectionsBase)
							sections.Add((alias, sectionBase));
						// Sustituye el placeholder de la cadena SQL
						Sql = Sql.Replace(placeholder, alias);
				}
			// Devuelve las secciones generadas
			return sections;
	}

	/// <summary>
	///		Interpreta las líneas
	/// </summary>
	private List<ParserBaseSectionModel> Parse(string content)
	{ 
		List<ParserBaseSectionModel> sections = [];
		BlockInfoCollection blocks = new();

			// Interpreta los bloques
			blocks.Parse(content);
			// Si hay algo que validar
			if (blocks.Blocks.Count == 0)
				throw new Exceptions.ReportingParserException("Can't parse section");
			else
				foreach (BlockInfo block in blocks.Blocks)
					if (block.HasHeader(HeaderFields))
						sections.Add(ParseFields(block));
					else if (block.HasHeader(HeaderInnerJoin))
						sections.Add(ParseJoin(block, ParserJoinSectionModel.JoinType.InnerJoin));
					else if (block.HasHeader(HeaderLeftJoin))
						sections.Add(ParseJoin(block, ParserJoinSectionModel.JoinType.LeftJoin));
					else if (block.HasHeader(HeaderRightJoin))
						sections.Add(ParseJoin(block, ParserJoinSectionModel.JoinType.RightJoin));
					else if (block.HasHeader(HeaderFullJoin))
						sections.Add(ParseJoin(block, ParserJoinSectionModel.JoinType.FullJoin));
					else if (block.HasHeader(HeaderCrossJoin))
						sections.Add(ParseJoin(block, ParserJoinSectionModel.JoinType.CrossJoin));
					else if (block.HasHeader(HeaderWhere))
						sections.Add(ParseWhere(block));
					else if (block.HasHeader(HeaderSubquery))
						sections.Add(ParseSubquery(block));
					else if (block.HasHeader(HeaderGroupBy))
						sections.Add(ParseGroupBy(block));
					else if (block.HasHeader(HeaderHaving))
						sections.Add(ParseHaving(block));
					else if (block.HasHeader(HeaderOrderBy))
						sections.Add(ParseOrderBy(block));
					else if (block.HasHeader(HeaderPagination))
						sections.Add(new ParserPaginationSectionModel());
					else if (block.HasHeader(HeaderIfRequest))
						sections.Add(ParseIfRequestExpression(block));
					else if (block.HasHeader(HeaderPartitionBy))
						sections.Add(ParsePartition(block));
					else
						throw new Exceptions.ReportingParserException($"Section type unknown when parse: {blocks.Blocks[0].Line}");
			// Devuelve la lista de secciones
			return sections;
	}

	/// <summary>
	///		Interpreta una cláusula <see cref="ParserFieldsSectionModel"/>
	/// </summary>
	private ParserFieldsSectionModel ParseFields(BlockInfo block)
	{ 
		ParserFieldsSectionModel fields = new();

			// Carga los datos
			foreach (BlockInfo child in block.Blocks)
				if (child.HasHeader(HeaderDimension))
					fields.ParserDimensions.Add(ParseDimension(child));
				else if (child.HasHeader(HeaderWithComma))
					fields.WithComma = child.GetBooleanValue();
			// Devuelve los datos del campo
			return fields;
	}

	/// <summary>
	///		Interpreta una cláusula <see cref="ParserJoinSectionModel"/>
	/// </summary>
	private ParserJoinSectionModel ParseJoin(BlockInfo block, ParserJoinSectionModel.JoinType type)
	{ 
		ParserJoinSectionModel join = new();

			// Asigna las propiedades
			join.Join = type;
			// Añade los parámetros
			foreach (BlockInfo child in block.Blocks)
				if (child.HasHeader(HeaderDimension))
					join.JoinDimensions.Add(ParseJoinDimension(child.Content, child));
				else if (child.HasHeader(HeaderTable))
					join.Table = child.Content;
				else if (child.HasHeader(HeaderSql))
					join.Sql = child.GetChildsContent();
				else if (child.HasHeader(HeaderNoDimensionSql))
					join.SqlNoDimension = child.GetChildsContent();
				else if (child.HasHeader(HeaderOn)) //TODO: eliminar ... esto es para asegurarnos que la cláusula ON pueda estar fuera de la dimensión
				{
					if (join.JoinDimensions.Count > 0)
						foreach (ParserJoinDimensionSectionModel dimension in join.JoinDimensions)
							if (!dimension.WithRequestedFields && dimension.Fields.Count == 0)
								dimension.AddFieldsJoin(child.Content);
				}
			// Devuelve la cláusula
			return join;
	}

	/// <summary>
	///		Interpreta una dimensión para relacionarla con una tabla en un JOIN
	/// </summary>
	private ParserJoinDimensionSectionModel ParseJoinDimension(string dimensionKey, BlockInfo block)
	{
		ParserJoinDimensionSectionModel dimension = new()
														{
															DimensionKey = dimensionKey
														};

			// Crea los datos de la relación
			foreach (BlockInfo child in block.Blocks)
				if (child.HasHeader(HeaderTable))
					dimension.Table = child.Content;
				else if (child.HasHeader(HeaderAlias))
					dimension.TableAlias = child.Content;
				else if (child.HasHeader(HeaderOnRequestFields))
					dimension.WithRequestedFields = child.GetBooleanValue();
				else if (child.HasHeader(HeaderCheckIfNull))
					dimension.CheckIfNull = child.GetBooleanValue();
				else if (child.HasHeader(HeaderOn))
					dimension.AddFieldsJoin(child.Content);
			// Devuelve la relación
			return dimension;
	}

	/// <summary>
	///		Interpreta una dimensión
	/// </summary>
	private ParserDimensionModel ParseDimension(BlockInfo block)
	{ 
		ParserDimensionModel dimension = new()
											{
												DimensionKey = block.Content ?? string.Empty
											};

			// Obtiene los datos
			foreach (BlockInfo child in block.Blocks)
				if (child.HasHeader(HeaderName))
					dimension.DimensionKey = child.Content;
				else if (child.HasHeader(HeaderTable))
					dimension.Table = child.Content;
				else if (child.HasHeader(HeaderAdditionalTable))
					dimension.AdditionalTable = child.Content;
				else if (child.HasHeader(HeaderAlias))
					dimension.TableAlias = child.Content;
				else if (child.HasHeader(HeaderWithPrimaryKeys))
					dimension.WithPrimaryKeys = child.GetBooleanValue();
				else if (child.HasHeader(HeaderWithRequestedFields) || child.HasHeader(HeaderOnRequestFields))
					dimension.WithRequestedFields = child.GetBooleanValue();
				else if (child.HasHeader(HeaderRequired))
					dimension.Required = child.GetBooleanValue();
				else if (child.HasHeader(HeaderIfRequest))
					dimension.AddRelatedDimensions(child.Content);
				else if (child.HasHeader(HeaderCheckIfNull))
					dimension.CheckIfNull = child.GetBooleanValue();
			// Se solicitan los campos se no se ha solicitado nada
			if (!dimension.WithPrimaryKeys && !dimension.WithRequestedFields)
				dimension.WithRequestedFields = true;
			// Devuelve los datos de la dimensión
			return dimension;
	}

	/// <summary>
	///		Interpreta una subconsulta
	/// </summary>
	private ParserSubquerySectionModel ParseSubquery(BlockInfo block)
	{ 
		ParserSubquerySectionModel section = new();

			// Asigna las propiedades
			section.Name = block.Content;
			// Devuelve la cláusula
			return section;
	}

	/// <summary>
	///		Interpreta una cláusula WHERE
	/// </summary>
	private ParserWhereSectionModel ParseWhere(BlockInfo block)
	{ 
		ParserWhereSectionModel section = new();

			// Asigna las propiedades
			foreach (BlockInfo child in block.Blocks)
				if (child.HasHeader(HeaderDataSource))
					section.DataSources.Add(ParseDataSource(child));
				else if (child.HasHeader(HeaderOperator))
					section.Operator = child.Content.TrimIgnoreNull();
				else if (child.HasHeader(HeaderSql))
					section.Sql = child.GetChildsContent();
			// Devuelve la cláusula
			return section;
	}

	/// <summary>
	///		Interpreta una cláusula HAVING
	/// </summary>
	private ParserHavingSectionModel ParseHaving(BlockInfo block)
	{ 
		ParserHavingSectionModel section = new();

			// Asigna las propiedades
			foreach (BlockInfo child in block.Blocks)
				if (child.HasHeader(HeaderExpression))
					section.Expressions.Add(ParseExpression(child));
				else if (child.HasHeader(HeaderOperator))
					section.Operator = child.Content.TrimIgnoreNull();
				else if (child.HasHeader(HeaderSql))
					section.Sql = child.GetChildsContent();
			// Devuelve la cláusula
			return section;
	}

	/// <summary>
	///		Interpreta una expresión
	/// </summary>
	private ParserExpressionModel ParseExpression(BlockInfo block)
	{
		ParserExpressionModel expression = new()
											{
												Expression = block.Content.TrimIgnoreNull()
											};

			// Interpreta los bloques hijo
			foreach (BlockInfo child in block.Blocks)
				if (child.HasHeader(HeaderTable))
					expression.Table = child.Content.TrimIgnoreNull();
				else if (child.HasHeader(HeaderAggregation))
					expression.Aggregation = child.Content.TrimIgnoreNull();
			// Devuelve los datos de la expresión
			return expression;
	}

	/// <summary>
	///		Interpreta una sección <see cref="ParserDataSourceModel"/>
	/// </summary>
	private ParserDataSourceModel ParseDataSource(BlockInfo block)
	{
		ParserDataSourceModel dataSource = new()
											{
												DataSourceKey = block.Content.TrimIgnoreNull()
											};

			// Obtiene los datos
			foreach (BlockInfo child in block.Blocks)
				if (child.HasHeader(HeaderTable))
					dataSource.Table = child.Content.TrimIgnoreNull();
			// Devuelve el origen de datos
			return dataSource;
	}

	/// <summary>
	///		Interpreta una cláusula GROUP BY
	/// </summary>
	private ParserGroupBySectionModel ParseGroupBy(BlockInfo block)
	{ 
		ParserGroupBySectionModel section = new();

			// Asigna las propiedades
			foreach (BlockInfo child in block.Blocks)
				if (child.HasHeader(HeaderDimension))
					section.Dimensions.Add(ParseDimension(child));
				else if (child.HasHeader(HeaderSql))
					section.Sql = child.GetChildsContent();
			// Devuelve la cláusula
			return section;
	}

	/// <summary>
	///		Interpreta una cláusula ORDER BY
	/// </summary>
	private ParserOrderBySectionModel ParseOrderBy(BlockInfo block)
	{ 
		ParserOrderBySectionModel section = new();

			// Asigna las propiedades
			foreach (BlockInfo child in block.Blocks)
				if (child.HasHeader(HeaderDimension))
					section.Dimensions.Add(ParseDimension(child));
				else if (child.HasHeader(HeaderSql))
					section.Sql = child.GetChildsContent();
				else if (child.HasHeader(HeaderRequired))
					section.Required = child.GetBooleanValue();
			// Devuelve la cláusula
			return section;
	}

	/// <summary>
	///		Interpreta una cláusula IfRequest
	/// </summary>
	private ParserIfRequestSectionModel ParseIfRequestExpression(BlockInfo block)
	{ 
		ParserIfRequestSectionModel section = new();

			// Asigna las propiedades
			foreach (BlockInfo child in block.Blocks)
				if (child.HasHeader(HeaderExpression))
					section.Expressions.Add(ParseRequestExpression(child));
				else if (child.HasHeader(HeaderWhenRequestTotals))
					section.WhenRequestTotals = child.GetBooleanValue();
				else if (child.HasHeader(HeaderWithComma))
					section.WithComma = child.GetBooleanValue();
				else if (child.HasHeader(HeaderSql))
					section.Sql = child.GetChildsContent();
			// Devuelve la cláusula
			return section;
	}

	/// <summary>
	///		Interpreta los datos de una expresión asociada a una sentencia IfRequest
	/// </summary>
	private ParserIfRequestExpressionSectionModel ParseRequestExpression(BlockInfo block)
	{
		ParserIfRequestExpressionSectionModel section = new();

			// Añade las claves de expresión
			section.AddExpressions(block.Content);
			// Añade los datos asociados con el contenido de la expresión
			foreach (BlockInfo child in block.Blocks)
				if (child.HasHeader(HeaderSql))
					section.Sql = child.GetChildsContent();
			// Devuelve la sección interpretada
			return section;
	}

	/// <summary>
	///		Interpreta una cláusula PARTITION BY
	/// </summary>
	private ParserPartitionBySectionModel ParsePartition(BlockInfo block)
	{ 
		ParserPartitionBySectionModel section = new();

			// Asigna las propiedades
			foreach (BlockInfo child in block.Blocks)
				if (child.HasHeader(HeaderDimension))
					section.Dimensions.Add(ParseDimension(child));
				else if (child.HasHeader(HeaderSql))
					section.Sql = child.GetChildsContent();
				else if (child.HasHeader(HeaderOrderBy))
					section.OrderBy = child.GetChildsContent();
			// Devuelve la cláusula
			return section;
	}

	/// <summary>
	///		Reemplaza una sección por una cadena SQL
	/// </summary>
	internal void Replace(string marker, string sql)
	{
		Sql = Sql.Replace(marker, sql);
	}

	/// <summary>
	///		Quita los marcadores
	/// </summary>
	internal void RemoveMarkers()
	{
		while (!string.IsNullOrWhiteSpace(Sql) && (Sql.IndexOf("{{") >= 0 || Sql.IndexOf("}}") >= 0))
		{
			Sql = Sql.Replace("{{", string.Empty);
			Sql = Sql.Replace("}}", string.Empty);
		}
	}

	/// <summary>
	///		Sql
	/// </summary>
	internal string Sql { get; private set; }
}