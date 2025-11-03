using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Game.TargetFilters;

public class AliveAlliesFilter: ITargetFilter
{
    public List<Unit> GetValidTargets(GameState gameState, Unit actor)
    {
        return gameState.CurrentPlayer.ActiveBoard.GetAliveUnits();
    }
}