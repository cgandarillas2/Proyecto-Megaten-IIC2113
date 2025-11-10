namespace Shin_Megami_Tensei_Model.Exceptions;

public class InvalidTeamException: Exception
{
    public InvalidTeamException(string message) : base(message)
    {
    }

    public InvalidTeamException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}