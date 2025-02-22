using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Tools;

/// <summary>
///		Generador de SQL para filtros
/// </summary>
internal class SqlFilterGenerator
{
	internal SqlFilterGenerator(SqlTools sqlTools)
	{
		SqlTools = sqlTools;
	}

	/// <summary>
	///		Obtiene la SQL de los filtros para un <see cref="List{RequestFilterModel}"/>
	/// </summary>
	internal string GetSql(string table, string column, List<RequestFilterModel> filters) => GetSql(table, column, string.Empty, filters);

	/// <summary>
	///		Obtiene la SQL de los filtros para un <see cref="List{RequestFilterModel}"/>
	/// </summary>
	internal string GetSql(string table, string column, string aggregation, List<RequestFilterModel> filters)
	{
		string sql = string.Empty;

			// Genera la SQL de los filtros
			foreach (RequestFilterModel filter in filters)
				sql = sql.AddWithSeparator(GetSqlWithFilter(table, column, aggregation, filter), Environment.NewLine + "AND");
			// Devuelve la cadena SQL
			return sql;
	}

	/// <summary>
	///		Obtiene la cadena SQL para un <see cref="RequestFilterModel"/>
	/// </summary>
	internal string GetSql(string table, string column, RequestFilterModel filter) => GetSqlWithFilter(table, column, string.Empty, filter);

	/// <summary>
	///		Obtiene la cadena SQL de un <see cref="RequestFilterModel"/>
	/// </summary>
	internal string GetSqlWithFilter(string table, string column, string aggregation, RequestFilterModel? filter)
	{
		string sql = GetFieldNameWithAggregation(table, column, aggregation);
		
			// Añade el filtro
			if (filter is not null)
				sql = sql.AddWithSeparator(GetCondition(filter.Condition) + " " + GetValues(filter.Condition, filter.Values), " ");
			// Devuelve el filtro
			return sql;
	}

	/// <summary>
	///		Obtiene el nombre de tabla / campo incluyendo la función de agregación si es necesario
	/// </summary>
	private string GetFieldNameWithAggregation(string table, string column, string aggregation)
	{
		string field = SqlTools.GetFieldName(table, column);

			// Añade la función de agregación si es necesario
			if (!string.IsNullOrWhiteSpace(aggregation))
				field = $"{aggregation}({field})";
			// Devuelve el nombre del campo
			return field;
	}

	/// <summary>
	///		Obtiene la cadena SQL de la condición
	/// </summary>
	private string GetCondition(RequestFilterModel.ConditionType condition)
	{
		return condition switch
					{
						RequestFilterModel.ConditionType.Equals => "=",
						RequestFilterModel.ConditionType.Less => "<",
						RequestFilterModel.ConditionType.Greater => ">",
						RequestFilterModel.ConditionType.LessOrEqual => "<=",
						RequestFilterModel.ConditionType.GreaterOrEqual => ">=",
						RequestFilterModel.ConditionType.Contains => "LIKE",
						RequestFilterModel.ConditionType.In => "IN",
						RequestFilterModel.ConditionType.Between => "BETWEEN",
						_ => throw new LibReporting.Models.Exceptions.ReportingException($"Condition unknown: {condition.ToString()}")
					};
	}

	/// <summary>
	///		Obtiene la cadena SQL de los valores
	/// </summary>
	private string GetValues(RequestFilterModel.ConditionType condition, List<object?> values)
	{
		if (values.Count < 1)
			throw new LibReporting.Models.Exceptions.ReportingException("Not defined values for filter");
		else
			return condition switch
						{
							RequestFilterModel.ConditionType.Between => GetValuesBetween(values),
							RequestFilterModel.ConditionType.In => GetValuesIn(values),
							RequestFilterModel.ConditionType.Contains => GetValueLike(values[0]),
							_ => GetValue(values[0])
						};
	}

	/// <summary>
	///		Obtiene la cadena SQL para una condición BETWEEN
	/// </summary>
	private string GetValuesBetween(List<object?> values)
	{
		if (values.Count < 2)
			throw new LibReporting.Models.Exceptions.ReportingException("Not enough values defined for filter BETWEEN");
		else
			return $"{GetValue(values[0])} AND {GetValue(values[1])}";
	}

	/// <summary>
	///		Obtiene la cadena SQL para una condición IN
	/// </summary>
	private string GetValuesIn(List<object?> values)
	{
		string sql = string.Empty;

			// Concatena los valores
			foreach (object? value in values)
				sql = sql.AddWithSeparator(GetValue(value), ",");
			// Devuelve la cadena
			return sql;
	}

	/// <summary>
	///		Obtiene la cadena SQL para una condición LIKE
	/// </summary>
	private string GetValueLike(object? value) => $"'%{GetValue(value, false)}%'";

	/// <summary>
	///		Obtiene la cadena SQL para un valor
	/// </summary>
	private string GetValue(object? value, bool withApostrophe = true)
	{
		return value switch
				{
					null => "NULL",
					byte valueInteger => ConvertIntToSql(valueInteger),
					int valueInteger => ConvertIntToSql(valueInteger),
					short valueInteger => ConvertIntToSql(valueInteger),
					long valueInteger => ConvertIntToSql(valueInteger),
					double valueDecimal => ConvertDecimalToSql(valueDecimal),
					float valueDecimal => ConvertDecimalToSql(valueDecimal),
					decimal valueDecimal => ConvertDecimalToSql((double) valueDecimal),
					string valueString => ConvertStringToSql(valueString, withApostrophe),
					DateTime valueDate => ConvertDateToSql(valueDate),
					bool valueBool => ConvertBooleanToSql(valueBool),
					_ => ConvertStringToSql(value?.ToString() ?? string.Empty, withApostrophe)
				};
	}

	/// <summary>
	///		Convierte un valor lógico a SQL
	/// </summary>
	private string ConvertBooleanToSql(bool value)
	{
		if (value)
			return "1";
		else
			return "0";
	}

	/// <summary>
	///		Convierte una fecha a SQL
	/// </summary>
	private string ConvertDateToSql(DateTime valueDate) => $"'{valueDate:yyyy-MM-dd}'";

	/// <summary>
	///		Convierte un valor decimal a Sql
	/// </summary>
	private string ConvertDecimalToSql(double value) => value.ToString(System.Globalization.CultureInfo.InvariantCulture);

	/// <summary>
	///		Convierte un entero en una cadena
	/// </summary>
	private string ConvertIntToSql(long value) => value.ToString();

	/// <summary>
	///		Convierte una cadena a SQL
	/// </summary>
	private string ConvertStringToSql(string value, bool withApostrophe)
	{
		if (withApostrophe)
			return "'" + value.Replace("'", "''") + "'";
		else
			return value.Replace("'", "''");
	}

	/// <summary>
	///		Herramientas de generación de SQL
	/// </summary>
	internal SqlTools SqlTools { get; }
}