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
        var effectsByTarget = result.Effects
            .GroupBy(e => e.TargetName)
            .ToList();
        
        var actorEffects = effectsByTarget.FirstOrDefault(unit => unit.Key == actor.Name);
        var otherEffects = effectsByTarget.Where(unit => unit.Key != actor.Name).ToList();

        foreach (var targetGroup in otherEffects)
        {
            DisplayTargetEffects(actor, targetGroup.Key, targetGroup.ToList());
        }

        if (actorEffects != null)
        {
            DisplayTargetEffects(actor, actorEffects.Key, actorEffects.ToList());
        }
        
        foreach (var message in result.Messages)
        {
            _view.WriteLine(message);
        }
    }

    private void DisplayTargetEffects(Unit actor, string targetName, List<SkillEffect> effects)
    {
        if (effects.Count == 0)
        {
            return;
        }

        var lastEffect = effects[^1];
        if (lastEffect.IsHealEffect() || lastEffect.IsReviveEffect())
        {
            DisplayHealEffects(actor, targetName, effects, lastEffect);
            return;
        }

        if (lastEffect.IsDrainHealEffect())
        {
            DisplayDrainHealthEffect(actor, lastEffect);
            return;
        }

        var hasRepel = effects.Any(e => e.AffinityResult == Affinity.Repel);
        var hasDrain = effects.Any(e => e.AffinityResult == Affinity.Drain);

        if (hasRepel)
        {
            DisplayRepelEffects(actor, targetName, effects);
            return;
        }

        if (hasDrain)
        {
            DisplayDrainEffects(actor, targetName, effects);
            return;
        }

        // Efectos ofensivos normales
        foreach (var effect in effects)
        {
            DisplaySingleHit(actor, targetName, effect);
        }

        _view.WriteLine($"{targetName} termina con HP:{lastEffect.FinalHP}/{lastEffect.MaxHP}");
    }

    private void DisplayHealEffects(Unit actor, string targetName, List<SkillEffect> effects, SkillEffect lastEffect)
    {
        var totalHealing = effects.Sum(e => e.HealingDone);

        if (lastEffect.IsReviveEffect())
        {
            _view.WriteLine($"{actor.Name} revive a {targetName}");
        }
        else
        {
            _view.WriteLine($"{actor.Name} cura a {targetName}");
        }
        
        _view.WriteLine($"{targetName} recibe {totalHealing} de HP");
        _view.WriteLine($"{targetName} termina con HP:{lastEffect.FinalHP}/{lastEffect.MaxHP}");
    }

    private void DisplayDrainHealthEffect(Unit actor, SkillEffect lastEffect)
    {
        _view.WriteLine($"{actor.Name} termina con HP:{lastEffect.FinalHP}/{lastEffect.MaxHP}");
    }

    private void DisplayRepelEffects(Unit actor, string targetName, List<SkillEffect> effects)
    {
        var lastEffect = effects[^1];

        foreach (var effect in effects)
        {
            var attackVerb = GetAttackVerbByElement(effect.Element);
            _view.WriteLine($"{actor.Name} {attackVerb} {targetName}");
            _view.WriteLine($"{targetName} devuelve {effect.DamageDealt} daño a {actor.Name}");
        }

        // Mostrar HP final del atacante (solo una vez al final)
        _view.WriteLine($"{actor.Name} termina con HP:{lastEffect.FinalHP}/{lastEffect.MaxHP}");
    }

    private void DisplayDrainEffects(Unit actor, string targetName, List<SkillEffect> effects)
    {
        var lastEffect = effects[^1];

        foreach (var effect in effects)
        {
            var attackVerb = GetAttackVerbByElement(effect.Element);
            _view.WriteLine($"{actor.Name} {attackVerb} {targetName}");
            _view.WriteLine($"{targetName} absorbe {effect.HealingDone} daño");
        }

        // Mostrar HP final del target (solo una vez al final)
        _view.WriteLine($"{targetName} termina con HP:{lastEffect.FinalHP}/{lastEffect.MaxHP}");
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
            DisplayFinishMessage(targetName, effect);
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
        if (effect.AffinityResult == Affinity.Weak)
        {
            _view.WriteLine($"{targetName} es débil contra el ataque de {actor.Name}");
        }
        else if (effect.AffinityResult == Affinity.Resist)
        {
            _view.WriteLine($"{targetName} es resistente el ataque de {actor.Name}");
        }
        else if (effect.AffinityResult == Affinity.Null)
        {
            _view.WriteLine($"{targetName} bloquea el ataque de {actor.Name}");
        }
    }

    private void DisplayFinishMessage(string targetName, SkillEffect effect)
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
}