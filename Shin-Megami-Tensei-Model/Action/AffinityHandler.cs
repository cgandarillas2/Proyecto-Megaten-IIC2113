using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Stats.AffinityBehaviors;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Action;

/// <summary>
/// Maneja la lógica relacionada con afinidades en el combate usando polimorfismo
/// </summary>
public class AffinityHandler
{
    private readonly AffinityBehaviorFactory _behaviorFactory;

    public AffinityHandler()
    {
        _behaviorFactory = new AffinityBehaviorFactory();
    }

    public void ApplyDamageByAffinity(Unit attacker, Unit target, int damage, Affinity affinity)
    {
        var behavior = _behaviorFactory.GetBehavior(affinity);
        behavior.ApplyDamage(attacker, target, damage);
    }

    public int ApplyAffinityMultiplier(double baseDamage, Affinity affinity)
    {
        var behavior = _behaviorFactory.GetBehavior(affinity);
        var multiplier = behavior.GetDamageMultiplier();
        return (int)Math.Floor(baseDamage * multiplier);
    }

    public TurnConsumption CalculateTurnConsumption(Affinity affinity)
    {
        var behavior = _behaviorFactory.GetBehavior(affinity);
        return behavior.GetTurnConsumption();
    }

    public int GetAffinityPriority(Affinity affinity)
    {
        var behavior = _behaviorFactory.GetBehavior(affinity);
        return behavior.GetPriority();
    }

    public SkillEffect CreateSkillEffect(Unit attacker, Unit target, int damage, Affinity affinity, Element element)
    {
        var behavior = _behaviorFactory.GetBehavior(affinity);
        return behavior.CreateSkillEffect(attacker, target, damage, element);
    }
}