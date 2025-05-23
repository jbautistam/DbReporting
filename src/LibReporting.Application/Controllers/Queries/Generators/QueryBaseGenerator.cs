﻿using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Generators;

/// <summary>
///		Clase base para generar laS SQL de <see cref="ParserBaseSectionModel"/>
/// </summary>
internal abstract class QueryBaseGenerator
{
	protected QueryBaseGenerator(ReportQueryGenerator manager)
	{
		Manager = manager;
	}

	/// <summary>
	///		Obtiene la SQL adecuada para esta sección
	/// </summary>
	internal abstract string GetSql();

	/// <summary>
	///		Obtiene la cadena SQL para los campos solicitados de las dimensiones
	/// </summary>
	protected string GetSqlFieldsForDimensions(QueryDimensionCollectionModel queryDimensions, List<ParserDimensionModel> dimensions, bool onlyVisibleFields)
	{
		string sql = string.Empty;

			// Obtiene los campos
			foreach (ParserDimensionModel dimension in dimensions)
			{
				List<QueryDimensionFieldModel> fields = queryDimensions.GetFieldsRequest(dimension.DimensionKey);

					// Añade los campos solicitados a la SQL
					foreach (QueryDimensionFieldModel field in fields)
						if (MustIncludeField(field, dimension.WithPrimaryKeys, dimension.WithRequestedFields, onlyVisibleFields))
						{
							string sqlField = string.Empty;

								// Añade el nombre del campo
								if (dimension.CheckIfNull)
									sqlField = $"IsNull({Manager.SqlTools.GetFieldName(dimension.TableAlias, field.Alias)}, {Manager.SqlTools.GetFieldName(dimension.AdditionalTable, field.Alias)}) AS {field}";
								else
									sqlField = Manager.SqlTools.GetFieldName(dimension.TableAlias, field.Alias);
								// Añade el campo a la cadena SQL
								sql = sql.AddWithSeparator(sqlField, ",");
						}
			}
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Comprueba si se debe incluir el campo en la salida de la cadena SQL
	/// </summary>
	protected bool MustIncludeField(QueryDimensionFieldModel field, bool withPrimaryKeys, bool withRequestedFields, bool onlyVisibleFields)
	{
		if (field.IsPrimaryKey && withPrimaryKeys)
			return true;
		else if (withRequestedFields) 
		{
			if (!onlyVisibleFields || (field.Visible && onlyVisibleFields))
				return true;
			else
				return false;
		}
		else
			return false;
	}

	/// <summary>
	///		Manager
	/// </summary>
	internal ReportQueryGenerator Manager { get; }
}
