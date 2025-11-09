using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_View.ConsoleLib;

public class SkillResultView
{
    private readonly View _view;

    public SkillResultView(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    public void Present(Unit actor, SkillResult result)
    {
        var effectsByTarget = GroupEffectsByTarget(result.Effects);
        var actorEffects = FindEffectsForUnit(effectsByTarget, actor);
        var otherEffects = GetOtherEffects(effectsByTarget, actor);

        var lastRepelTarget = FindLastTargetWithProperty(otherEffects, HasRepelEffect);
        var lastDrainTarget = FindLastTargetWithProperty(otherEffects, HasDrainEffect);

        DisplayOtherTargetEffects(actor, otherEffects, lastRepelTarget, lastDrainTarget);
        DisplayActorEffects(actor, actorEffects);
        DisplayMessages(result.Messages);
    }

    private List<EffectGroup> GroupEffectsByTarget(SkillEffectsCollection effects)
    {
        var groups = new List<EffectGroup>();

        foreach (var effect in effects)
        {
            var existingGroup = groups.FirstOrDefault(g => g.Key == effect.Target);

            if (existingGroup == null)
            {
                existingGroup = new EffectGroup(effect.Target);
                groups.Add(existingGroup);
            }

            existingGroup.AddEffect(effect);
        }

        return groups;
    }

    private EffectGroup FindEffectsForUnit(List<EffectGroup> groups, Unit unit)
    {
        return groups.FirstOrDefault(g => g.Key == unit);
    }

    private List<EffectGroup> GetOtherEffects(List<EffectGroup> groups, Unit actor)
    {
        return groups.Where(g => g.Key != actor).ToList();
    }

    private Unit FindLastTargetWithProperty(List<EffectGroup> groups, Func<SkillEffectsCollection, bool> predicate)
    {
        for (int i = groups.Count - 1; i >= 0; i--)
        {
            if (predicate(groups[i].GetEffects()))
            {
                return groups[i].Key;
            }
        }
        return null;
    }

    private bool HasRepelEffect(SkillEffectsCollection effects)
    {
        return effects.Any(e => e.AffinityResult == Affinity.Repel);
    }

    private bool HasDrainEffect(SkillEffectsCollection effects)
    {
        return effects.Any(e => e.IsDrainEffect());
    }

    private bool HasAffinityType(SkillEffectsCollection effects, Affinity affinity)
    {
        return effects.Any(e => e.AffinityResult == affinity);
    }

    private bool HasElement(SkillEffectsCollection effects, Element element)
    {
        return effects.Any(e => e.Element == element);
    }

    private void DisplayOtherTargetEffects(Unit actor, List<EffectGroup> otherEffects, Unit lastRepelTarget, Unit lastDrainTarget)
    {
        foreach (var targetGroup in otherEffects)
        {
            bool isLastRepelTarget = targetGroup.Key == lastRepelTarget;
            bool isLastDrainTarget = targetGroup.Key == lastDrainTarget;
            DisplayTargetEffects(actor, targetGroup.Key.Name, targetGroup.GetEffects(), isLastRepelTarget, isLastDrainTarget);
        }
    }

    private void DisplayActorEffects(Unit actor, EffectGroup actorEffects)
    {
        if (actorEffects != null)
        {
            DisplayTargetEffects(actor, actorEffects.Key.Name, actorEffects.GetEffects(), false, false);
        }
    }

    private void DisplayMessages(List<string> messages)
    {
        foreach (var message in messages)
        {
            _view.WriteLine(message);
        }
    }

    private void DisplayTargetEffects(Unit actor, string targetName, SkillEffectsCollection effects, bool isLastRepelTarget, bool isLastDrainTarget)
    {
        if (effects.Count == 0)
        {
            return;
        }

        var lastEffect = effects[^1];

        if (ShouldDisplayAsHeal(lastEffect))
        {
            DisplayHealEffects(actor, targetName, effects, lastEffect);
            return;
        }

        if (lastEffect.IsDrainHealEffect())
        {
            DisplayDrainHealthEffect(actor, lastEffect);
            return;
        }

        if (lastEffect.IsDrainEffect())
        {
            DisplayDrainSkillEffects(actor, targetName, effects, isLastDrainTarget);
            return;
        }

        var isAlmighty = HasElement(effects, Element.Almighty);
        var hasRepel = HasAffinityType(effects, Affinity.Repel);
        var hasDrain = HasAffinityType(effects, Affinity.Drain);

        if (isAlmighty)
        {
            DisplayAlmightyEffects(actor, targetName, effects, isLastRepelTarget);
        }

        if (hasRepel)
        {
            DisplayRepelEffects(actor, targetName, effects, isLastRepelTarget);
            return;
        }

        if (hasDrain)
        {
            DisplayDrainAffinityEffects(actor, targetName, effects);
            return;
        }

        if (!isAlmighty)
        {
            DisplayRegularEffects(actor, targetName, effects);
        }

        DisplayFinalHP(targetName, lastEffect);
    }

    private bool ShouldDisplayAsHeal(SkillEffect effect)
    {
        return effect.IsHealEffect() || effect.IsReviveEffect();
    }

    private void DisplayRegularEffects(Unit actor, string targetName, SkillEffectsCollection effects)
    {
        foreach (var effect in effects)
        {
            DisplaySingleHit(actor, targetName, effect);
        }
    }

    private void DisplayFinalHP(string targetName, SkillEffect effect)
    {
        _view.WriteLine($"{targetName} termina con HP:{effect.FinalHP}/{effect.MaxHP}");
    }

    private void DisplayHealEffects(Unit actor, string targetName, SkillEffectsCollection effects, SkillEffect lastEffect)
    {
        var totalHealing = effects.Sum(e => e.HealingDone);

        var actionVerb = lastEffect.IsReviveEffect() ? "revive" : "cura";
        _view.WriteLine($"{actor.Name} {actionVerb} a {targetName}");
        _view.WriteLine($"{targetName} recibe {totalHealing} de HP");
        DisplayFinalHP(targetName, lastEffect);
    }

    private void DisplayDrainHealthEffect(Unit actor, SkillEffect lastEffect)
    {
        _view.WriteLine($"{actor.Name} termina con HP:{lastEffect.FinalHP}/{lastEffect.MaxHP}");
    }

    private void DisplayRepelEffects(Unit actor, string targetName, SkillEffectsCollection effects, bool isLastRepelTarget)
    {
        var lastEffect = effects[^1];

        foreach (var effect in effects)
        {
            DisplayRepelAttack(actor, targetName, effect);
        }

        if (isLastRepelTarget)
        {
            DisplayFinalHP(actor.Name, lastEffect);
        }
    }

    private void DisplayRepelAttack(Unit actor, string targetName, SkillEffect effect)
    {
        var attackVerb = GetAttackVerbByElement(effect.Element);
        _view.WriteLine($"{actor.Name} {attackVerb} {targetName}");

        if (effect.AffinityResult == Affinity.Repel && effect.IsInstantKill)
        {
            _view.WriteLine($"{targetName} devuelve el ataque a {actor.Name} y lo elimina");
        }
        else
        {
            _view.WriteLine($"{targetName} devuelve {effect.DamageDealt} daño a {actor.Name}");
        }
    }

    private void DisplayDrainSkillEffects(Unit actor, string targetName, SkillEffectsCollection effects, bool isLastTarget)
    {
        foreach (var effect in effects)
        {
            DisplayDrainSkillAttack(actor, targetName, effect, isLastTarget);
        }
    }

    private void DisplayDrainSkillAttack(Unit actor, string targetName, SkillEffect effect, bool isLastTarget)
    {
        _view.WriteLine($"{actor.Name} lanza un ataque todo poderoso a {targetName}");

        var drainsHP = IsDrainingHP(effect);
        var drainsMP = IsDrainingMP(effect);

        if (drainsHP)
        {
            DisplayHPDrain(actor, targetName, effect, isLastTarget);
        }

        if (drainsMP)
        {
            DisplayMPDrain(actor, targetName, effect, isLastTarget);
        }
    }

    private bool IsDrainingHP(SkillEffect effect)
    {
        return effect.EffectType == SkillEffectType.DrainHP ||
               effect.EffectType == SkillEffectType.DrainBoth;
    }

    private bool IsDrainingMP(SkillEffect effect)
    {
        return effect.EffectType == SkillEffectType.DrainMP ||
               effect.EffectType == SkillEffectType.DrainBoth;
    }

    private void DisplayHPDrain(Unit actor, string targetName, SkillEffect effect, bool isLastTarget)
    {
        _view.WriteLine($"El ataque drena {effect.HPDrained} HP de {targetName}");
        _view.WriteLine($"{targetName} termina con HP:{effect.FinalHP}/{effect.MaxHP}");

        if (isLastTarget)
        {
            _view.WriteLine($"{actor.Name} termina con HP:{actor.CurrentStats.CurrentHP}/{actor.CurrentStats.MaxHP}");
        }
    }

    private void DisplayMPDrain(Unit actor, string targetName, SkillEffect effect, bool isLastTarget)
    {
        _view.WriteLine($"El ataque drena {effect.MPDrained} MP de {targetName}");
        _view.WriteLine($"{targetName} termina con MP:{effect.FinalMP}/{effect.MaxMP}");

        if (isLastTarget)
        {
            _view.WriteLine($"{actor.Name} termina con MP:{actor.CurrentStats.CurrentMP}/{actor.CurrentStats.MaxMP}");
        }
    }

    private void DisplayAlmightyEffects(Unit actor, string targetName, SkillEffectsCollection effects, bool isLastRepelTarget)
    {
        var lastEffect = effects[^1];

        foreach (var effect in effects)
        {
            var attackVerb = GetAttackVerbByElement(effect.Element);
            _view.WriteLine($"{actor.Name} {attackVerb} {targetName}");
            _view.WriteLine($"{targetName} recibe {effect.DamageDealt} de daño");
        }

        if (isLastRepelTarget)
        {
            DisplayFinalHP(actor.Name, lastEffect);
        }
    }

    private void DisplayDrainAffinityEffects(Unit actor, string targetName, SkillEffectsCollection effects)
    {
        var lastEffect = effects[^1];

        foreach (var effect in effects)
        {
            var attackVerb = GetAttackVerbByElement(effect.Element);
            _view.WriteLine($"{actor.Name} {attackVerb} {targetName}");
            _view.WriteLine($"{targetName} absorbe {effect.HealingDone} daño");
        }

        DisplayFinalHP(targetName, lastEffect);
    }

    private void DisplaySingleHit(Unit actor, string targetName, SkillEffect effect)
    {
        var attackVerb = GetAttackVerbByElement(effect.Element);
        _view.WriteLine($"{actor.Name} {attackVerb} {targetName}");

        if (effect.EffectType == SkillEffectType.InstantKill)
        {
            DisplayInstantKillSkillEffects(actor, targetName, effect);
        }
        else
        {
            DisplayAffinityResult(actor, targetName, effect);
            DisplayDamageMessage(targetName, effect);
        }
    }

    private void DisplayInstantKillSkillEffects(Unit actor, string targetName, SkillEffect effect)
    {
        var isInstaKillAndDied = effect.EffectType == SkillEffectType.InstantKill && effect.TargetDied;

        if (isInstaKillAndDied)
        {
            DisplayAffinityResult(actor, targetName, effect);
            _view.WriteLine($"{targetName} ha sido eliminado");
        }
        else
        {
            if (effect.AffinityResult == Affinity.Null)
            {
                _view.WriteLine($"{targetName} bloquea el ataque de {actor.Name}");
            }
            else
            {
                _view.WriteLine($"{actor.Name} ha fallado el ataque");
            }
        }
    }

    private void DisplayAffinityResult(Unit actor, string targetName, SkillEffect effect)
    {
        var message = effect.AffinityResult switch
        {
            Affinity.Weak => $"{targetName} es débil contra el ataque de {actor.Name}",
            Affinity.Resist => $"{targetName} es resistente el ataque de {actor.Name}",
            Affinity.Null => $"{targetName} bloquea el ataque de {actor.Name}",
            _ => null
        };

        if (message != null)
        {
            _view.WriteLine(message);
        }
    }

    private void DisplayDamageMessage(string targetName, SkillEffect effect)
    {
        if (effect.AffinityResult != Affinity.Null && effect.DamageDealt > 0)
        {
            _view.WriteLine($"{targetName} recibe {effect.DamageDealt} de daño");
        }
    }

    private string GetAttackVerbByElement(Element element)
    {
        return element switch
        {
            Element.Phys => "ataca a",
            Element.Gun => "dispara a",
            Element.Fire => "lanza fuego a",
            Element.Ice => "lanza hielo a",
            Element.Elec => "lanza electricidad a",
            Element.Force => "lanza viento a",
            Element.Light => "ataca con luz a",
            Element.Dark => "ataca con oscuridad a",
            Element.Almighty => "lanza un ataque todo poderoso a",
            _ => "ataca a"
        };
    }

    private class EffectGroup
    {
        public Unit Key { get; }
        private readonly SkillEffectsCollection _effects;

        public EffectGroup(Unit key)
        {
            Key = key;
            _effects = new SkillEffectsCollection();
        }

        public void AddEffect(SkillEffect effect)
        {
            _effects.Add(effect);
        }

        public SkillEffectsCollection GetEffects()
        {
            return _effects;
        }
    }
}
