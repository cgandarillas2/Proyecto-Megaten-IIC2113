using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Game.TargetFilters;

public class PartyFilter: ITargetFilter
{
    private bool _isDrainHeal;
    public PartyFilter(bool isDrainHeal)
    {
        _isDrainHeal = isDrainHeal;
    }
    public List<Unit> GetValidTargets(GameState gameState, Unit actor)
        => GetValidTargets(gameState, actor, _isDrainHeal);

    public List<Unit> GetValidTargets(GameState gameState, Unit actor, bool isDrainHeal)
    {
        return isDrainHeal
            ? gameState.GetAllTeamUnitsInOrder()
            : gameState.CurrentPlayer.ActiveBoard.GetAliveUnits();
    }
}