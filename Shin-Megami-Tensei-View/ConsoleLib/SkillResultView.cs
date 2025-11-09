using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Services;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;
using Shin_Megami_Tensei_View.ConsoleLib.SkillResults;


namespace Shin_Megami_Tensei_View.ConsoleLib;

/// <summary>
/// Coordinator for displaying skill results.
/// Single Responsibility: Coordinate effect display delegation.
/// </summary>
public class SkillResultView
{
    private readonly View _view;
    private readonly EffectGrouper _effectGrouper;
    private readonly HealEffectDisplay _healDisplay;
    private readonly DrainEffectDisplay _drainDisplay;
    private readonly RepelEffectDisplay _repelDisplay;
    private readonly RegularEffectDisplay _regularDisplay;

    public SkillResultView(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _effectGrouper = new EffectGrouper();
        _healDisplay = new HealEffectDisplay(view);
        _drainDisplay = new DrainEffectDisplay(view);
        _repelDisplay = new RepelEffectDisplay(view);
        _regularDisplay = new RegularEffectDisplay(view);
    }

    public void Present(Unit actor, SkillResult result)
    {
        var effectsByTarget = _effectGrouper.GroupEffectsByTarget(result.Effects);
        var actorEffects = _effectGrouper.FindEffectsForUnit(effectsByTarget, actor);
        var otherEffects = _effectGrouper.GetOtherEffects(effectsByTarget, actor);

        var lastRepelTarget = _effectGrouper.FindLastTargetWithProperty(otherEffects, _effectGrouper.HasRepelEffect);
        var lastDrainTarget = _effectGrouper.FindLastTargetWithProperty(otherEffects, _effectGrouper.HasDrainEffect);

        DisplayOtherTargetEffects(actor, otherEffects, lastRepelTarget, lastDrainTarget);
        DisplayActorEffects(actor, actorEffects);
        DisplayMessages(result.Messages);
    }

    private void DisplayOtherTargetEffects(Unit actor, List<EffectGroup> otherEffects, Unit lastRepelTarget, Unit lastDrainTarget)
    {
        foreach (var targetGroup in otherEffects)
        {
            bool isLastRepelTarget = targetGroup.Target == lastRepelTarget;
            bool isLastDrainTarget = targetGroup.Target == lastDrainTarget;
            DisplayTargetEffects(actor, targetGroup.Target.Name, targetGroup.Effects, isLastRepelTarget, isLastDrainTarget);
        }
    }

    private void DisplayActorEffects(Unit actor, EffectGroup actorEffects)
    {
        if (actorEffects != null)
        {
            DisplayTargetEffects(actor, actorEffects.Target.Name, actorEffects.Effects, false, false);
        }
    }

    private void DisplayMessages(StringCollection messages)
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
            _healDisplay.Display(actor, targetName, effects, lastEffect);
            return;
        }

        if (lastEffect.IsDrainHealEffect())
        {
            _healDisplay.DisplayDrainHealthEffect(actor, lastEffect);
            return;
        }

        if (lastEffect.IsDrainEffect())
        {
            _drainDisplay.DisplayDrainSkill(actor, targetName, effects, isLastDrainTarget);
            return;
        }

        var isAlmighty = HasElement(effects, Element.Almighty);
        var hasRepel = HasAffinity(effects, Affinity.Repel);
        var hasDrain = HasAffinity(effects, Affinity.Drain);

        var attackVerb = GetAttackVerbByElement(effects[0].Element);

        if (isAlmighty)
        {
            _regularDisplay.DisplayAlmighty(actor, targetName, effects, isLastRepelTarget, attackVerb);
        }

        if (hasRepel)
        {
            _repelDisplay.Display(actor, targetName, effects, isLastRepelTarget, attackVerb);
            return;
        }

        if (hasDrain)
        {
            _drainDisplay.DisplayDrainAffinity(actor, targetName, effects, attackVerb);
            return;
        }

        if (!isAlmighty)
        {
            _regularDisplay.DisplayRegular(actor, targetName, effects, attackVerb);
        }

        DisplayFinalHP(targetName, lastEffect);
    }

    private bool ShouldDisplayAsHeal(SkillEffect effect)
    {
        return effect.IsHealEffect() || effect.IsReviveEffect();
    }

    private bool HasAffinity(SkillEffectsCollection effects, Affinity affinity)
    {
        return effects.Any(e => e.AffinityResult == affinity);
    }

    private bool HasElement(SkillEffectsCollection effects, Element element)
    {
        return effects.Any(e => e.Element == element);
    }

    private void DisplayFinalHP(string targetName, SkillEffect effect)
    {
        _view.WriteLine($"{targetName} termina con HP:{effect.FinalHP}/{effect.MaxHP}");
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
