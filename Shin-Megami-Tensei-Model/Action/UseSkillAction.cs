using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Action;

public class UseSkillAction: IAction
{
    public bool CanExecute(Unit actor, GameState gameState)
    {
        return actor.IsAlive();
    }

    public ActionResult Execute(Unit actor, Unit target, GameState gameState)
    {
        var turnConsumption = TurnConsumption.NeutralOrResist();
        return ActionResult.Successful(turnConsumption, 0);
        
    }
}