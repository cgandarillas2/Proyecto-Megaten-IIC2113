using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_View.ConsoleLib;

/// <summary>
/// Responsible for displaying repel effects.
/// Single Responsibility: Repel effect presentation.
/// </summary>
public class RepelEffectDisplay
{
    private readonly View _view;

    public RepelEffectDisplay(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    public void Display(Unit actor, string targetName, SkillEffectsCollection effects, bool isLastRepelTarget, string attackVerb)
    {
        var lastEffect = effects[^1];

        foreach (var effect in effects)
        {
            DisplaySingleRepelAttack(actor, targetName, effect, attackVerb);
        }

        if (isLastRepelTarget)
        {
            _view.WriteLine($"{actor.Name} termina con HP:{lastEffect.FinalHP}/{lastEffect.MaxHP}");
        }
    }

    private void DisplaySingleRepelAttack(Unit actor, string targetName, SkillEffect effect, string attackVerb)
    {
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
}
