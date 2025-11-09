using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_View.ConsoleLib.SkillResult;

public class SupportEffectDisplay
{
    private readonly View _view;

    public SupportEffectDisplay(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    public void Display(Unit actor, SkillEffect effect)
    {
        var targetName = effect.Target.Name;

        switch (effect.EffectType)
        {
            case SkillEffectType.ChargePhysical:
                DisplayPhysicalCharge(targetName);
                break;

            case SkillEffectType.ChargeMagical:
                DisplayMagicalCharge(targetName);
                break;

            case SkillEffectType.BuffAttack:
                DisplayAttackBuff(targetName);
                break;

            case SkillEffectType.BuffDefense:
                DisplayDefenseBuff(targetName);
                break;

            case SkillEffectType.BloodRitual:
                DisplayBloodRitual(effect);
                break;
        }
    }

    private void DisplayPhysicalCharge(string targetName)
    {
        _view.WriteLine($"{targetName} ha cargado su siguiente ataque físico o disparo a más del doble");
    }

    private void DisplayMagicalCharge(string targetName)
    {
        _view.WriteLine($"{targetName} ha cargado su siguiente ataque mágico a más del doble");
    }

    private void DisplayAttackBuff(string targetName)
    {
        _view.WriteLine($"El ataque de {targetName} ha aumentado");
    }

    private void DisplayDefenseBuff(string targetName)
    {
        _view.WriteLine($"La defensa de {targetName} ha aumentado");
    }

    private void DisplayBloodRitual(SkillEffect effect)
    {
        var targetName = effect.Target.Name;
        _view.WriteLine($"El ataque de {targetName} ha aumentado");
        _view.WriteLine($"La defensa de {targetName} ha aumentado");
        _view.WriteLine($"{targetName} termina con HP:{effect.FinalHP}/{effect.MaxHP}");
    }
}
