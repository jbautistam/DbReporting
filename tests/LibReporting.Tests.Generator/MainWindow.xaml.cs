using System.Windows;

namespace LibReporting.Tests.Generator;

/// <summary>
///		Ventana principal de la aplicación
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
	}

	/// <summary>
	///		Genera los archivos
	/// </summary>
	private void GenerateFiles()
	{
		if (ValidateData())
			new Managers.FilesTestGenerator().Generate(txtFile.Text, txtOutput.Text);
	}

	/// <summary>
	///		Comprueba los datos
	/// </summary>
	private bool ValidateData()
	{
		bool validated = false;

			// Comprueba los datos
			if (string.IsNullOrWhiteSpace(txtFile.Text) || string.IsNullOrWhiteSpace(txtOutput.Text))
				MessageBox.Show("Introduzca el nombre de archivo y el directorio de salida");
			else if (!System.IO.File.Exists(txtFile.Text))
				MessageBox.Show("Seleccione el archivo de reporting");
			else
				validated = true;
			// Devuelve el valor que indica si los datos son correctos
			return validated;
	}

	private void cmdGenerate_Click(object sender, RoutedEventArgs e)
	{
		GenerateFiles();
	}
}