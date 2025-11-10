namespace Shin_Megami_Tensei_Model.Exceptions;

/// Exception thrown when invalid input is requested from the user.
public class InvalidInputRequestException(string message) : ApplicationException(message) { }
