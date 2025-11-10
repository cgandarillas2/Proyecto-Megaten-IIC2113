using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Stats.AffinityBehaviors;

public class DrainAffinityBehavior : IAffinityBehavior
{
    public double GetDamageMultiplier() => 1.0;

    public int GetPriority() => 6;

    public TurnConsumption GetTurnConsumption() => TurnConsumption.RepelOrDrain();

    public void ApplyDamage(Unit attacker, Unit target, int baseDamage)
    {
        target.Heal(baseDamage);
    }

    public SkillEffect CreateSkillEffect(Unit attacker, Unit target, int damage, Element element)
    {
        target.Heal(damage);

        return new SkillEffectBuilder()
            .ForTarget(target)
            .WithDamage(0)
            .WithHealing(damage)
            .WithAffinity(Affinity.Drain)
            .WithFinalHP(target.CurrentStats.CurrentHP, target.CurrentStats.MaxHP)
            .WithElement(element)
            .AsOffensive()
            .TargetDied(false)
            .Build();
    }
}
