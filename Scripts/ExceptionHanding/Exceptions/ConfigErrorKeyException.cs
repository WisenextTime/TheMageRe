using System;

namespace TheMage.Scripts.ExceptionHanding.Exceptions;

public class ConfigErrorKeyException : Exception
{
	public ConfigErrorKeyException() { }
	public ConfigErrorKeyException(string message) : base(message) { }
	public ConfigErrorKeyException(string message, Exception innerException) : base(message, innerException) { }
	public ConfigErrorKeyException(string filePath, string errorSection, string errorKey)
	{
		throw new ConfigErrorKeyException(
			$"Necessary key '{errorKey} in section '{errorSection}' was not found in file '{filePath}'.'");
	}
}