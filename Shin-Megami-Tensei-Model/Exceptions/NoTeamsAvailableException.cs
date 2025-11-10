namespace Shin_Megami_Tensei.Exceptions;

/// Exception thrown when no team files are available to load.
public class NoTeamsAvailableException : Exception
{
    public NoTeamsAvailableException()
        : base("No hay archivos de equipos disponibles")
    {
    }

    public NoTeamsAvailableException(string message)
        : base(message)
    {
    }

    public NoTeamsAvailableException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
