namespace Shin_Megami_Tensei.Exceptions;

/// Exception thrown when a unit has no skills available to use.
public class NoSkillsAvailableException : Exception
{
    public NoSkillsAvailableException(string unitName)
        : base($"{unitName} no tiene habilidades disponibles con suficiente MP")
    {
        UnitName = unitName;
    }

    public NoSkillsAvailableException(string unitName, string message)
        : base(message)
    {
        UnitName = unitName;
    }

    public string UnitName { get; }
}
