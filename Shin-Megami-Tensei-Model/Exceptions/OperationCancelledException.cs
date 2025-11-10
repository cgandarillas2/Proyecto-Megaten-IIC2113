namespace Shin_Megami_Tensei.Exceptions;

/// Exception thrown when a user cancels an operation.
public class OperationCancelledException : Exception
{
    public OperationCancelledException()
        : base("La operación fue cancelada por el usuario")
    {
    }

    public OperationCancelledException(string message)
        : base(message)
    {
    }

    public OperationCancelledException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
