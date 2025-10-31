using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Action;

public class PassTurnAction: IAction
{
    public bool CanExecute(Unit actor, GameState gameState)
    {
        return actor.IsAlive();
    }

    public ActionResult Execute(Unit actor, Unit target, GameState gameState)
    {
        var blinkingTurns = gameState.CurrentTurnState.BlinkingTurns;
        var turnConsumption = TurnConsumption.PassOrSummon();
        
        return ActionResult.Successful(turnConsumption, 0, Affinity.Neutral);
    }
}