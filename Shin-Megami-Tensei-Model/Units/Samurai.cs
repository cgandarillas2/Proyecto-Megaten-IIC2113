using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Utils;
namespace Shin_Megami_Tensei_Model.Units;

public class Samurai: Unit
{
    private const int MaxSkills = 8;
    private readonly List<ISkill> _skills;

    public Samurai(
        string name,
        UnitStats baseStats,
        AffinitySet affinities,
        List<ISkill> skills)
        : base(name, baseStats, affinities)
    {
        _skills = ValidateAndCopySkills(skills ?? new List<ISkill>());
    }

    public override List<ISkill> GetSkills()
    {
        return new List<ISkill>(_skills);
    }

    public override List<ISkill> GetSkillsWithEnoughMana()
    {
        var usableSkills = _skills
            .Where(skill => skill.Cost <= CurrentStats.CurrentMP)
            .ToList();
        return usableSkills;
    }

    public bool HasSkill(string skillName)
    {
        return _skills.Any(s => s.Name == skillName);
    }
    
    

    private static List<ISkill> ValidateAndCopySkills(List<ISkill> skills)
    {
        ValidateSkillCount(skills);
        ValidateNoDuplicates(skills);
        return new List<ISkill>(skills);
    }

    private static void ValidateSkillCount(List<ISkill> skills)
    {
        if (skills.Count > MaxSkills)
        {
            throw new ArgumentException($"Maximum {MaxSkills} skills allowed");
        }
    }

    private static void ValidateNoDuplicates(List<ISkill> skills)
    {
        var distinctCount = skills.Select(s => s.Name).Distinct().Count();
        if (distinctCount != skills.Count)
        {
            throw new ArgumentException("Duplicate skills not allowed");
        }
    }
}