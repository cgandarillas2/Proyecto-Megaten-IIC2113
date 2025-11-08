using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Services;

/// <summary>
/// Service responsible for summon operations following SRP.
/// Extracted from CombatController to keep business logic in the model layer.
/// </summary>
public class SummonService
{
    public void PerformSummon(Monster monster, int position, GameState gameState)
    {
        if (monster == null)
        {
            throw new ArgumentNullException(nameof(monster));
        }

        var board = gameState.CurrentPlayer.ActiveBoard;
        var currentUnit = board.GetUnitAt(position);

        if (!currentUnit.IsEmpty() && currentUnit is Monster existingMonster)
        {
            gameState.CurrentPlayer.AddMonsterToReserve(existingMonster);
        }

        gameState.CurrentPlayer.RemoveMonsterFromReserve(monster);
        board.PlaceUnit(monster, position);
    }

    public List<Monster> GetAvailableMonsters(GameState gameState)
    {
        gameState.CurrentPlayer.ReorderReserveFromSelectionFile();
        return gameState.CurrentPlayer.GetAliveReserveMonsters();
    }

    public bool HasAvailableMonsters(GameState gameState)
    {
        return GetAvailableMonsters(gameState).Count > 0;
    }

    public List<int> GetAvailablePositions(GameState gameState)
    {
        var positions = new List<int>();
        var board = gameState.CurrentPlayer.ActiveBoard;

        for (int i = 0; i < 4; i++)
        {
            positions.Add(i);
        }

        return positions;
    }
}
