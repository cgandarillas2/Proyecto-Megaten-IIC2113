using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Stats.AffinityBehaviors;

public class DrainAffinityBehavior : IAffinityBehavior
{
    public double GetDamageMultiplier() => 1.0;

    public int GetPriority() => 6;

    public TurnConsumption GetTurnConsumption() => TurnConsumption.OffensiveSkill();

    public void ApplyDamage(Unit attacker, Unit target, int baseDamage)
    {
        // Target heals instead of taking damage
        target.Heal(baseDamage);
    }
}
