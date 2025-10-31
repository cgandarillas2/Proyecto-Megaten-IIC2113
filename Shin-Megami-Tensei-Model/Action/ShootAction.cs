using Shin_Megami_Tensei_Model.Combat;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Action;

public class ShootAction: IAction
{
    private readonly DamageCalculator _damageCalculator;

    public ShootAction(DamageCalculator damageCalculator)
    {
        _damageCalculator = damageCalculator;
    }

    public bool CanExecute(Unit actor, GameState gameState)
    {
        return actor.IsAlive() && CanShoot(actor);
    }

    public ActionResult Execute(Unit actor, Unit target, GameState gameState)
    {
        var baseDamage = _damageCalculator.CalculateShootDamage(actor);
        var affinity = target.Affinities.GetAffinity(Element.Gun);
        var finalDamage = ApplyAffinityMultiplier(baseDamage, affinity);
        
        /*target.TakeDamage(damage);*/
        ApplyDamageByAffinity(actor, target, finalDamage, affinity);
            
        /*var turnConsumption = TurnConsumption.NeutralOrResist();*/
        var turnConsumption = CalculateTurnConsumption(affinity);
        return ActionResult.Successful(turnConsumption, finalDamage, affinity);
    }
    
    private void ApplyDamageByAffinity(Unit actor, Unit target, int damage, Affinity affinity)
    {
        if (affinity == Affinity.Null)
        {
            return;
        }

        if (affinity == Affinity.Repel)
        {
            actor.TakeDamage(damage);
            return;
        }

        if (affinity == Affinity.Drain)
        {
            target.Heal(damage);
            return;
        }

        target.TakeDamage(damage);
    }

    private int ApplyAffinityMultiplier(int baseDamage, Affinity affinity)
    {
        var multiplier = affinity switch
        {
            Affinity.Weak => 1.5,
            Affinity.Resist => 0.5,
            Affinity.Null => 0.0,
            Affinity.Repel => 1.0,
            Affinity.Drain => 1.0,
            _ => 1.0
        };

        return (int)Math.Floor(baseDamage * multiplier);
    }
    
    private TurnConsumption CalculateTurnConsumption(Affinity affinity)
    {
        return affinity switch
        {
            Affinity.Weak => TurnConsumption.Weak(),
            Affinity.Resist => TurnConsumption.NeutralOrResist(),
            Affinity.Null => TurnConsumption.Null(),
            Affinity.Repel => TurnConsumption.RepelOrDrain(),
            Affinity.Drain => TurnConsumption.RepelOrDrain(),
            _ => TurnConsumption.NeutralOrResist()
        };
    }
    
    private static bool CanShoot(Unit actor)
    {
        return actor is Samurai;
    }
}