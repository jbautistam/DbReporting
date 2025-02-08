using System.Data;
using System.Globalization;

using AnalyticAlways.Helper.Toolbelt.Extensors;
using Bau.Libraries.LibReporting.Conversors.Services.ReaderToJson.Models;

namespace Bau.Libraries.LibReporting.Conversors.Services.ReaderToJson.Builders;

/// <summary>
///     Generador de datos de una plantilla
/// </summary>
internal class JsonTemplateBuilder
{
    /// <summary>
    ///     Tipo del resultado
    /// </summary>
    private enum OutputType
    {
        String,
        DateTime,
        Int,
        Decimal,
        Boolean
    }

    internal JsonTemplateBuilder(ReaderToJsonService manager, JsonTemplateSection section)
    { 
        Manager = manager;
        Section = section;
    }

    /// <summary>
    ///     Añade los datos de un registro a la plantilla
    /// </summary>
    internal void Add(IDataReader reader)
    {
        if (Section is not null && !string.IsNullOrWhiteSpace(Section.Content))
        {
            // Añade el separador para Json
            if (Builder.Length > 0)
                Builder.AppendLine(",");
            // Añade los datos
            Builder.AppendLine(ApplyTemplate(Section.Content, reader, string.Empty));
        }
    }

    /// <summary>
    ///     Obtiene la cadena resultante
    /// </summary>
    internal string GetString() => Builder.ToString();

    /// <summary>
    ///     Aplica los datos del registro a la plantilla
    /// </summary>
    private string ApplyTemplate(string template, IDataReader reader, string variablePrefix)
    {
        List<string> variables = template.Extract("{{", "}}");

            // Extrae el valor de las variables
            foreach (string variable in variables)
                if (variable.StartsWith("Template:", StringComparison.CurrentCultureIgnoreCase))
                    template = template.Replace("{{" + variable + "}}", ApplyExternalTemplate(variable, reader));
                else if (variable.StartsWith("Subtemplate:", StringComparison.CurrentCultureIgnoreCase))
                    template = template.Replace("{{" + variable + "}}", ApplyInternalTemplate(variable, reader));
                else
                {
                    // Si no se le ha pasado ningún prefijo, se coge una cadena vacía (para evitar errores en el Replace)
                    if (string.IsNullOrWhiteSpace(variablePrefix))
                        variablePrefix = string.Empty;
                    // Sustituye el valor del marcador por el valor del registro
                    template = template.Replace("{{" + variable + "}}", 
                                                GetVariableValue(variable.Replace("[Prefix]", variablePrefix, StringComparison.CurrentCultureIgnoreCase), 
                                                                    reader));
                }
            // Devuelve el valor resultante
            return template;
    }
    
    /// <summary>
    ///     Aplica el conenido de una plantilla externa
    /// </summary>
	private string ApplyExternalTemplate(string marker, IDataReader reader)
	{
        string keyTemplate = GetKeyTemplate(marker);

            if (string.IsNullOrEmpty(keyTemplate))
                throw new ArgumentException("Template name is empty (Use {{Template: key}})");
            else
            {
                JsonTemplateSection? externalSection = Manager.GetTemplate(keyTemplate)?.GetSection(Section.Type);

                    // Si no se ha definido ninguna sección externa, no se lanza un error porque puede
                    // que se haya dejado vacía a propósito. Si se define una sección, se aplican las
                    // variables a la plantilla
                    if (externalSection is null)
                        return string.Empty;
                    else
                        return ApplyTemplate(externalSection.Content, reader, string.Empty);
            }

            // Obtiene la clave de la plantilla a partir de un marcador
            // El marcador debería ser: Template: Key
            string GetKeyTemplate(string marker)
            {
                string[] parts = marker.Split(':');

                    if (parts.Length == 2)
                        return parts[1].TrimIgnoreNull();
                    else
                        return string.Empty;
            }
	}
    
    /// <summary>
    ///     Aplica el conenido de una plantilla interna
    /// </summary>
	private string ApplyInternalTemplate(string marker, IDataReader reader)
	{
        (string keyTemplate, string variablePrefix) = GetKeyTemplate(marker);

            if (string.IsNullOrEmpty(keyTemplate))
                throw new ArgumentException("Template name is empty (Use {{Template: key}})");
            else
            {
                JsonTemplateSection externalSection = Section.Template.GetSection(JsonTemplateSection.SectionType.Subtemplate,
                                                                              keyTemplate);

                    // Si no se ha definido ninguna sección externa, no se lanza un error porque puede
                    // que se haya dejado vacía a propósito. Si se define una sección, se aplican las
                    // variables a la plantilla
                    if (string.IsNullOrWhiteSpace(externalSection.Content))
                        return string.Empty;
                    else
                        return ApplyTemplate(externalSection.Content, reader, variablePrefix);
            }

            // Obtiene la clave de la plantilla y un prefijo de variable a partir de un marcador
            // El marcador debería ser: Template: Key,Prefix
            (string keyTemplate, string variablePrefix) GetKeyTemplate(string marker)
            {
                string keyTemplate = string.Empty, variablePrefix = string.Empty;
                string[] parts = marker.Split(':');

                    if (parts.Length == 2)
                    {
                        // Obtiene la clave de la plantilla
                        keyTemplate = parts[1].TrimIgnoreNull();
                        // Si existe, obtiene el prefijo de la variable
                        if (keyTemplate.IndexOf(',') >= 0)
                        {
                            // Separa la clave en dos
                            parts = keyTemplate.Split(',');
                            // Obtiene los dos valores: clave y prefijo
                            if (parts.Length >= 2)
                            {
                                keyTemplate = parts[0].TrimIgnoreNull();
                                variablePrefix = parts[1].TrimIgnoreNull();
                            }
                        }
                    }
                    // Devuelve los valores localizados
                    return (keyTemplate, variablePrefix);
            }
	}

	/// <summary>
	///     Obtiene el valor de la variable
	/// </summary>
	private string GetVariableValue(string variable, IDataReader reader)
    {
        (string field, OutputType type, string defaultValue) = GetParts(variable);

            // Obtiene el valor del registro
            if (!ExistsField(reader, field))
                return defaultValue;
            else
                return ConvertToJson(type, reader.GetValue(reader.GetOrdinal(field)), defaultValue);
    }

    /// <summary>
    ///     Convierte un objeto a Json
    /// </summary>
    private string ConvertToJson(OutputType type, object value, string defaultValue)
    {
        // Convierte el valor leido
        if (!(value is DBNull) && value != null)
            switch (type)
            {
                case OutputType.String:
                    return '"' + value.ToString() + '"';
                case OutputType.Int:
                    return Convert.ToInt64(value).ToString(CultureInfo.InvariantCulture);
                case OutputType.Decimal:
                    return Convert.ToDouble(value).ToString(CultureInfo.InvariantCulture);
                case OutputType.DateTime:
                    if (value is DateTime date)
                        return $"\"{date:yyyy-MM-ddTHH:mm:ss}\"";
                    else
                        return defaultValue;
                case OutputType.Boolean:
                    if (value is bool boolean)
                        return boolean.ToString().ToLower();
                    else if (value is double decimalValue)
                        return (decimalValue > 0).ToString().ToLower();
                    else if (string.IsNullOrWhiteSpace(defaultValue))
                        return defaultValue;
                    else
                        return "false";
            }
        // Si ha llegado hasta aquí es poruqe no lo ha podido convertir
        return defaultValue;
    }

    /// <summary>
    ///     Comprueba si existe un campo en el <see cref="IDataReader"/>
    /// </summary>
    private bool ExistsField(IDataReader reader, string field)
    {
        // Comprueba los campos
        for (int column = 0; column < reader.FieldCount; column++)
            if (reader.GetName(column).Equals(field, StringComparison.CurrentCultureIgnoreCase))
                return true;
        // Si ha llegado hasta aquí es porque no ha encontrado el nombre del campo
        return false;
    }

    /// <summary>
    ///     Obtiene las diferentes partes que componen la variable
    /// </summary>
    private (string field, OutputType type, string defaultValue) GetParts(string variable)
    {
        string[] parts = variable.Split(',');
        string field = string.Empty;
        OutputType type = OutputType.String;
        string defaultValue = "null";

            // Obtiene las secciones de la variable
            if (parts.Length > 0)
                field = parts[0].TrimIgnoreNull();
            // Obtiene el tipo de la variable
            if (parts.Length > 1)
                type = parts[1].TrimIgnoreNull().GetEnum(OutputType.String);
            // Obtiene el valor predeterminado
            if (parts.Length > 2)
                defaultValue = parts[2].TrimIgnoreNull();
            // Devuelve las diferentes partes
            return (field, type, defaultValue);
    }

    /// <summary>
    ///     Servicio de conversión a Json
    /// </summary>
    internal ReaderToJsonService Manager { get; }

    /// <summary>
    ///     Sección
    /// </summary>
    private JsonTemplateSection Section { get; }

    /// <summary>
    ///     Generador
    /// </summary>
    private System.Text.StringBuilder Builder { get; } = new();
}