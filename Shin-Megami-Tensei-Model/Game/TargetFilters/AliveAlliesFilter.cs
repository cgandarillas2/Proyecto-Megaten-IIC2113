using Shin_Megami_Tensei_Model.Units;
using Shin_Megami_Tensei_Model.Collections;

namespace Shin_Megami_Tensei_Model.Game.TargetFilters;

public class AliveAlliesFilter: ITargetFilter
{
    public UnitsCollection GetValidTargets(GameState gameState, Unit actor)
    {
        return new UnitsCollection(gameState.CurrentPlayer.ActiveBoard.GetAliveUnits());
    }
}