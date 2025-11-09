using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Game.TargetFilters;

public interface ITargetFilter
{
    UnitsCollection GetValidTargets(GameState gameState, Unit actor);
}