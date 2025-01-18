namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Sección abstracta para las diferentes secciones
/// </summary>
internal abstract class ParserBaseSectionModel
{
	/// <summary>
	///		Separa una cadena por un separador
	/// </summary>
	protected List<string> SplitContent(string content, string? separator = null)
	{
		List<string> parts = [];

			// Si hay algo que separar
			if (!string.IsNullOrWhiteSpace(content))
			{
				// Normaliza el separador
				if (string.IsNullOrWhiteSpace(separator))
					separator = "-";
				// Separa el contenido
				foreach (string part in content.Split(separator))
					if (!string.IsNullOrWhiteSpace(part))
						parts.Add(part.Trim());
			}
			// Devuelve las diferentes partes del contenido
			return parts;
	}
}
