using System.Collections;
using Shin_Megami_Tensei_Model.Action;

namespace Shin_Megami_Tensei_Model.Collections;

/// <summary>
/// Encapsulates a collection of skill effects following the "Encapsulate Collections" principle.
/// Provides controlled access to skill effects without exposing the internal list.
/// </summary>
public class SkillEffectsCollection : IEnumerable<SkillEffect>
{
    private readonly List<SkillEffect> _effects;

    public SkillEffectsCollection()
    {
        _effects = new List<SkillEffect>();
    }

    public SkillEffectsCollection(IEnumerable<SkillEffect> effects)
    {
        _effects = new List<SkillEffect>(effects ?? throw new ArgumentNullException(nameof(effects)));
    }

    public int Count => _effects.Count;

    public bool IsEmpty => _effects.Count == 0;

    public SkillEffect this[int index] => _effects[index];

    public void Add(SkillEffect effect)
    {
        if (effect == null)
        {
            throw new ArgumentNullException(nameof(effect));
        }
        _effects.Add(effect);
    }

    public void Remove(SkillEffect effect)
    {
        _effects.Remove(effect);
    }

    public void Clear()
    {
        _effects.Clear();
    }

    public bool Contains(SkillEffect effect)
    {
        return _effects.Contains(effect);
    }

    public SkillEffectsCollection Where(Func<SkillEffect, bool> predicate)
    {
        return new SkillEffectsCollection(_effects.Where(predicate));
    }

    public bool Any(Func<SkillEffect, bool> predicate)
    {
        return _effects.Any(predicate);
    }

    /// <summary>
    /// Returns a defensive copy of the internal list.
    /// Use this only when absolutely necessary (e.g., for compatibility with legacy code).
    /// </summary>
    public List<SkillEffect> ToList()
    {
        return new List<SkillEffect>(_effects);
    }

    public IEnumerator<SkillEffect> GetEnumerator()
    {
        return _effects.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static SkillEffectsCollection Empty()
    {
        return new SkillEffectsCollection();
    }
}
