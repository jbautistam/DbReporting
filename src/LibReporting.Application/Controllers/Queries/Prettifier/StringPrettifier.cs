using System.Text;

using Bau.Libraries.LibHelper.Extensors;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Prettifier;

/// <summary>
///		Generador de cadenas
/// </summary>
internal class StringPrettifier
{
	// Variables privadas
	private int _indent;

	/// <summary>
	///		Añade una cadena
	/// </summary>
	internal void Append(string text)
	{
		AppendText(text, 0, string.Empty);
	}

	/// <summary>
	///		Añade una cadena con un máximo de caracteres por fila
	/// </summary>
	internal void Append(string text, int maxCharactersLength, string separator)
	{
		AppendText(text, maxCharactersLength, separator);
	}

	/// <summary>
	///		Añade una nueva línea
	/// </summary>
	internal void NewLine()
	{
		Builder.Append(Environment.NewLine);
	}

	/// <summary>
	///		Incrementa la indentación
	/// </summary>
	internal void Indent()
	{
		_indent++;
	}

	/// <summary>
	///		Decrementa la indentación
	/// </summary>
	internal void Unindent()
	{
		// Quita la indentación
		_indent--;
		// y la normaliza
		if (_indent < 0)
			_indent = 0;
	}

	/// <summary>
	///		Obtiene la indentación
	/// </summary>
	private string GetIndent(int? newIndent = null)
	{
		int indent = newIndent ?? _indent;

			// Devuelve la cadena de indentación
			if (indent == 0)
				return string.Empty;
			else
				return new string('\t', indent);
	}

	/// <summary>
	///		Formatea un texto para que tenga una indentación y se parta por un separador cuando tiene longitud máxima
	/// </summary>
	private void AppendText(string text, int maxCharactersLength, string separator)
	{
		if (!string.IsNullOrWhiteSpace(text))
		{
			string [] parts = text.Split('\r');

				for (int index = 0; index < parts.Length; index++)
				{
					string part = parts[index].TrimIgnoreNull();

						if (!string.IsNullOrWhiteSpace(part))
						{
							// Indenta
							Builder.Append(GetIndent());
							// Añade el texto (sin partir o partido)
							if (maxCharactersLength == 0)
							{
								Builder.Append(part.TrimIgnoreNull());
								if (index < parts.Length - 1)
									NewLine();
							}
							else
								AppendTextLength(part.TrimIgnoreNull(), _indent + 2, maxCharactersLength, separator);
						}
				}
		}
	}

	/// <summary>
	///		Formatea la cadena SQL con su longitud
	/// </summary>
	private void AppendTextLength(string text, int indent, int maxCharactersLength, string separator)
	{
		if (!string.IsNullOrWhiteSpace(text))
		{
			if (text.Length < maxCharactersLength)
				Builder.Append(text);
			else
			{
				int lengthUsed = 0;
				string [] parts = text.Split(separator);

					// Separa la línea por comas y las va añadiendo hasta que ya no tienen espacio
					for (int index = 0; index < parts.Length; index++)
					{
						string part = parts[index].TrimIgnoreNull();

							// Si hemos superado la longitud máxima, saltamos de línea
							if (lengthUsed > maxCharactersLength)
							{
								// Añade un salto de línea
								Builder.Append(Environment.NewLine + GetIndent(indent));
								// Indica que se ha iniciado la línea
								lengthUsed = 0;
							}
							// Añade la cadena y la coma de separación (AddWithSeparator nos pondría la coma en la línea siguiente)
							Builder.Append(part.TrimIgnoreNull());
							if (index < parts.Length - 1)
								Builder.Append(separator + " ");
							// Añade la longitud e indica que ya no es la primera vez
							lengthUsed += part.TrimIgnoreNull().Length + separator.Length + 1;
					}
			}
		}
	}

	/// <summary>
	///		Obtiene la cadena resultante
	/// </summary>
	public override string ToString()
	{
		string[] newLines = [ "\n", "\r", "\r\n" ];
		string sql = Builder.ToString();

			// Quita los saltos de línea duplicados
			while (!string.IsNullOrWhiteSpace(sql) && ExistsDuplicatedString(sql, newLines))
				foreach (string newLine in newLines)
					sql = sql.Replace(newLine + newLine, newLine);
			// Devuelve la cadena
			return sql;
	}

	/// <summary>
	///		Comprueba si existe una cadena por duplicado
	/// </summary>
	private bool ExistsDuplicatedString(string sql, string[] newLines)
	{
		// Comprueba si existe la cadena
		foreach (string newLine in newLines)
			if (sql.Contains(newLine + newLine))
				return true;
		// Si ha llegado hasta aquí es porque no existe
		return false;
	}

	/// <summary>
	///		Generador de cadenas
	/// </summary>
	private StringBuilder Builder { get; } = new();
}
