using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Action;

public class SummonAction : IAction
{
    public bool CanExecute(Unit actor, GameState gameState)
    {
        return actor.IsAlive();
    }

    // DOS VERSIONES: 1 samurai - 2 monstruo
    public ActionResult Execute(Unit actor, Unit target, GameState gameState)
    {
        var turnConsumption = TurnConsumption.PassOrSummon();
        
        return ActionResult.Successful(turnConsumption, 0, Affinity.Neutral);
    }

    private bool HasMonstersToSummon(GameState gameState)
    {
        var reserveMonsters = gameState.CurrentPlayer.GetAliveReserveMonsters();
        return reserveMonsters.Count > 0;
    }

    public List<Monster> GetTargets(GameState gameState)
    {
        return gameState.CurrentPlayer.GetAliveReserveMonsters();
    }
}
