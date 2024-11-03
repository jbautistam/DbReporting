namespace Bau.Libraries.LibReporting.Models.Base;

/// <summary>
///		Lista de <see cref="BaseReportingModel"/>
/// </summary>
public class BaseReportingListModel<TypeData> : List<TypeData> where TypeData : BaseReportingModel
{
	/// <summary>
	///		Busca un elemento
	/// </summary>
	public TypeData? Search(string key)
	{
		// Busca el elemento
		foreach (TypeData item in this)
			if (item.Id.Equals(key, StringComparison.CurrentCultureIgnoreCase))
				return item;
		// Si ha llegado hasta aquí es porque no ha encontrado nada
		return null;
	}

	/// <summary>
	///		Obtiene un valor
	/// </summary>
	public TypeData? this[string key]
	{
		get
		{
			return Search(key);
		}
		set
		{
			TypeData? item = Search(key);

				if (item is not null)
					item = value;
				else if (value is not null)
					Add(value);
		}
	}
}
