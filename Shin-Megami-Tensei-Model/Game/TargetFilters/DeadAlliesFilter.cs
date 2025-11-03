using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Game.TargetFilters;

public class DeadAlliesFilter: ITargetFilter
{
    public List<Unit> GetValidTargets(GameState gameState, Unit actor)
    {
        var deadUnits = gameState.CurrentPlayer.ActiveBoard.GetAllUnits()
            .Where(u => !u.IsEmpty() && !u.IsAlive())
            .ToList();

        var deadReserveMonsters = gameState.CurrentPlayer.GetDeadReserveMonsters();
        deadUnits.AddRange(deadReserveMonsters);

        return deadUnits;
    }
}