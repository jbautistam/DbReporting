using System.Drawing;

using AnalyticAlways.Helper.Toolbelt.Extensors;
using AnalyticAlways.Xml.Toolbelt;
using Bau.Libraries.LibReporting.Conversors.Services.ReaderToExcel.Models;

namespace Bau.Libraries.LibReporting.Conversors.Services.ReaderToExcel.Repositories;

/// <summary>
///		Repositorio de <see cref="ExcelTemplate"/>
/// </summary>
internal class ExcelTemplateRepository
{
	// Constantes privadas
	private const string TagRoot = "ExcelTemplate";
	private const string TagId = "Id";
	private const string TagSheet = "Sheet";
	private const string TagRecordsPerSheet = "RecordsPerSheet";
	private const string TagWithTotals = "WithTotals";
	private const string TagFreezeHeadersRows = "FreezeHeadersRows";
	private const string TagFreezeColumns = "FreezeColumns";
	private const string TagColumn = "Column";
	private const string TagTitle = "Title";
	private const string TagRequired = "Required";
	private const string TagField = "Field";
	private const string TagTotalField = "TotalField";
	private const string TagDefault = "Default";
	private const string TagType = "Type";
	private const string TagFormat = "Format";
	private const string TagHeaderStyleId = "HeaderStyleId";
	private const string TagCellStyleId = "CellStyleId";
	private const string TagCellTotalStyleId = "TotalStyleId";
	private const string TagStyle = "Style";
	private const string TagWrapText = "WrapText";
	private const string TagHorizontalAlign = "HorizontalAlignment";
	private const string TagVerticalAlignment = "VerticalAlignment";
	private const string TagColor = "Color";
	private const string TagBackground = "Background";
	private const string TagFill = "Fill";
	private const string TagSize = "Size";
	private const string TagBold = "Bold";
	private const string TagBorderTop = "BorderTop";
	private const string TagBorderBottom = "BorderBottom";
	private const string TagBorderRight = "BorderRight";
	private const string TagBorderLeft = "BorderLeft";
	private const string TagPattern = "Pattern";

	/// <summary>
	///		Carga los datos de una plantilla
	/// </summary>
	internal ExcelTemplate? Load(string fileName)
	{
		ExcelTemplate? template = null;
		MLFile fileML = new Xml.Toolbelt.Services.XML.XMLParser().Load(fileName);

			// Carga los datos del archivo
			if (fileML is not null)
				foreach (MLNode rootML in fileML.Nodes)
					if (rootML.Name == TagRoot)
					{
						// Crea la plantilla
						template = new ExcelTemplate(rootML.Attributes[TagId].Value.TrimIgnoreNull())
											{
												Sheet = rootML.Attributes[TagSheet].Value.TrimIgnoreNull(),
												RecordsPerSheet = rootML.Attributes[TagRecordsPerSheet].Value.GetInt(999_998),
												WithTotals = rootML.Attributes[TagWithTotals].Value.GetBool(true),
												FreezeHeadersRows = rootML.Attributes[TagFreezeHeadersRows].Value.GetBool(true),
												FreezeColumns = rootML.Attributes[TagFreezeColumns].Value.GetInt(0)
											};
						// Carga el contenido
						foreach (MLNode nodeML in rootML.Nodes)
							switch (nodeML.Name)
							{
								case TagColumn:
										template.Columns.Add(LoadColumn(template, nodeML));
									break;
								case TagStyle:
										template.Styles.Add(LoadStyle(nodeML));
									break;
							}
					}
			// Devuelve la plantilla cargada
			return template;
	}

	/// <summary>
	///		Carga los datos de una plantilla de columna
	/// </summary>
	private ExcelTemplateColumn LoadColumn(ExcelTemplate template, MLNode rootML)
	{
		return new ExcelTemplateColumn(template)
						{
							Required = rootML.Attributes[TagRequired].Value.GetBool(),
							Title = rootML.Attributes[TagTitle].Value.TrimIgnoreNull(),
							Field = rootML.Attributes[TagField].Value.TrimIgnoreNull(),
							TotalField = rootML.Attributes[TagTotalField].Value.TrimIgnoreNull(),
							Default = rootML.Attributes[TagDefault].Value.TrimIgnoreNull(),
							Type = rootML.Attributes[TagType].Value.GetEnum(ExcelTemplateColumn.OutputType.String),
							Format = rootML.Attributes[TagFormat].Value.TrimIgnoreNull(),
							HeaderStyleId = rootML.Attributes[TagHeaderStyleId].Value.TrimIgnoreNull(),
							CellStyleId = rootML.Attributes[TagCellStyleId].Value.TrimIgnoreNull(),
							TotalStyleId = rootML.Attributes[TagCellTotalStyleId].Value.TrimIgnoreNull()
						};
	}

	/// <summary>
	///		Carga los datos de un estilo
	/// </summary>
	private ExcelTemplateStyle LoadStyle(MLNode rootML)
	{
		ExcelTemplateStyle style = new(rootML.Attributes[TagId].Value.TrimIgnoreNull())
										{
											WrapText = rootML.Attributes[TagWrapText].Value.GetBool(),
											HorizontalAlign = rootML.Attributes[TagHorizontalAlign].Value.GetEnum(ExcelTemplateStyle.HorizontalAlignment.None),
											VerticalAlign = rootML.Attributes[TagVerticalAlignment].Value.GetEnum(ExcelTemplateStyle.VerticalAlignment.None),
											Color = TransformColor(rootML.Attributes[TagColor].Value.TrimIgnoreNull()),
											Background = TransformColor(rootML.Attributes[TagBackground].Value.TrimIgnoreNull()),
											Fill = rootML.Attributes[TagFill].Value.GetEnum(ExcelTemplateStyle.Pattern.None),
											Size = rootML.Attributes[TagSize].Value.GetDouble(10),
											Bold = rootML.Attributes[TagBold].Value.GetBool()
										};

			// Carga el resto de datos
			foreach (MLNode nodeML in rootML.Nodes)
				switch (nodeML.Name)
				{
					case TagBorderTop:
							style.BorderTop = LoadBorder(nodeML);
						break;
					case TagBorderBottom:
							style.BorderBottom = LoadBorder(nodeML);
						break;
					case TagBorderLeft:
							style.BorderLeft = LoadBorder(nodeML);
						break;
					case TagBorderRight:
							style.BorderRight = LoadBorder(nodeML);
						break;
				}
			// Devuelve el estilo
			return style;
	}

	/// <summary>
	///		Carga los datos de un borde
	/// </summary>
	private ExcelTemplateStyleBorder LoadBorder(MLNode nodeML)
	{
		return new ExcelTemplateStyleBorder()
						{
							Color = TransformColor(nodeML.Attributes[TagColor].Value.TrimIgnoreNull()),
							Pattern = nodeML.Attributes[TagPattern].Value.GetEnum(ExcelTemplateStyleBorder.LinePattern.None)
						};	
	}

	/// <summary>
	///		Transforma un color
	/// </summary>
	private Color? TransformColor(string value)
	{
		Color? color = null;

			// Transforma el color
			if (!string.IsNullOrWhiteSpace(value))
			{
				(int red, int green, int blue) = GetColor(value);

					color = Color.FromArgb(red, green, blue);
			}
			// Devuelve el color leido
			return color;

			// Separa los componentes de un color en formato r,g,b
			(int red, int green, int blue) GetColor(string value)
			{
				int red = 0, green = 0, blue = 0;
				string[] parts = value.Split(',');

					// Obtiene los valores de los componentes del color
					if (parts.Length > 0)
						red = Math.Clamp(parts[0].GetInt(0), 0, 255);
					if (parts.Length > 1)
						green = Math.Clamp(parts[1].GetInt(0), 0, 255);
					if (parts.Length > 2)
						blue = Math.Clamp(parts[2].GetInt(0), 0, 255);
					// Devuelve los componentes del color
					return (red, green, blue);
			}
	}
}