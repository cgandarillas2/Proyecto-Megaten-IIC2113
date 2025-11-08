using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Stats.AffinityBehaviors;

public interface IAffinityBehavior
{
    double GetDamageMultiplier();
    int GetPriority();
    TurnConsumption GetTurnConsumption();
    void ApplyDamage(Unit attacker, Unit target, int baseDamage);
}
