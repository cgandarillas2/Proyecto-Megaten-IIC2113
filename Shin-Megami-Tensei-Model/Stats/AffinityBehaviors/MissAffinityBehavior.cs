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
}
