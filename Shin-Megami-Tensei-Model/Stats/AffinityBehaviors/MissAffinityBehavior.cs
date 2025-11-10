using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Stats.AffinityBehaviors;

public class MissAffinityBehavior : IAffinityBehavior
{
    public double GetDamageMultiplier() => 0.0;

    public int GetPriority() => 4;

    public TurnConsumption GetTurnConsumption() => TurnConsumption.Miss();

    public void ApplyDamage(Unit attacker, Unit target, int baseDamage)
    {
        // Attack missed - no damage applied
    }

    public SkillEffect CreateSkillEffect(Unit attacker, Unit target, int damage, Element element)
    {
        return new SkillEffectBuilder()
            .ForTarget(target)
            .WithDamage(0)
            .WithAffinity(Affinity.Miss)
            .WithFinalHP(target.CurrentStats.CurrentHP, target.CurrentStats.MaxHP)
            .WithElement(element)
            .AsOffensive()
            .TargetDied(false)
            .Build();
    }
}
