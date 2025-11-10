using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Services;


/// Responsible for grouping skill effects by target unit.
public class EffectGrouper
{
    public List<EffectGroup> GroupEffectsByTarget(SkillEffectsCollection effects)
    {
        var groups = new List<EffectGroup>();

        foreach (var effect in effects)
        {
            var existingGroup = groups.FirstOrDefault(g => g.Target == effect.Target);

            if (existingGroup == null)
            {
                existingGroup = new EffectGroup(effect.Target);
                groups.Add(existingGroup);
            }

            existingGroup.AddEffect(effect);
        }

        return groups;
    }

    public EffectGroup FindEffectsForUnit(List<EffectGroup> groups, Unit unit)
    {
        return groups.FirstOrDefault(g => g.Target == unit);
    }

    public List<EffectGroup> GetOtherEffects(List<EffectGroup> groups, Unit actor)
    {
        return groups.Where(g => g.Target != actor).ToList();
    }

    public Unit FindLastTargetWithProperty(List<EffectGroup> groups, Func<SkillEffectsCollection, bool> predicate)
    {
        for (int i = groups.Count - 1; i >= 0; i--)
        {
            if (predicate(groups[i].Effects))
            {
                return groups[i].Target;
            }
        }
        return null;
    }

    public bool HasRepelEffect(SkillEffectsCollection effects)
    {
        return effects.Any(e => e.AffinityResult == Affinity.Repel);
    }

    public bool HasDrainEffect(SkillEffectsCollection effects)
    {
        return effects.Any(e => e.IsDrainEffect());
    }
}

/// Represents a group of effects for a single target.
public class EffectGroup
{
    public Unit Target { get; }
    public SkillEffectsCollection Effects { get; }

    public EffectGroup(Unit target)
    {
        Target = target;
        Effects = new SkillEffectsCollection();
    }

    public void AddEffect(SkillEffect effect)
    {
        Effects.Add(effect);
    }
}
