using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Utils;

namespace Shin_Megami_Tensei_Model.Units;

public class Monster: Unit
{
    private readonly List<string> _skillNames;

    public Monster(
        string name,
        UnitStats baseStats,
        AffinitySet affinities,
        List<string> skillNames)
        : base(name, baseStats, affinities)
    {
        _skillNames = ValidateAndCopySkills(skillNames);
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
        if (skills == null)
        {
            throw new ArgumentNullException(nameof(skills));
        }

        ValidateNoEmptyNames(skills);
        return new List<string>(skills);
    }
    
    private static void ValidateNoEmptyNames(List<string> skills)
    {
        if (skills.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("Skill names cannot be empty");
        }
    }
}