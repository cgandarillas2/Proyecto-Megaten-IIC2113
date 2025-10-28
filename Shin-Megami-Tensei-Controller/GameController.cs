using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_View;

namespace Shin_Megami_Tensei;

public class GameController
{
    private readonly TeamController _teamController;
    private readonly View _view;
    private readonly string _teamsFolder;

    public GameController(
        TeamController teamController,
        View view,
        string teamsFolder)
    {
        _teamController = teamController ?? throw new ArgumentNullException(nameof(teamController));
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
        _view.WriteSeparation();
        DisplayGameStart(player1, player2);
        // TODO: Implementar flujo de combate en E1
    }

    private void DisplayGameStart(Team player1, Team player2)
    {
        _view.WriteLine($"Ronda de {player1.Leader.Name} ({player1.PlayerName})");
        _view.WriteSeparation();
    }
}