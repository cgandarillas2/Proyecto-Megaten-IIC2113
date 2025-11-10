using System.Collections;
using Shin_Megami_Tensei_Model.Skills;

namespace Shin_Megami_Tensei_Model.Collections;

public class SkillsCollection : IEnumerable<ISkill>
{
    private readonly List<ISkill> _skills;

    public SkillsCollection()
    {
        _skills = new List<ISkill>();
    }

    public SkillsCollection(IEnumerable<ISkill> skills)
    {
        _skills = new List<ISkill>(skills ?? throw new ArgumentNullException(nameof(skills)));
    }

    public SkillsCollection Where(Func<ISkill, bool> predicate)
    {
        return new SkillsCollection(_skills.Where(predicate));
    }
    
    public List<ISkill> ToList()
    {
        return new List<ISkill>(_skills);
    }

    public IEnumerator<ISkill> GetEnumerator()
    {
        return _skills.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static SkillsCollection Empty()
    {
        return new SkillsCollection();
    }
}