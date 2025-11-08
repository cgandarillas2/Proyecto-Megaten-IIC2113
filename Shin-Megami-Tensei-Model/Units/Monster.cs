using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Utils;

namespace Shin_Megami_Tensei_Model.Units;

public class Monster: Unit
{
    private readonly List<ISkill> _skills;

    public Monster(
        string name,
        UnitStats baseStats,
        AffinitySet affinities,
        List<ISkill> skills)
        : base(name, baseStats, affinities)
    {
        _skills = ValidateAndCopySkills(skills);
    }

    public override List<ISkill> GetSkills()
    {
        return new List<ISkill>(_skills);
    }
    
    public override List<ISkill> GetSkillsWithEnoughMana()
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
        return usableSkills;
    }

    public bool HasSkill(string skillName)
    {
        for (int i = 0; i < _skills.Count; i++)
        {
            if (_skills[i].Name == skillName)
            {
                return true;
            }
        }
        return false;
    }

    private static List<ISkill> ValidateAndCopySkills(List<ISkill> skills)
    {
        if (skills == null)
        {
            throw new ArgumentNullException(nameof(skills));
        }
        return new List<ISkill>(skills);
    }
}