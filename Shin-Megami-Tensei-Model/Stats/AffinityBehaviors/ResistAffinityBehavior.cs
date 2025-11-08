using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Stats.AffinityBehaviors;

public class ResistAffinityBehavior : IAffinityBehavior
{
    public double GetDamageMultiplier() => 0.5;

    public int GetPriority() => 1;

    public TurnConsumption GetTurnConsumption() => TurnConsumption.OffensiveSkill();

    public void ApplyDamage(Unit attacker, Unit target, int baseDamage)
    {
        int finalDamage = (int)(baseDamage * GetDamageMultiplier());
        target.TakeDamage(finalDamage);
    }
}
