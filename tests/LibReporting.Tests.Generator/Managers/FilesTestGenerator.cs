using Bau.Libraries.LibReporting.Application;
using Bau.Libraries.LibReporting.Models.Base;
using Bau.Libraries.LibReporting.Models.DataWarehouses;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Requests.Models;

namespace LibReporting.Tests.Generator.Managers;

/// <summary>
///		Generador de archivos de solicitud y respuesta de informes
/// </summary>
internal class FilesTestGenerator
{
	// Constantes privadas
	private const string RequestExtension = "request.xml";
	private const string ResponseExtension = "response.sql";

	/// <summary>
	///		Genera los archivos
	/// </summary>
	internal void Generate(string schemaFile, string outputFolder)
	{
		// Carga el archivo de esquema
		LoadSchema(schemaFile);
		// Crea el directorio y borra los archivos de salida
		Directory.CreateDirectory(outputFolder);
		CleanOutputFiles(outputFolder);
		// Genera los archivos de solicitud / respuesta 
		foreach (DataWarehouseModel dataWarehouse in Manager.Schema.DataWarehouses.EnumerateValues())
			foreach (ReportModel report in dataWarehouse.Reports.EnumerateValues())
				GenerateFiles(report, Path.Combine(outputFolder, report.Id));
	}

	/// <summary>
	///		Limpia los archivos de salida
	/// </summary>
	private void CleanOutputFiles(string folder)
	{
		foreach (string file in Directory.GetFiles(folder, $"*.{RequestExtension}"))
			Bau.Libraries.LibHelper.Files.HelperFiles.KillFile(file, false);
		foreach (string file in Directory.GetFiles(folder, $"*.{ResponseExtension}"))
			Bau.Libraries.LibHelper.Files.HelperFiles.KillFile(file, false);
	}

	/// <summary>
	///		Genera los archivos
	/// </summary>
	private void GenerateFiles(ReportModel report, string folder)
	{
		List<ReportRequestModel> requests = GenerateRequests(report, CombineDimensions(report.GetAllDimensions()));

			// Graba las solicitudes
			foreach (ReportRequestModel request in requests)
			{
				string sql = Manager.GetSqlResponse(request);

					// Graba la solicitud
					SaveRequest(request, sql, folder, $"{requests.IndexOf(request):0000}");
			}
	}

	/// <summary>
	///		Combina las dimensiones
	/// </summary>
	/// <remarks>
	/// De la lista 0, 1, 2, se obtienen todas las combinaciones de dimensiones posibles:
	///		[], [0], [1], [2], [01], [012], [02], [12]
	/// Es decir, el número de dimensiones combinadas es 2 ^ dimensiones, para obtenerlas vamos de 0 a 2 ^ dimensiones - 1, pasamos
	/// a binarios y obtenemos todas las dimensiones, por ejemplo, si el valor de contador es 3, sería 011 en binario que recogería
	/// las dimensiones 1 y 2
	/// </remarks>
	private List<List<BaseDimensionModel>> CombineDimensions(List<BaseDimensionModel> dimensions)
	{
		List<List<BaseDimensionModel>> combined = [];

			// Añade un elemento vacío (sin ninguna dimensión)
			combined.Add([]);
			// Obtiene todos los valores combinados (se salta el 0 porque ya hemos añadido un elemento sin ninguna dimensión)
			for (int counter = 1; counter < Math.Min(Math.Pow(2, 64), Math.Pow(2, dimensions.Count)); counter++)
				combined.Add(GetDimensions(dimensions, counter.ToString("b")));
			// Devuelve la lista combinada
			return combined;

		// Obtiene las dimensiones que se corresponden con una cadena en binario
		List<BaseDimensionModel> GetDimensions(List<BaseDimensionModel> dimensions, string binary)
		{
			List<BaseDimensionModel> combined = [];

				// Añade las dimensiones (recorre la cadena al revés para obtener siempre el de peso mínimo al final)
				// Cuando el valor es 1 el binario es 1, cuando el valor es 2 el binario es 10, cuando el valor es 8 el
				// binario es 1000. Es decir, las cadenas binarias obtenidas no tienen la misma longitud
				for (int charIndex = binary.Length - 1, index = 0; charIndex >= 0; charIndex--, index++)
					if (binary[charIndex] == '1')
						combined.Add(dimensions[index]);
				// Devuelve las dimensiones combinadas
				return combined;
		}
	}

	/// <summary>
	///		Genera las solicitudes de un informe
	/// </summary>
	private List<ReportRequestModel> GenerateRequests(ReportModel report, List<List<BaseDimensionModel>> combinedDimensions)
	{
		List<ReportRequestModel> requests = [];

			// Genera las solicitudes (3 páginas: la primera sin paginación)
			for (int page = 0; page < 3; page++)
				requests.AddRange(GeneratePagedRequests(report, combinedDimensions, page));
			// Devuelve las solicitudes
			return requests;
	}

	/// <summary>
	///		Genera las solicitudes paginadas
	/// </summary>
	private List<ReportRequestModel> GeneratePagedRequests(ReportModel report, List<List<BaseDimensionModel>> combinedDimensions, int page)
	{
		List<ReportRequestModel> requests = [];

			foreach (List<BaseDimensionModel> dimensions in combinedDimensions)
			{
				ReportRequestModel request = new(report.DataWarehouse.Id, report.Id);

					// Añade las dimensiones
					request.Dimensions.AddRange(GetRequestDimensions(dimensions));
					// Añade las expresiones
					foreach (ReportExpressionModel expression in report.Expressions)
						request.Expressions.Add(new ColumnRequestModel(expression.Id));
					// Asigna la paginación
					request.Pagination.MustPaginate = page > 0;
					if (page > 0)
					{
						request.Pagination.Page = page;
						request.Pagination.RecordsPerPage = 40;
					}
					// Añade la solicitud a la lista
					requests.Add(request);
			}
			// Devuelve las solicitudes
			return requests;
	}

	/// <summary>
	///		Obtiene los datos de la solicitud a partir de la lista de dimensiones
	/// </summary>
	private List<DataRequestModel> GetRequestDimensions(List<BaseDimensionModel> dimensions)
	{
		List<DataRequestModel> dataRequests = [];

			// Añade un campo de cada dimensión
			foreach (BaseDimensionModel dimension in dimensions)
			{
				DataSourceColumnModel? column = GetFirstColumn(dimension.GetColumns());

					if (column is not null)
					{
						DataRequestModel dimensionRequest = new(dimension.Id);

							// Añade la columna
							dimensionRequest.Columns.Add(new ColumnRequestModel(column.Id)
																	{
																		Visible = true
																	}
														 );
							// Añade la solicitud
							dataRequests.Add(dimensionRequest);
					}
			}
			// Devuelve las solicitudes
			return dataRequests;

		// Obtiene la primera columna
		DataSourceColumnModel? GetFirstColumn(BaseReportingDictionaryModel<DataSourceColumnModel> columns)
		{
			// Obtiene la primera columna visible
			foreach (DataSourceColumnModel column in columns.EnumerateValues())
				if (column.Visible)
					return column;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return null;
		}
	}

	/// <summary>
	///		Graba las solicitudes y respuestas
	/// </summary>
	private void SaveRequest(ReportRequestModel request, string response, string folder, string filePrefix)
	{
		string fileName = Path.Combine(folder, request.ReportId + $"_{filePrefix}.{RequestExtension}");

			// Graba el archivo de solicitud
			new Bau.Libraries.LibReporting.Repository.Xml.Repositories.RequestRepository().Save(fileName, request);
			// Graba el archivo SQL
			fileName = Path.Combine(folder, request.ReportId + $"_{filePrefix}.{ResponseExtension}");
			File.WriteAllText(fileName, response);
	}

	/// <summary>
	///		Carga el esquema de informes
	/// </summary>
	private void LoadSchema(string fileName)
	{
		DataWarehouseModel dataWarehouse = new Bau.Libraries.LibReporting.Repository.Xml.ReportingRepositoryXml().DataWarehouseRepository.Load(fileName, Manager.Schema);

			// Añade el datawarehouse del esquema
			if (dataWarehouse is not null)
				Manager.AddDataWarehouse(dataWarehouse);
	}

	/// <summary>
	///		Manager de reporting
	/// </summary>
	private ReportingManager Manager { get; } = new();
}