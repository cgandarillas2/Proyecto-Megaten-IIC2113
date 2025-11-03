using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Game.TargetFilters;

public class SelfTargetFilter: ITargetFilter
{
    public List<Unit> GetValidTargets(GameState gameState, Unit actor)
    {
        return new List<Unit> { actor };
    }
}