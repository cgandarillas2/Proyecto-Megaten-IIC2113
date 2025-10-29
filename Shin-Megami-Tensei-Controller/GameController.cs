using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_View;

namespace Shin_Megami_Tensei;

public class GameController
{
    private readonly TeamController _teamController;
    private readonly CombatController _combatController;
    private readonly View _view;
    private readonly string _teamsFolder;

    public GameController(
        TeamController teamController,
        View view,
        string teamsFolder)
    {
        _teamController = teamController ?? throw new ArgumentNullException(nameof(teamController));
        _combatController = new CombatController(view);
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _teamsFolder = teamsFolder ?? throw new ArgumentNullException(nameof(teamsFolder));
    }

    public void Run()
    {
        var teams = SelectTeams();

        if (teams == null)
        {
            return;
        }

        var (player1, player2) = teams.Value;
        StartCombat(player1, player2);
    }

    private (Team, Team)? SelectTeams()
    {
        return _teamController.SelectAndLoadTeams(_teamsFolder);
    }

    private void StartCombat(Team player1, Team player2)
    {
        var gameState = new GameState(player1, player2);
        RunCombatLoop(gameState);
    }

    private void RunCombatLoop(GameState gameState)
    {
        while (!gameState.IsGameOver())
        {
            _combatController.InitialRoundHeaderMessage(gameState);
            while (gameState.HasTurnsRemaining() && !gameState.IsGameOver()) 
            {
                var actionExecuted = _combatController.ExecuteRound(gameState);
                    
                if (!actionExecuted)
                {
                    break;
                }

                if (gameState.IsGameOver())
                {
                    DisplayGameOver(gameState);
                    return;
                }
            }
                
            if (gameState.IsGameOver())
            {
                DisplayGameOver(gameState);
                break;
            }
                
            gameState.SwitchPlayer();
        }
    }

    
    private void DisplayGameOver(GameState gameState)
    {
        var winner = gameState.GetWinner();
        var winnerName = winner.PlayerName;
        
        var samurai = winner.ActiveBoard.GetSamurai();
        _view.WriteSeparation();
        _view.WriteLine($"Ganador: {samurai.Name} ({winnerName})");
    }
}