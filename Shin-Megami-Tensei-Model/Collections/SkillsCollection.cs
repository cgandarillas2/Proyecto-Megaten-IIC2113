using System.Collections;
using Shin_Megami_Tensei_Model.Skills;

namespace Shin_Megami_Tensei_Model.Collections;

/// <summary>
/// Encapsulates a collection of skills following the "Encapsulate Collections" principle.
/// Provides controlled access to skills without exposing the internal list.
/// </summary>
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

    public int Count => _skills.Count;

    public bool IsEmpty => _skills.Count == 0;

    public ISkill this[int index] => _skills[index];

    public void Add(ISkill skill)
    {
        if (skill == null)
        {
            throw new ArgumentNullException(nameof(skill));
        }
        _skills.Add(skill);
    }

    public void Remove(ISkill skill)
    {
        _skills.Remove(skill);
    }

    public void Clear()
    {
        _skills.Clear();
    }

    public bool Contains(ISkill skill)
    {
        return _skills.Contains(skill);
    }

    public SkillsCollection Where(Func<ISkill, bool> predicate)
    {
        return new SkillsCollection(_skills.Where(predicate));
    }

    public bool Any(Func<ISkill, bool> predicate)
    {
        return _skills.Any(predicate);
    }

    public bool All(Func<ISkill, bool> predicate)
    {
        return _skills.All(predicate);
    }

    /// <summary>
    /// Returns a defensive copy of the internal list.
    /// Use this only when absolutely necessary (e.g., for compatibility with legacy code).
    /// </summary>
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
