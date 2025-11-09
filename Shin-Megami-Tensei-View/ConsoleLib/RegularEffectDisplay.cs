using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_View.ConsoleLib;

/// <summary>
/// Responsible for displaying regular attack effects and almighty effects.
/// Single Responsibility: Regular and almighty effect presentation.
/// </summary>
public class RegularEffectDisplay
{
    private readonly View _view;

    public RegularEffectDisplay(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    public void DisplayRegular(Unit actor, string targetName, SkillEffectsCollection effects, string attackVerb)
    {
        foreach (var effect in effects)
        {
            DisplaySingleHit(actor, targetName, effect, attackVerb);
        }
    }

    public void DisplayAlmighty(Unit actor, string targetName, SkillEffectsCollection effects, bool isLastRepelTarget, string attackVerb)
    {
        var lastEffect = effects[^1];

        foreach (var effect in effects)
        {
            _view.WriteLine($"{actor.Name} {attackVerb} {targetName}");
            _view.WriteLine($"{targetName} recibe {effect.DamageDealt} de daño");
        }

        if (isLastRepelTarget)
        {
            _view.WriteLine($"{actor.Name} termina con HP:{lastEffect.FinalHP}/{lastEffect.MaxHP}");
        }
    }

    private void DisplaySingleHit(Unit actor, string targetName, SkillEffect effect, string attackVerb)
    {
        _view.WriteLine($"{actor.Name} {attackVerb} {targetName}");

        if (effect.EffectType == SkillEffectType.InstantKill)
        {
            DisplayInstantKill(actor, targetName, effect);
        }
        else
        {
            DisplayAffinityResult(actor, targetName, effect);
            DisplayDamageMessage(targetName, effect);
        }
    }

    private void DisplayInstantKill(Unit actor, string targetName, SkillEffect effect)
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
}
