using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_View.ConsoleLib;

/// <summary>
/// Responsible for displaying healing and revival effects.
/// Single Responsibility: Heal effect presentation.
/// </summary>
public class HealEffectDisplay
{
    private readonly View _view;

    public HealEffectDisplay(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    public void Display(Unit actor, string targetName, SkillEffectsCollection effects, SkillEffect lastEffect)
    {
        var totalHealing = effects.Sum(e => e.HealingDone);
        var actionVerb = lastEffect.IsReviveEffect() ? "revive" : "cura";

        _view.WriteLine($"{actor.Name} {actionVerb} a {targetName}");
        _view.WriteLine($"{targetName} recibe {totalHealing} de HP");
        _view.WriteLine($"{targetName} termina con HP:{lastEffect.FinalHP}/{lastEffect.MaxHP}");
    }

    public void DisplayDrainHealthEffect(Unit actor, SkillEffect lastEffect)
    {
        _view.WriteLine($"{actor.Name} termina con HP:{lastEffect.FinalHP}/{lastEffect.MaxHP}");
    }
}
