using System.Collections;
using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Skills;

namespace Shin_Megami_Tensei_Model.Collections;

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

    public SkillEffect this[int index] => _effects[index];

    public void Add(SkillEffect effect)
    {
        if (effect == null)
        {
            throw new ArgumentNullException(nameof(effect));
        }
        _effects.Add(effect);
    }
    
    public bool Any(Func<SkillEffect, bool> predicate)
    {
        return _effects.Any(predicate);
    }
    
    public IEnumerator<SkillEffect> GetEnumerator()
    {
        return _effects.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
