using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei.Services;

public class SummonCoordinator
{
    public void PerformSummon(Monster monster, int position, GameState gameState)
    {
        var board = gameState.CurrentPlayer.ActiveBoard;
        var currentUnit = board.GetUnitAt(position);

        if (!currentUnit.IsEmpty() && currentUnit is Monster existingMonster)
        {
            gameState.CurrentPlayer.AddMonsterToReserve(existingMonster);
        }

        gameState.CurrentPlayer.RemoveMonsterFromReserve(monster);
        gameState.CurrentPlayer.ReorderReserveFromSelectionFile();

        board.PlaceUnit(monster, position);
    }

    public void AddToActionQueue(Monster target, int position, GameState gameState, Unit actor)
    {
        if (IsEmptyPosition(position, gameState))
        {
            gameState.ActionQueue.AddToEnd(target);
        }
        else
        {
            var queuePosition = GetQueuePositionToAdd(position, gameState);
            gameState.ActionQueue.SwapUnit(target, queuePosition);
        }
    }

    public int FindMonsterPosition(Unit monster, GameState gameState)
    {
        var board = gameState.CurrentPlayer.ActiveBoard;
        for (int i = 1; i <= 3; i++)
        {
            if (board.GetUnitAt(i) == monster)
            {
                return i;
            }
        }
        return -1;
    }

    private bool IsEmptyPosition(int position, GameState gameState)
    {
        var board = gameState.CurrentPlayer.ActiveBoard;
        return board.IsPositionEmpty(position);
    }

    private int GetQueuePositionToAdd(int position, GameState gameState)
    {
        var board = gameState.CurrentPlayer.ActiveBoard;
        var unitInPosition = board.GetUnitAt(position);

        return gameState.ActionQueue.FindMonsterPosition(unitInPosition);
    }
}
