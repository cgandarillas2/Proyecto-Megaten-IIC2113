using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Stats.AffinityBehaviors;

public class NeutralAffinityBehavior : IAffinityBehavior
{
    public double GetDamageMultiplier() => 1.0;

    public int GetPriority() => 1;

    public TurnConsumption GetTurnConsumption() => TurnConsumption.NeutralOrResist();

    public void ApplyDamage(Unit attacker, Unit target, int baseDamage)
    {
        target.TakeDamage(baseDamage);
    }

    public SkillEffect CreateSkillEffect(Unit attacker, Unit target, int damage, Element element)
    {
        target.TakeDamage(damage);
        var died = !target.IsAlive();

        return new SkillEffectBuilder()
            .ForTarget(target)
            .WithDamage(damage)
            .WithAffinity(Affinity.Neutral)
            .WithFinalHP(target.CurrentStats.CurrentHP, target.CurrentStats.MaxHP)
            .WithElement(element)
            .AsOffensive()
            .TargetDied(died)
            .Build();
    }
}
