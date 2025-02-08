using System.Data;

using AnalyticAlways.Libraries.LibExcel;
using Bau.Libraries.LibReporting.Conversors.Services.ReaderToExcel.Models;

namespace Bau.Libraries.LibReporting.Conversors.Services.ReaderToExcel.Builders;

/// <summary>
///		Generador de archivos Excel
/// </summary>
internal class ExcelFileBuilder
{
	internal ExcelFileBuilder(ExcelTemplate template)
	{
		Template = template;
	}

	/// <summary>
	///		Genera el archivo Excel
	/// </summary>
	internal void Generate(string fileName, IDataReader reader)
	{
		ExcelBuilder excelBuilder = new(fileName);
		ExcelSheetBuilder? sheetBuilder = null;
		int worksheetIndex = 0, row = 2, records = 0;

			// Genera los datos
			while (reader.Read())
			{
				// Crea una nueva hoja si es necesario
				if (sheetBuilder is null)
				{
					// Crea el generador
					sheetBuilder = excelBuilder.WithWorkSheet(GetSheetName(Template.Sheet, worksheetIndex++));
					// Genera las cabeceras
					CreateHeader(sheetBuilder, reader);
					row = 2;
					// Genera los totales
					if (Template.WithTotals && worksheetIndex == 1)
					{
						// Genera la fila de cabecera
						CreateTotals(sheetBuilder, row, reader);
						// Incrementa la fila
						row = 3;
					}
					// Fija las filas y columnas especificadas en la plantilla
					if (Template.FreezeHeadersRows)
						sheetBuilder.WithFreezeRows(row - 1);
					if (Template.FreezeColumns > 0)
						sheetBuilder.WithFreezeColumns(Template.FreezeColumns);
				}
				// Crea la fila
				CreateRow(sheetBuilder, row++, reader);
				// Si se supera el número de registros por hoja, se pasa a una nueva hoja
				if (++records > Template.RecordsPerSheet)
				{
					// Ajusta las columnas
					if (sheetBuilder is not null)
						sheetBuilder.WithAutoAdjustColumns();
					// Indica que se debe crear una nueva hoja
					sheetBuilder = null;
					records = 0;
				}
			}
			// Ajusta las columnas
			if (sheetBuilder is not null)
				sheetBuilder.WithAutoAdjustColumns();
			// Graba el archivo
			excelBuilder.Save();
	}

	/// <summary>
	///		Obtiene el nombre de la hoja
	/// </summary>
	private string GetSheetName(string sheet, int workSheetIndex)
	{
		if (workSheetIndex == 0)
			return sheet;
		else
			return $"{sheet} {workSheetIndex.ToString()}";
	}

	/// <summary>
	///		Crea la cabecera
	/// </summary>
	private void CreateHeader(ExcelSheetBuilder sheetBuilder, IDataReader reader)
	{
		int columnIndex = 1;

			// Crea las celdas de la cabecera
			foreach (ExcelTemplateColumn column in Template.Columns)
				if (column.Required || Exists(column.Field, reader))
				{
					// Asigna los valores
					sheetBuilder.WithCell(1, columnIndex)
								.WithValue(column.Title);
					// Aplica el estilo de cabeceras
					ApplyStyle(sheetBuilder.WithCell(1, columnIndex), column.HeaderStyleId);
					// Incrementa el índice de columna
					columnIndex++;
				}
	}

	/// <summary>
	///		Genera la línea de totales
	/// </summary>
	private void CreateTotals(ExcelSheetBuilder sheetBuilder, int row, IDataReader reader)
	{
		int columnIndex = 1;

			foreach (ExcelTemplateColumn column in Template.Columns)
				if (column.Required || Exists(column.Field, reader))
				{
					// Asigna el valor del total
					if (!string.IsNullOrWhiteSpace(column.TotalField) && Exists(column.TotalField, reader))
						sheetBuilder.WithCell(row, columnIndex)
									.WithValue(GetValue(column.TotalField, column, reader));
					// Aplica el estilo de total
					ApplyStyle(sheetBuilder.WithCell(1, columnIndex), column.TotalStyleId);
					// Incrementa el índice de columna
					columnIndex++;
				}
	}

	/// <summary>
	///		Genera la línea de la fila
	/// </summary>
	private void CreateRow(ExcelSheetBuilder sheetBuilder, int row, IDataReader reader)
	{
		int columnIndex = 1;

			foreach (ExcelTemplateColumn column in Template.Columns)
				if (column.Required || Exists(column.Field, reader))
				{
					// Asigna el valor de la celda
					sheetBuilder.WithCell(row, columnIndex)
									.WithValue(GetValue(column.Field, column, reader));
					// Aplica el estilo de la celda
					ApplyStyle(sheetBuilder.WithCell(1, columnIndex), column.CellStyleId);
					// Incrementa el índice de columna
					columnIndex++;
				}
	}

	/// <summary>
	///		Asigna el valor
	/// </summary>
	private object GetValue(string field, ExcelTemplateColumn column, IDataReader reader)
	{	
		object value = reader.GetValue(reader.GetOrdinal(field));

			// Ajusta el valor si es nulo
			if (value is null || value is DBNull)
			{
				if (!string.IsNullOrWhiteSpace(column.Default))
					value = column.Default;
				else
					value = "";
			}
			// Devuelve el valor
			return value;
	}

	/// <summary>
	///		Comprueba si existe una columna en el <see cref="IDataReader"/>
	/// </summary>
	private bool Exists(string field, IDataReader reader)
	{
		// Comprueba si existe el campo en el lector
		for (int index = 0; index < reader.FieldCount; index++)
			if (reader.GetName(index).Equals(field, StringComparison.CurrentCultureIgnoreCase))
				return true;
		// Si ha llegado hasta aquí es porque no existe
		return false;
	}

	/// <summary>
	///		Aplica el estilo a una celda
	/// </summary>
	private void ApplyStyle(ExcelCellBuilder cellBuilder, string styleId)
	{
		ExcelTemplateStyle? style = Template.GetStyle(styleId);

			if (style is not null)
			{
				if (style.HorizontalAlign != ExcelTemplateStyle.HorizontalAlignment.None)
					cellBuilder.WithHorizontalAlign(Convert(style.HorizontalAlign));
				if (style.VerticalAlign != ExcelTemplateStyle.VerticalAlignment.None)
					cellBuilder.WithVerticalAlign(Convert(style.VerticalAlign));
				if (style.Color is not null)
					cellBuilder.WithColor(style.Color);
				if (style.Background is not null)
					cellBuilder.WithBackground(style.Background);
				cellBuilder.WithSize(style.Size);
				if (style.Bold)
					cellBuilder.WithBold(style.Bold);
				if (style.Fill != ExcelTemplateStyle.Pattern.None)
					cellBuilder.WithPattern(Convert(style.Fill));
				if (style.BorderTop is not null)
					cellBuilder.WithBorderTop(style.BorderTop.Color, Convert(style.BorderTop.Pattern));
				if (style.BorderBottom is not null)
					cellBuilder.WithBorderBottom(style.BorderBottom.Color, Convert(style.BorderBottom.Pattern));
				if (style.BorderLeft is not null)
					cellBuilder.WithBorderLeft(style.BorderLeft.Color, Convert(style.BorderLeft.Pattern));
				if (style.BorderRight is not null)
					cellBuilder.WithBorderRight(style.BorderRight.Color, Convert(style.BorderRight.Pattern));
			}
	}

	/// <summary>
	///		Convierte la alineación horizontal
	/// </summary>
	private ExcelBuilder.HorizontalAlignment Convert(ExcelTemplateStyle.HorizontalAlignment horizontalAlign)
	{
		return horizontalAlign switch
					{
						ExcelTemplateStyle.HorizontalAlignment.Center => ExcelBuilder.HorizontalAlignment.Center,
						ExcelTemplateStyle.HorizontalAlignment.Left => ExcelBuilder.HorizontalAlignment.Left,
						ExcelTemplateStyle.HorizontalAlignment.Right => ExcelBuilder.HorizontalAlignment.Right,
						_ => ExcelBuilder.HorizontalAlignment.Unknown
					};
	}

	/// <summary>
	///		Convierte la alineación vertical
	/// </summary>
	private ExcelBuilder.VerticalAlignment Convert(ExcelTemplateStyle.VerticalAlignment verticalAlign)
	{
		return verticalAlign switch
					{
						ExcelTemplateStyle.VerticalAlignment.Top => ExcelBuilder.VerticalAlignment.Top,
						ExcelTemplateStyle.VerticalAlignment.Bottom => ExcelBuilder.VerticalAlignment.Bottom,
						ExcelTemplateStyle.VerticalAlignment.Middle => ExcelBuilder.VerticalAlignment.Center,
						_ => ExcelBuilder.VerticalAlignment.Unknown
					};
	}

	/// <summary>
	///		Convierte el patrón de relleno
	/// </summary>
	private ExcelBuilder.Pattern Convert(ExcelTemplateStyle.Pattern fill)
	{
		return fill switch
				{
					ExcelTemplateStyle.Pattern.Solid => ExcelBuilder.Pattern.Solid,
					ExcelTemplateStyle.Pattern.DarkDown => ExcelBuilder.Pattern.DarkDown,
					ExcelTemplateStyle.Pattern.DarkGray => ExcelBuilder.Pattern.DarkGray,
					ExcelTemplateStyle.Pattern.DarkGrid => ExcelBuilder.Pattern.DarkGrid,
					ExcelTemplateStyle.Pattern.DarkHorizontal => ExcelBuilder.Pattern.DarkHorizontal,
					ExcelTemplateStyle.Pattern.DarkTrellis => ExcelBuilder.Pattern.DarkTrellis,
					ExcelTemplateStyle.Pattern.DarkUp => ExcelBuilder.Pattern.DarkUp,
					ExcelTemplateStyle.Pattern.DarkVertical => ExcelBuilder.Pattern.DarkVertical,
					ExcelTemplateStyle.Pattern.Gray0625 => ExcelBuilder.Pattern.Gray0625,
					ExcelTemplateStyle.Pattern.Gray125 => ExcelBuilder.Pattern.Gray125,
					ExcelTemplateStyle.Pattern.LightDown => ExcelBuilder.Pattern.LightDown,
					ExcelTemplateStyle.Pattern.LightGray => ExcelBuilder.Pattern.LightGray,
					ExcelTemplateStyle.Pattern.LightGrid => ExcelBuilder.Pattern.LightGrid,
					ExcelTemplateStyle.Pattern.LightHorizontal => ExcelBuilder.Pattern.LightHorizontal,
					ExcelTemplateStyle.Pattern.LightTrellis => ExcelBuilder.Pattern.LightTrellis,
					ExcelTemplateStyle.Pattern.LightUp => ExcelBuilder.Pattern.LightUp,
					ExcelTemplateStyle.Pattern.LightVertical => ExcelBuilder.Pattern.LightVertical,
					ExcelTemplateStyle.Pattern.MediumGray => ExcelBuilder.Pattern.MediumGray,
					_ => ExcelBuilder.Pattern.None
				};
	}

	/// <summary>
	///		Convierte el patrón de línea
	/// </summary>
	private ExcelBuilder.LinePattern Convert(ExcelTemplateStyleBorder.LinePattern pattern)
	{
		return pattern switch
				{
					ExcelTemplateStyleBorder.LinePattern.DashDot => ExcelBuilder.LinePattern.DashDot,
					ExcelTemplateStyleBorder.LinePattern.DashDotDot => ExcelBuilder.LinePattern.DashDotDot,
					ExcelTemplateStyleBorder.LinePattern.Dashed => ExcelBuilder.LinePattern.Dashed,
					ExcelTemplateStyleBorder.LinePattern.Dotted => ExcelBuilder.LinePattern.Dotted,
					ExcelTemplateStyleBorder.LinePattern.Double => ExcelBuilder.LinePattern.Double,
					ExcelTemplateStyleBorder.LinePattern.Hair => ExcelBuilder.LinePattern.Hair,
					ExcelTemplateStyleBorder.LinePattern.Medium => ExcelBuilder.LinePattern.Medium,
					ExcelTemplateStyleBorder.LinePattern.MediumDashDot => ExcelBuilder.LinePattern.MediumDashDot,
					ExcelTemplateStyleBorder.LinePattern.MediumDashDotDot => ExcelBuilder.LinePattern.MediumDashDotDot,
					ExcelTemplateStyleBorder.LinePattern.MediumDashed => ExcelBuilder.LinePattern.MediumDashed,
					ExcelTemplateStyleBorder.LinePattern.SlantDashDot => ExcelBuilder.LinePattern.SlantDashDot,
					ExcelTemplateStyleBorder.LinePattern.Thick => ExcelBuilder.LinePattern.Thick,
					ExcelTemplateStyleBorder.LinePattern.Thin => ExcelBuilder.LinePattern.Thin,
					_ => ExcelBuilder.LinePattern.None
				};
	}

	/// <summary>
	///		Plantilla
	/// </summary>
	private ExcelTemplate Template { get; }
}
