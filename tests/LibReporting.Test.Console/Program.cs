using LibReporting.Test.Console.Managers;

string schemaFile = @"C:\OneDrivePersonal\OneDrive\Documentos\Reporting\NewReports\Sql Server - Reporting - Test.reporting.xml";

ConfigurationModel configuration = new(schemaFile);

string sql = new SqlTestManager(configuration).GetSqlFile(ConfigurationModel.ReportType.Transfers);

LibReporting.Test.Console.WindowsClipboard.SetText(sql);
Console.WriteLine(sql);
