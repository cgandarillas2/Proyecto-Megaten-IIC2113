using Shin_Megami_Tensei_Model.Repositories;
using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei_Model;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Exceptions;

namespace Shin_Megami_Tensei;

public class TeamController
{
    private readonly TeamRepository _teamRepository;
    private readonly View _view;
    
    public TeamController(TeamRepository teamRepository, View view)
    {
        _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }
    
    public (Team player1, Team player2)? SelectAndLoadTeams(string teamsFolder)
    {
        var teamFiles = GetTeamFiles(teamsFolder);
            
        if (teamFiles.Count == 0)
        {
            _view.WriteLine("No hay archivos de equipos disponibles");
            return null;
        }

        DisplayTeamFiles(teamFiles);
        var selectedFile = ReadTeamSelection(teamFiles);

        if (selectedFile == null)
        {
            return null;
        }

        return LoadTeamsFromFile(selectedFile);
    }
    
    private List<string> GetTeamFiles(string teamsFolder)
    {
        return _teamRepository.GetAvailableTeamFiles(teamsFolder);
    }

    private void DisplayTeamFiles(List<string> teamFiles)
    {
        _view.WriteLine("Elige un archivo para cargar los equipos");
            
        for (int i = 0; i < teamFiles.Count; i++)
        {
            var fileName = Path.GetFileName(teamFiles[i]);
            _view.WriteLine($"{i}: {fileName}");
        }
        
    }
    
    private string ReadTeamSelection(List<string> teamFiles)
    {
        var input = _view.ReadLine();
            
        if (!TryParseSelection(input, out int selection))
        {
            return null;
        }

        if (!IsValidSelection(selection, teamFiles.Count))
        {
            return null;
        }

        return teamFiles[selection];
    }

    private (Team, Team)? LoadTeamsFromFile(string filePath)
    {
        try
        {
            return _teamRepository.LoadTeams(filePath);
        }
        catch (InvalidTeamException)
        {
            _view.WriteLine("Archivo de equipos inválido");
            return null;
        }
        catch (ArgumentException)
        {
            _view.WriteLine("Archivo de equipos inválido");
            return null;
        }
        catch (Exception)
        {
            _view.WriteLine("Archivo de equipos inválido");
            return null;
        }
    }
    
    private static bool TryParseSelection(string input, out int selection)
    {
        return int.TryParse(input, out selection);
    }

    private static bool IsValidSelection(int selection, int maxCount)
    {
        return selection >= 0 && selection < maxCount;
    }
}