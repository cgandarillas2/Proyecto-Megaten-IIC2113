using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Action;

public class SummonService
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

    public void UpdateActionQueue(Unit actor, Monster monster, int position, GameState gameState)
    {
        if (IsEmptyPosition(position, gameState))
        {
            gameState.ActionQueue.AddToEnd(monster);
        }
        else
        {
            var queuePosition = GetQueuePositionForSwap(position, gameState);
            gameState.ActionQueue.SwapUnit(monster, queuePosition);
        }
    }

    public int FindMonsterPosition(Monster monster, GameState gameState)
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
        return gameState.CurrentPlayer.ActiveBoard.IsPositionEmpty(position);
    }

    private int GetQueuePositionForSwap(int position, GameState gameState)
    {
        var board = gameState.CurrentPlayer.ActiveBoard;
        var unitInPosition = board.GetUnitAt(position);
        return gameState.ActionQueue.FindMonsterPosition(unitInPosition);
    }
}