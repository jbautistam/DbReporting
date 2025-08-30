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
			try
			{
				// Genera los archivos
				new Managers.FilesTestGenerator().Generate(txtFile.FileName, txtOutput.PathName);
				// Mensaje al usuario
				MessageBox.Show("End files generation");
			}
			catch (Exception exception)
			{
				MessageBox.Show($"Error when generate requests. {exception.Message}");
			}
	}

	/// <summary>
	///		Comprueba los datos
	/// </summary>
	private bool ValidateData()
	{
		bool validated = false;

			// Comprueba los datos
			if (string.IsNullOrWhiteSpace(txtFile.FileName) || string.IsNullOrWhiteSpace(txtOutput.PathName))
				MessageBox.Show("Enter the schema file name and the output folder");
			else if (!File.Exists(txtFile.FileName))
				MessageBox.Show("Can't find the schema file name");
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