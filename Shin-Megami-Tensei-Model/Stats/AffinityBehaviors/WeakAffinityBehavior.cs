using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Stats.AffinityBehaviors;

public class WeakAffinityBehavior : IAffinityBehavior
{
    public double GetDamageMultiplier() => 1.5;

    public int GetPriority() => 3;

    public TurnConsumption GetTurnConsumption() => TurnConsumption.Weak();

    public void ApplyDamage(Unit attacker, Unit target, int baseDamage)
    {
        int finalDamage = (int)(baseDamage * GetDamageMultiplier());
        target.TakeDamage(finalDamage);
    }
}
