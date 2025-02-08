using Bau.Libraries.LibHelper.Extensors;

namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Información de un bloque para interpretación
/// </summary>
internal class BlockInfo
{
	internal BlockInfo(BlockInfo? parent, int tabs, string line)
	{ 
		Parent = parent;
		Tabs = tabs;
		Line = line;
	}

	/// <summary>
	///		Separa los datos de la línea
	/// </summary>
	internal void Split()
	{ 
		int separatorIndex = Line.IndexOf(':');

			// Si se ha encontrado el separador, se convierte cabecera y contenido
			if (separatorIndex >= 0)
			{ 
				Header = Line[..separatorIndex];
				if (Line.Length >= separatorIndex + 1)
					Content = Line[(separatorIndex + 1)..].TrimIgnoreNull();
			}
			else
				Header = Line;
			// Separa los datos de los hijos
			foreach (BlockInfo block in Blocks)
				block.Split();
	}

	/// <summary>
	///		Obtiene una cadena de depuración
	/// </summary>
	internal string GetDebugString(int indent)
	{ 
		string debug = new string(' ', indent);

			// Añade la cabecera y el contenido
			if (!string.IsNullOrWhiteSpace(Header))
				debug += Header + " -> ";
			if (!string.IsNullOrWhiteSpace(Content))
				debug += Content;
			// Añade el contenido de los hijos
			if (Blocks.Count > 0)
				foreach (BlockInfo block in Blocks)
				{
					// Añade un salto de línea
					debug += Environment.NewLine;
					// Añade la información de depuración de los hijos
					debug += block.GetDebugString(indent + 1);
				}
			// Devuelve la línea creada
			return debug;
	}

	/// <summary>
	///		Comprueba si el bloque tiene una cabecera en concreto
	/// </summary>
	internal bool HasHeader(string key) => !string.IsNullOrWhiteSpace(Header) && Header.Equals(key, StringComparison.CurrentCultureIgnoreCase);

	/// <summary>
	///		Obtiene un valor lógico. Si no se le ha pasado ningún valor, se considera true
	/// </summary>
	internal bool GetBooleanValue()
	{
		string value = Content;

			return string.IsNullOrWhiteSpace(value) || value.Equals("true", StringComparison.CurrentCultureIgnoreCase);
	}

	/// <summary>
	///		Comprueba si existe una clave
	/// </summary>
	internal bool ExistsChildHeader(string key)
	{ 
		// Comprueba las claves de los bloques hijo
		foreach (BlockInfo child in Blocks)
			if (child.HasHeader(key))
				return true;
		// Si ha llegado hasta aquí es porque no ha encontrado nada
		return false;
	}

	/// <summary>
	///		Concatena el contenido de las líneas de los bloques hijo
	/// </summary>
	internal string GetChildsContent() 
	{
		string result = Content;

			// Añade el contenido de los hijos
			foreach (BlockInfo block in Blocks)
				result = result.AddWithSeparator(block.Line.TrimIgnoreNull(), Environment.NewLine, false);
			// Devuelve el contenido
			return result;
	}

	/// <summary>
	///		Bloque padre
	/// </summary>
	internal BlockInfo? Parent { get; }

	/// <summary>
	///		Número de tabuladores o espacios previos a la línea
	/// </summary>
	internal int Tabs { get; }

	/// <summary>
	///		Datos de la línea
	/// </summary>
	internal string Line { get; }

	/// <summary>
	///		Cabecera de la línea
	/// </summary>
	internal string Header { get; private set; } = string.Empty;

	/// <summary>
	///		Contenido de la línea (sin cabecera)
	/// </summary>
	internal string Content { get; private set; } = string.Empty;

	/// <summary>
	///		Bloques hijo
	/// </summary>
	internal List<BlockInfo> Blocks { get; } = [];
}
