namespace Shin_Megami_Tensei_Model.Exceptions;

/// <summary>
/// Exception thrown when invalid input is requested from the user.
/// </summary>
public class InvalidInputRequestException(string message) : ApplicationException(message) { }
