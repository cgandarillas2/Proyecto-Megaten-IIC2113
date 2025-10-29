using Shin_Megami_Tensei_Model.Combat;
using Shin_Megami_Tensei_Model.Game;
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
        var damage = _damageCalculator.CalculateShootDamage(actor);
        target.TakeDamage(damage);
            
        var turnConsumption = TurnConsumption.NeutralOrResist();
        return ActionResult.Successful(turnConsumption, damage);
    }
    
    private static bool CanShoot(Unit actor)
    {
        return actor is Samurai;
    }
}