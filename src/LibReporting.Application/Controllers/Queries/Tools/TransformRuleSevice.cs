using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Transformers;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Tools;

/// <summary>
///		Servicio para aplicación de las reglas
/// </summary>
internal class TransformRuleSevice(List<TransformRuleModel> rules)
{
	/// <summary>
	///		Aplica una serie de reglas sobre una cadena SQL
	/// </summary>
	internal string Apply(string sql)
	{
		// Ordena las reglas de forma que se apliquen antes las reglas con mayor longitud en las cadenas
		Rules.Sort((first, second) => -1 * first.Source.Length.CompareTo(second.Source.Length));
		// Aplica las transformaciones y devuelve el resultado
		return ApplyRules(Split(sql));
	}

	/// <summary>
	///		Divide las cadenas por los apóstrofes
	/// </summary>
	private IEnumerable<string> Split(string source)
	{
		string actual = string.Empty;
		bool atApostrophe = false;

			// Separa las secciones por apóstrofes
			foreach (char chr in source)
			{
				// Añade el carácter a la cadena actual o trata los apóstrofes
				if (chr == '\'')
				{
					if (atApostrophe)
					{
						// Añade un apóstrofe al final
						actual += "'";
						// Devuelve la cadena actual
						yield return actual;
						// Inicializa la cadena actual
						actual = "";
						// Indica que no está en una cadena de apóstrofe
						atApostrophe = false;
					}
					else
					{
						// Devuelve la cadena actual
						if (!string.IsNullOrWhiteSpace(actual))
							yield return actual;
						// Inicializa la cadena actual
						actual = "'";
						// Indica que está en una cadena de apóstrofe
						atApostrophe = true;
					}
				}
				else
					actual += chr;
			}
			// Si queda algo en la cadena la devuelve
			if (!string.IsNullOrWhiteSpace(actual))
				yield return actual;
	}

	/// <summary>
	///		Aplica las reglas a la lista de secciones
	/// </summary>
	private string ApplyRules(IEnumerable<string> sections)
	{
		System.Text.StringBuilder builder = new();

			// Aplica las reglas sobre las secciones
			foreach (string section in sections)
				if (!string.IsNullOrWhiteSpace(section))
				{
					// Aplica las reglas siempre y cuando no comience por apóstrofe
					if (!section.StartsWith('\''))
						builder.Append(ApplyRules(section));
					else
						builder.Append(section);
				}
			// Devuelve la cadena resultante
			return builder.ToString();

		// Aplica las reglas sobre una cadena
		string ApplyRules(string section)
		{
			// Aplica las reglas
			foreach (TransformRuleModel rule in Rules)
				section = section.Replace(rule.Source, rule.Target, StringComparison.CurrentCultureIgnoreCase);
			// Devuelve los datos convertidos
			return section;
		}
	}

	/// <summary>
	///		Reglas que se deben aplicar
	/// </summary>
	internal List<TransformRuleModel> Rules { get; } = rules;
}