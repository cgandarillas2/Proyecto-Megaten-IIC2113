using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Stats.AffinityBehaviors;

public class RepelAffinityBehavior : IAffinityBehavior
{
    public double GetDamageMultiplier() => 1.0;

    public int GetPriority() => 6;

    public TurnConsumption GetTurnConsumption() => TurnConsumption.RepelOrDrain();

    public void ApplyDamage(Unit attacker, Unit target, int baseDamage)
    {
        // Damage is reflected back to the attacker
        attacker.TakeDamage(baseDamage);
    }
}
