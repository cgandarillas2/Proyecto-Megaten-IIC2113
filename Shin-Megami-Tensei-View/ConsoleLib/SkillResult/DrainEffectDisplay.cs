using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_View.ConsoleLib.SkillResult;

/// <summary>
/// Responsible for displaying drain skill effects (HP/MP drain).
/// Single Responsibility: Drain effect presentation.
/// </summary>
public class DrainEffectDisplay
{
    private readonly View _view;

    public DrainEffectDisplay(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    public void DisplayDrainSkill(Unit actor, string targetName, SkillEffectsCollection effects, bool isLastTarget)
    {
        foreach (var effect in effects)
        {
            DisplaySingleDrainAttack(actor, targetName, effect, isLastTarget);
        }
    }

    public void DisplayDrainAffinity(Unit actor, string targetName, SkillEffectsCollection effects, string attackVerb)
    {
        var lastEffect = effects[^1];

        foreach (var effect in effects)
        {
            _view.WriteLine($"{actor.Name} {attackVerb} {targetName}");
            _view.WriteLine($"{targetName} absorbe {effect.HealingDone} daño");
        }

        _view.WriteLine($"{targetName} termina con HP:{lastEffect.FinalHP}/{lastEffect.MaxHP}");
    }

    private void DisplaySingleDrainAttack(Unit actor, string targetName, SkillEffect effect, bool isLastTarget)
    {
        _view.WriteLine($"{actor.Name} lanza un ataque todo poderoso a {targetName}");

        if (IsDrainingHP(effect))
        {
            DisplayHPDrain(actor, targetName, effect, isLastTarget);
        }

        if (IsDrainingMP(effect))
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
}
