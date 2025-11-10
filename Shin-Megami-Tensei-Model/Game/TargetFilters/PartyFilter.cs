using Shin_Megami_Tensei_Model.Units;
using Shin_Megami_Tensei_Model.Collections;

namespace Shin_Megami_Tensei_Model.Game.TargetFilters;

public class PartyFilter: ITargetFilter
{
    private bool _isDrainHeal;
    public PartyFilter(bool isDrainHeal)
    {
        _isDrainHeal = isDrainHeal;
    }
    public UnitsCollection GetValidTargets(GameState gameState, Unit actor)
        => GetValidTargets(gameState, actor, _isDrainHeal);

    public UnitsCollection GetValidTargets(GameState gameState, Unit actor, bool isDrainHeal)
    {
        return isDrainHeal
            ? gameState.GetAllTeamUnitsInOrder()
            : gameState.CurrentPlayer.ActiveBoard.GetAllUnits();
    }
}