namespace Shin_Megami_Tensei.Exceptions;

/// <summary>
/// Exception thrown when there are no valid targets for a skill.
/// </summary>
public class NoValidTargetsException : Exception
{
    public NoValidTargetsException(string unitName)
        : base($"No hay objetivos válidos para {unitName}")
    {
        UnitName = unitName;
    }

    public NoValidTargetsException(string unitName, string message)
        : base(message)
    {
        UnitName = unitName;
    }

    public string UnitName { get; }
}
