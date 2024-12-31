namespace Bau.Libraries.LibReporting.Application.Exceptions;

/// <summary>
///		Excepción de interpretación de un informe
/// </summary>
public class ReportingParserException : Exception
{
	public ReportingParserException()
	{
	}

	public ReportingParserException(string message) : base(message)
	{
	}

	public ReportingParserException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
