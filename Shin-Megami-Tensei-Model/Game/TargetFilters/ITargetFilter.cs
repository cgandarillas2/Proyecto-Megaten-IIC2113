using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Game.TargetFilters;

public interface ITargetFilter
{
    List<Unit> GetValidTargets(GameState gameState, Unit actor);
}