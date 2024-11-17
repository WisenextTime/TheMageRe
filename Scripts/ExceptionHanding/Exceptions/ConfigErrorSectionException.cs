using System;

namespace TheMage.Scripts.ExceptionHanding.Exceptions;

public class ConfigErrorSectionException : Exception
{
	public ConfigErrorSectionException() { }
	public ConfigErrorSectionException(string message) : base(message) { }
	public ConfigErrorSectionException(string message, Exception innerException) : base(message, innerException) { }

	public ConfigErrorSectionException(string filePath, string errorSection)
	{
		throw new ConfigErrorSectionException($"Necessary section '{errorSection}' was not found in file '{filePath}'.");
	}
}