using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Request.Models;

/// <summary>
///		Lista de <see cref="RequestFilterModel"/>
/// </summary>
internal class RequestFilterCollectionModel : List<RequestFilterModel>
{
	/// <summary>
	///		Añade un <see cref="List{FilterRequestModel}"/>
	/// </summary>
	internal void AddRange(List<FilterRequestModel> filters)
	{
		foreach (FilterRequestModel filter in filters)
		{
			RequestFilterModel converted = new()
											{
												Condition = Convert(filter.Condition)
											};

				// Añade los valores
				converted.Values.AddRange(filter.Values);
				// Añade el filtro
				Add(converted);
		}

		// Convierte la condición
		RequestFilterModel.ConditionType Convert(FilterRequestModel.ConditionType condition)
		{
			return condition switch
						{
							FilterRequestModel.ConditionType.Equals => RequestFilterModel.ConditionType.Equals,
							FilterRequestModel.ConditionType.Less => RequestFilterModel.ConditionType.Less,
							FilterRequestModel.ConditionType.Greater => RequestFilterModel.ConditionType.Greater,
							FilterRequestModel.ConditionType.LessOrEqual => RequestFilterModel.ConditionType.LessOrEqual,
							FilterRequestModel.ConditionType.GreaterOrEqual => RequestFilterModel.ConditionType.GreaterOrEqual,
							FilterRequestModel.ConditionType.Contains => RequestFilterModel.ConditionType.Contains,
							FilterRequestModel.ConditionType.In => RequestFilterModel.ConditionType.In,
							FilterRequestModel.ConditionType.Between => RequestFilterModel.ConditionType.Between,
							_ => RequestFilterModel.ConditionType.Undefined
						};
		}
	}
}
