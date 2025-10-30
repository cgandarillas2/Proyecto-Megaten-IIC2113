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

    public bool HasSkill(string skillName)
    {
        return _skills.Any(s => s.Name == skillName);
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