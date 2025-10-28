using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Utils;
namespace Shin_Megami_Tensei_Model.Units;

public class Samurai: Unit
{
    private const int MaxSkills = 8;
    private readonly List<string> _skillNames;

    public Samurai(
        string name,
        UnitStats baseStats,
        AffinitySet affinities,
        List<string> skillNames = null)
        : base(name, baseStats, affinities)
    {
        _skillNames = ValidateAndCopySkills(skillNames ?? new List<string>());
    }
    
    public bool HasSkill(string skillName)
    {
        return _skillNames.Contains(skillName);
    }

    public List<string> GetSkillNames()
    {
        return new List<string>(_skillNames);
    }

    private static List<string> ValidateAndCopySkills(List<string> skills)
    {
        ValidateSkillCount(skills);
        ValidateNoDuplicates(skills);
        ValidateNoEmptyNames(skills);
        return new List<string>(skills);
    }
    
    private static void ValidateSkillCount(List<string> skills)
    {
        if (skills.Count > MaxSkills)
        {
            throw new ArgumentException($"Maximum {MaxSkills} skills allowed");
        }
    }

    private static void ValidateNoDuplicates(List<string> skills)
    {
        var hasDuplicates = skills.Count != skills.Distinct().Count();
        if (hasDuplicates)
        {
            throw new ArgumentException("Duplicate skills not allowed");
        }
    }

    private static void ValidateNoEmptyNames(List<string> skills)
    {
        if (skills.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("Skill names cannot be empty");
        }
    }
}