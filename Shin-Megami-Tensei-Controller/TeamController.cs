using Shin_Megami_Tensei.Exceptions;
using Shin_Megami_Tensei_Model.Repositories;
using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Exceptions;
using Shin_Megami_Tensei_View.ConsoleLib;

namespace Shin_Megami_Tensei;

public class TeamController
{
    private readonly TeamRepository _teamRepository;
    private readonly GameFlowView _gameView;
    private readonly TeamSelectionView _selectionView;

    public TeamController(TeamRepository teamRepository, View view)
    {
        _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
        _gameView = new GameFlowView(view);
        _selectionView = new TeamSelectionView(view);
    }

    public (Team, Team) SelectAndLoadTeams(string teamsFolder)
    {
        var teamFiles = _teamRepository.GetAvailableTeamFiles(teamsFolder);

        if (teamFiles.Count == 0)
        {
            _gameView.ShowNoTeamsAvailable();
            throw new NoTeamsAvailableException();
        }

        var selectedFile = _selectionView.SelectTeamFile(teamFiles);

        if (selectedFile == null)
        {
            throw new OperationCancelledException("Selección de equipo cancelada");
        }

        return LoadTeamsFromFile(selectedFile);
    }

    private (Team, Team) LoadTeamsFromFile(string filePath)
    {
        try
        {
            return _teamRepository.LoadTeams(filePath);
        }
        catch (Exception ex) when (ex is InvalidTeamException or ArgumentException)
        {
            _gameView.ShowInvalidTeamError();
            throw;
        }
    }
}