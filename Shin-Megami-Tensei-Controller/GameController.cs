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
        var teams = _teamController.SelectAndLoadTeams(_teamsFolder);

        if (!teams.HasValue)
        {
            return;
        }

        var (player1, player2) = teams.Value;
        StartCombat(player1, player2);
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
            bool actionExecuted = _combatController.ExecuteRound(gameState);

            if (!actionExecuted)
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