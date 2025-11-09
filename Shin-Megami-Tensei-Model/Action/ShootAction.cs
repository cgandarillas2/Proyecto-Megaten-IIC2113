using Shin_Megami_Tensei_Model.Combat;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Action;

public class ShootAction: IAction
{
    private readonly DamageCalculator _damageCalculator;
    private readonly AffinityHandler _affinityHandler;

    public ShootAction(DamageCalculator damageCalculator)
        : this(damageCalculator, new AffinityHandler())
    {
    }

    public ShootAction(DamageCalculator damageCalculator, AffinityHandler affinityHandler)
    {
        _damageCalculator = damageCalculator;
        _affinityHandler = affinityHandler;
    }

    public bool CanExecute(Unit actor, GameState gameState)
    {
        return actor.IsAlive() && CanShoot(actor);
    }

    public ActionResult Execute(Unit actor, Unit target, GameState gameState)
    {
        var baseDamage = _damageCalculator.CalculateShootDamage(actor);
        var affinity = target.Affinities.GetAffinity(Element.Gun);
        var affinityDamage = _affinityHandler.ApplyAffinityMultiplier(baseDamage, affinity);
        var finalDamage = Convert.ToInt32(Math.Floor(affinityDamage));
        
        _affinityHandler.ApplyDamageByAffinity(actor, target, finalDamage, affinity);
        
        var turnConsumption = _affinityHandler.CalculateTurnConsumption(affinity);
        return ActionResult.Successful(turnConsumption, finalDamage, affinity);
    }
    
    private static bool CanShoot(Unit actor)
    {
        return actor is Samurai;
    }
}