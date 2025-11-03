using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Game.TargetFilters;

public class AllUnitsFilter: ITargetFilter
{
    public List<Unit> GetValidTargets(GameState gameState, Unit actor)
    {
        var allUnits = new List<Unit>();
        allUnits.AddRange(gameState.CurrentPlayer.ActiveBoard.GetAliveUnits());
        allUnits.AddRange(gameState.GetOpponent().ActiveBoard.GetAliveUnits());
        return allUnits;
    }
}