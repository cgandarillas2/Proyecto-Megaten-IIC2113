using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Stats.AffinityBehaviors;

public class RepelAffinityBehavior : IAffinityBehavior
{
    public double GetDamageMultiplier() => 1.0;

    public int GetPriority() => 6;

    public TurnConsumption GetTurnConsumption() => TurnConsumption.RepelOrDrain();

    public void ApplyDamage(Unit attacker, Unit target, int baseDamage)
    {
        attacker.TakeDamage(baseDamage);
    }

    public SkillEffect CreateSkillEffect(Unit attacker, Unit target, int damage, Element element)
    {
        attacker.TakeDamage(damage);

        return new SkillEffectBuilder()
            .ForTarget(target)
            .WithDamage(damage)
            .WithAffinity(Affinity.Repel)
            .WithFinalHP(attacker.CurrentStats.CurrentHP, attacker.CurrentStats.MaxHP)
            .WithElement(element)
            .AsOffensive()
            .TargetDied(false)
            .Build();
    }
}
