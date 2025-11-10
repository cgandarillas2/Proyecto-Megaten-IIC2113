using Shin_Megami_Tensei_Model.Units;
using Shin_Megami_Tensei_Model.Collections;

namespace Shin_Megami_Tensei_Model.Game.TargetFilters;

public class AliveOpponentsFilter: ITargetFilter
{
    public UnitsCollection GetValidTargets(GameState gameState, Unit actor)
    {
        return gameState.GetOpponentAliveUnits();
    }
}