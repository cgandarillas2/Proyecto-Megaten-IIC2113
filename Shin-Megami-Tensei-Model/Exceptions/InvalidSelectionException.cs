namespace Shin_Megami_Tensei.Exceptions;

/// Exception thrown when user input is invalid for a selection.
public class InvalidSelectionException : Exception
{
    public InvalidSelectionException()
        : base("Selección inválida")
    {
    }

    public InvalidSelectionException(string message)
        : base(message)
    {
    }

    public InvalidSelectionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
