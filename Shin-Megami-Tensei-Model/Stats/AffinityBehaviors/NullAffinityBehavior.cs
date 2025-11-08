using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Stats.AffinityBehaviors;

public class NullAffinityBehavior : IAffinityBehavior
{
    public double GetDamageMultiplier() => 0.0;

    public int GetPriority() => 5;

    public TurnConsumption GetTurnConsumption() => TurnConsumption.OffensiveSkill();

    public void ApplyDamage(Unit attacker, Unit target, int baseDamage)
    {
        // No damage applied - attack is nullified
    }
}
