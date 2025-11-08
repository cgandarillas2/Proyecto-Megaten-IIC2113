using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Game.TargetFilters;

public class DeadAlliesFilter: ITargetFilter
{
    public List<Unit> GetValidTargets(GameState gameState, Unit actor)
    {
        var allUnits = gameState.CurrentPlayer.ActiveBoard.GetAllUnits();
        var deadUnits = new List<Unit>();

        for (int i = 0; i < allUnits.Count; i++)
        {
            Unit unit = allUnits[i];
            if (!unit.IsEmpty() && !unit.IsAlive())
            {
                deadUnits.Add(unit);
            }
        }

        var deadReserveMonsters = gameState.CurrentPlayer.GetDeadReserveMonsters();
        deadUnits.AddRange(deadReserveMonsters);

        return deadUnits;
    }
}