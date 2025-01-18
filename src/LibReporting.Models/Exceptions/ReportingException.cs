namespace Bau.Libraries.LibReporting.Models.Exceptions;

/// <summary>
///		Excepción interna
/// </summary>
public class ReportingException : Exception
{
	public ReportingException() : base() {}

	public ReportingException(string message) : base(message) {}

	public ReportingException(string message, Exception innerException) : base(message, innerException) {}
}
