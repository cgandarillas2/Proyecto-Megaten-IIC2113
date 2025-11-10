using Shin_Megami_Tensei_Model.Units;
using Shin_Megami_Tensei_Model.Collections;

namespace Shin_Megami_Tensei_Model.Game.TargetFilters;

public class AllUnitsFilter: ITargetFilter
{
    public UnitsCollection GetValidTargets(GameState gameState, Unit actor)
    {
        var allUnits = new List<Unit>();
        allUnits.AddRange(gameState.CurrentPlayer.ActiveBoard.GetAliveUnits());
        allUnits.AddRange(gameState.GetOpponent().ActiveBoard.GetAliveUnits());
        return new UnitsCollection(allUnits);
    }
}