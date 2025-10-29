using Shin_Megami_Tensei_Model.Combat;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Action;

public class SurrenderAction: IAction
{
    public bool CanExecute(Unit actor, GameState gameState)
    {
        return actor is Samurai;
    }

    public ActionResult Execute(Unit actor, Unit target, GameState gameState)
    {
        var winner = gameState.GetOpponentName();
        gameState.SurrenderByKillingAllUnits();
        return ActionResult.GameOver(winner);
    }
}