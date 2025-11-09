using Shin_Megami_Tensei.Exceptions;
using Shin_Megami_Tensei_Model.Exceptions;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei_View.ConsoleLib;

namespace Shin_Megami_Tensei;

public class GameController
{
    private readonly TeamController _teamController;
    private readonly CombatController _combatController;
    private readonly GameFlowView _gameView;
    private readonly string _teamsFolder;

    public GameController(
        TeamController teamController,
        View view,
        string teamsFolder)
    {
        _teamController = teamController ?? throw new ArgumentNullException(nameof(teamController));
        _combatController = new CombatController(view);
        _gameView = new GameFlowView(view);
        _teamsFolder = teamsFolder ?? throw new ArgumentNullException(nameof(teamsFolder));
    }

    public void Run()
    {
        try
        {
            var (player1, player2) = _teamController.SelectAndLoadTeams(_teamsFolder);
            StartCombat(player1, player2);
        }
        catch (Exception ex) when (ex is NoTeamsAvailableException
                                    or OperationCancelledException
                                    or InvalidTeamException
                                    or ArgumentException)
        {
            // User cancelled or error occurred during team selection/loading
            // Simply exit the game
            return;
        }
    }

    private void StartCombat(Team player1, Team player2)
    {
        var gameState = new GameState(player1, player2);
        ExecuteGameLoop(gameState);
    }

    private void ExecuteGameLoop(GameState gameState)
    {
        while (!gameState.IsGameOver())
        {
            ExecutePlayerRound(gameState);

            if (gameState.IsGameOver())
            {
                DisplayGameOver(gameState);
                return;
            }

            gameState.SwitchPlayer();
        }
    }

    private void ExecutePlayerRound(GameState gameState)
    {
        _combatController.InitialRoundHeaderMessage(gameState);

        while (gameState.HasTurnsRemaining() && !gameState.IsGameOver())
        {
            var executionResult = _combatController.ExecuteRound(gameState);

            if (executionResult.WasCancelled())
            {
                break;
            }
        }
    }

    private void DisplayGameOver(GameState gameState)
    {
        var winner = gameState.GetWinner();
        _gameView.ShowGameOver(winner);
    }
}