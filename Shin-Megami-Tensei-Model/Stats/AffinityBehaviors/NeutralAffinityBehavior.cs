using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Stats.AffinityBehaviors;

public class NeutralAffinityBehavior : IAffinityBehavior
{
    public double GetDamageMultiplier() => 1.0;

    public int GetPriority() => 1;

    public TurnConsumption GetTurnConsumption() => TurnConsumption.OffensiveSkill();

    public void ApplyDamage(Unit attacker, Unit target, int baseDamage)
    {
        target.TakeDamage(baseDamage);
    }
}
