
using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Game;

public class GameState
{
public Team Player1 { get; }
    public Team Player2 { get; }
    public Team CurrentPlayer { get; private set; }
    public TurnState CurrentTurnState { get; private set; }
    public ActionQueue ActionQueue { get; private set; }
    public int CurrentRound { get; private set; }
    

    public GameState(Team player1, Team player2)
    {
        Player1 = player1 ?? throw new ArgumentNullException(nameof(player1));
        Player2 = player2 ?? throw new ArgumentNullException(nameof(player2));
        CurrentPlayer = player1;
        CurrentRound = 1;
        InitializeRound();
    }

    public Team GetOpponent()
    {
        return CurrentPlayer == Player1 ? Player2 : Player1;
    }

    public string GetOpponentName()
    {
        var opponent = GetOpponent();
        return opponent.PlayerName;
    }

    public List<Unit> GetOpponentAliveUnits()
    {
        var opponent = GetOpponent();
        return opponent.ActiveBoard.GetAliveUnits();
    }

    public Unit GetCurrentActingUnit()
    {
        return ActionQueue.GetNext();
    }

    public void AdvanceActionQueue()
    {
        var current = ActionQueue.GetNext();
        ActionQueue.MoveToEnd(current);
    }
    
    public TurnConsumptionResult ApplyTurnConsumption(TurnConsumption consumption)
    {
        return CurrentTurnState.ApplyConsumption(consumption);
    }

    public bool HasTurnsRemaining()
    {
        return CurrentTurnState.HasTurns();
    }

    public void SwitchPlayer()
    {
        CurrentPlayer = GetOpponent();
        InitializeRound();
    }

    public void NextRound()
    {
        CurrentRound++;
        CurrentPlayer = Player1;
        InitializeRound();
    }

    public bool IsGameOver()
    {
        return !Player1.HasAliveUnitsOnBoard() || !Player2.HasAliveUnitsOnBoard();
    }

    public Team GetWinner()
    {
        if (!Player1.HasAliveUnitsOnBoard())
        {
            return Player2;
        }
        
        if (!Player2.HasAliveUnitsOnBoard())
        {
            return Player1;
        }
        
        return null;
    }

    public void IncrementSkillCount()
    {
        CurrentPlayer.IncrementSkillCount();
    }
    
    public int GetCurrentPlayerSkillCount()
    {
        return CurrentPlayer.SkillCount;
    }

    public void SurrenderByKillingAllUnits()
    {
        var aliveUnits = CurrentPlayer.ActiveBoard.GetAliveUnits();
        foreach (var unit in aliveUnits)
        {
            unit.KillInstantly();
        }
    }

    private void InitializeRound()
    {
        var aliveUnits = CurrentPlayer.ActiveBoard.GetAliveUnits();
        CurrentTurnState = new TurnState(aliveUnits.Count);
        ActionQueue = new ActionQueue(aliveUnits);
    }

    private Team FindTeamWithUnit(Unit unit)
    {
        if (Player1.ActiveBoard.GetAllUnits().Contains(unit))
        {
            return Player1;
        }
        return Player2;
    }

}