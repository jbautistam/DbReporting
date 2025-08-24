using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Transformers;

namespace Bau.Libraries.LibReporting.Repository.Xml.Repositories;

/// <summary>
///		Repositorio de <see cref="TransformRuleModel"/> con archivos XML
/// </summary>
public class TransformRuleRepository
{
	// Constantes privadas
	private const string TagRoot = "Rules";
	private const string TagRule = "Rule";
	private const string TagSource = "Source";
	private const string TagTarget = "Target";

	/// <summary>
	///		Carga la lista de <see cref="TransformRuleModel"/> de un archivo
	/// </summary>
	public List<TransformRuleModel> Load(string fileName) => GetFromXml(LibHelper.Files.HelperFiles.LoadTextFile(fileName));

	/// <summary>
	///		Carga la lista de <see cref="TransformRuleModel"/> de una cadena XML
	/// </summary>
	public List<TransformRuleModel> GetFromXml(string xml)
	{
		MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().ParseText(xml);
		List<TransformRuleModel> rules = [];

			// Carga los datos
			if (fileML is not null)
				foreach (MLNode rootML in fileML.Nodes)
					if (rootML.Name == TagRoot)
						foreach (MLNode childML in rootML.Nodes)
							if (childML.Name == TagRule)
							{
								TransformRuleModel rule = new()
															{
																Source = GetNodeOrAttribute(childML, TagSource),
																Target = GetNodeOrAttribute(childML, TagTarget)
															};

									// Comprueba antes de añadir
									if (!string.IsNullOrWhiteSpace(rule.Source) && !string.IsNullOrWhiteSpace(rule.Target))
										rules.Add(rule);
							}
			// Devuelve los datos leidos
			return rules;
	}

	/// <summary>
	///		Obtiene el valor de un nodo o un atributo
	/// </summary>
	private string GetNodeOrAttribute(MLNode nodeML, string tag)
	{
		string result = nodeML.Attributes[tag].Value.TrimIgnoreNull();

			// Obtiene el valor del nodo si el atributo está vacío
			if (string.IsNullOrWhiteSpace(result))
				result = nodeML.Nodes[tag].Value.TrimIgnoreNull();
			// Devuelve el resultado
			return result;
	}

	/// <summary>
	///		Modifica los datos de esquema
	/// </summary>
	public void Update(string id, List<TransformRuleModel> rules)
	{
		MLFile fileML = new();
		MLNode rootML = fileML.Nodes.Add(TagRoot);

			// Añade las reglas
			foreach (TransformRuleModel rule in rules)
			{
				MLNode nodeML = rootML.Nodes.Add(TagRule);

					// Añade los valores de la regla
					rootML.Nodes.Add(nodeML);
			}
			// Graba el archivo
			new LibMarkupLanguage.Services.XML.XMLWriter().Save(id, fileML);
	}
}
