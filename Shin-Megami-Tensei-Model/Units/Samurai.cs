using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Stats;

namespace Shin_Megami_Tensei_Model.Units;

public class Samurai: Unit
{
    private const int MaxSkills = 8;
    private readonly List<ISkill> _skills;

    public Samurai(
        string name,
        UnitStats baseStats,
        AffinitySet affinities,
        IEnumerable<ISkill> skills)
        : base(name, baseStats, affinities)
    {
        _skills = ValidateAndCopySkills(skills?.ToList() ?? new List<ISkill>());
    }

    public override SkillsCollection GetSkills()
    {
        return new SkillsCollection(_skills);
    }

    public override SkillsCollection GetSkillsWithEnoughMana()
    {
        var usableSkills = new List<ISkill>();
        for (int i = 0; i < _skills.Count; i++)
        {
            ISkill skill = _skills[i];
            if (skill.Cost <= CurrentStats.CurrentMP)
            {
                usableSkills.Add(skill);
            }
        }
        return new SkillsCollection(usableSkills);
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
        for (int i = 0; i < skills.Count; i++)
        {
            for (int j = i + 1; j < skills.Count; j++)
            {
                if (skills[i].Name == skills[j].Name)
                {
                    throw new ArgumentException("Duplicate skills not allowed");
                }
            }
        }
    }
}