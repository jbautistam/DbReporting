using System.Windows;
using System.Windows.Controls;

namespace LibReporting.Tests.Generator.Controls;

/// <summary>
///		Control de usuario para selección de un directorio
/// </summary>
public partial class PathSelect : UserControl
{ 
	// Propiedades
	public static readonly DependencyProperty PathNameProperty = DependencyProperty.Register(nameof(PathName), typeof(string), typeof(PathSelect),
																							 new FrameworkPropertyMetadata(null,
																														   FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
																														   null));
	// Eventos
	public event EventHandler? Changed;

	public PathSelect()
	{
		InitializeComponent();
		grdPathSelect.DataContext = this;
	}

	/// <summary>
	///		Abre el cuadro de diálogo apropiado
	/// </summary>
	private void OpenDialog()
	{
		Microsoft.Win32.OpenFolderDialog dialog = new();

			// Inicializa las propiedads
			dialog.Multiselect = false;
			dialog.Title = "Select a folder";
			dialog.InitialDirectory = PathName;
			// Muestra el diálogo
			if (dialog.ShowDialog() ?? false)
				PathName = dialog.FolderName;
	}

	/// <summary>
	///		Nombre de directorio
	/// </summary>
	public string PathName
	{
		get { return (string) GetValue(PathNameProperty); }
		set
		{
			SetValue(PathNameProperty, value);
			txtPath.Text = value;
			OnChanged();
		}
	}

	/// <summary>
	///		Lanza el evento de modificación
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
