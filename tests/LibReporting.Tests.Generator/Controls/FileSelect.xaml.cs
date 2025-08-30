using System.Windows;
using System.Windows.Controls;

namespace LibReporting.Tests.Generator.Controls;

/// <summary>
///		Control de usuario para selección de un archivo
/// </summary>
public partial class FileSelect : UserControl
{
	/// <summary>
	///		Modo de selección del archivo
	/// </summary>
	public enum ModeType
	{
		/// <summary>Cargar</summary>
		Load,
		/// <summary>Grabar</summary>
		Save
	}

	// Propiedades
	public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(nameof(FileName), typeof(string), typeof(FileSelect),
																							 new FrameworkPropertyMetadata(string.Empty,
																														   FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
	public static readonly DependencyProperty PathBaseProperty = DependencyProperty.Register(nameof(PathBase), typeof(string), typeof(FileSelect),
																							 new FrameworkPropertyMetadata(string.Empty,
																														   FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
	public static readonly DependencyProperty MaskProperty = DependencyProperty.Register(nameof(Mask), typeof(string), typeof(FileSelect),
																						 new FrameworkPropertyMetadata("All files (*.*)|*.*",
																													   FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
	public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof(Mode), typeof(ModeType), typeof(FileSelect),
																						 new FrameworkPropertyMetadata(ModeType.Load,
																													   FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
	// Eventos
	public event EventHandler? Changed;

	public FileSelect()
	{
		InitializeComponent();
		grdFileSelect.DataContext = this;
	}

	/// <summary>
	///		Abre el cuadro de diálogo apropiado
	/// </summary>
	private void OpenDialog()
	{
		string? path, extension = string.Empty;
		string? fileName = FileName;

			// Obtiene el directorio
			if (!string.IsNullOrEmpty(fileName))
			{
				path = Path.GetDirectoryName(fileName);
				fileName = Path.GetFileName(fileName);
				extension = Path.GetExtension(fileName);
			} 
			else
				path = PathBase;
			// Abre el cuadro de diálogo apropiado
			if (Mode == ModeType.Load)
				fileName = OpenDialogLoad(path, Mask, fileName, extension);
			else
				fileName = OpenDialogSave(path, Mask, fileName, extension);
			// Asigna el nombre de archivo
			if (!string.IsNullOrEmpty(fileName))
				FileName = fileName;
	}

	/// <summary>
	///		Abre el cuadro de diálogo de carga de archivos
	/// </summary>
	private string? OpenDialogLoad(string? defaultPath, string filter, string? defaultFileName = null, string? defaultExtension = null)
	{
		Microsoft.Win32.OpenFileDialog file = new();

			// Asigna las propiedades
			file.InitialDirectory = defaultPath;
			file.FileName = defaultFileName;
			file.DefaultExt = defaultExtension;
			file.Filter = filter;
			// Muestra el cuadro de diálogo
			if (file.ShowDialog() ?? false)
				return file.FileName;
			else
				return null;
	}

	/// <summary>
	///		Abre el cuadro de diálogo de grabación de archivos
	/// </summary>
	private string? OpenDialogSave(string? defaultPath, string filter, string? defaultFileName = null, string? defaultExtension = null)
	{
		Microsoft.Win32.SaveFileDialog file = new();

			// Asigna las propiedades
			file.InitialDirectory = defaultPath;
			file.FileName = defaultFileName;
			file.DefaultExt = defaultExtension;
			file.Filter = filter;
			// Muestra el cuadro de diálogo
			if (file.ShowDialog() ?? false)
				return file.FileName;
			else
				return null;
	}

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName
	{
		get { return (string) GetValue(FileNameProperty); }
		set
		{
			SetValue(FileNameProperty, value);
			OnChanged();
		}
	}

	/// <summary>
	///		Directorio base
	/// </summary>
	public string PathBase
	{
		get { return (string) GetValue(PathBaseProperty); }
		set
		{
			SetValue(PathBaseProperty, value);
			OnChanged();
		}
	}

	/// <summary>
	///		Máscara de archivo
	/// </summary>
	public string Mask
	{
		get { return (string) GetValue(MaskProperty); }
		set { SetValue(MaskProperty, value); }
	}

	/// <summary>
	///		Modo de selección de archivo
	/// </summary>
	public ModeType Mode
	{
		get
		{
			object property = GetValue(ModeProperty);

				if (property is ModeType mode)
					return mode;
				else
					return ModeType.Load;
		}
		set { SetValue(ModeProperty, ((int) value).ToString()); }
	}

	/// <summary> 
	///		Lanza el evento Changed
	/// </summary> 
	protected virtual void OnChanged()
	{
		Changed?.Invoke(this, EventArgs.Empty);
	}

	private void cmdSelect_Click(object sender, RoutedEventArgs e)
	{
		OpenDialog();
	}
}