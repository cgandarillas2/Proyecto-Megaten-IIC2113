using Shin_Megami_Tensei_Model.Game;

namespace Shin_Megami_Tensei_View.Gui;

/// <summary>
/// Vista de consola que implementa IShinMegamiTenseiView.
/// Permite que el controlador funcione tanto con consola como con GUI.
/// </summary>
public class ShinMegamiTenseiConsoleView : IShinMegamiTenseiView
{
    private readonly View _view;
    private readonly TeamController _teamController;

    public ShinMegamiTenseiConsoleView(View view, TeamController teamController)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _teamController = teamController ?? throw new ArgumentNullException(nameof(teamController));
    }

    public void ShowEndGameMessage(string message)
    {
        _view.WriteLine(message);
    }

    public (Team player1, Team player2) SelectTeams(string teamsFolder)
    {
        return _teamController.SelectAndLoadTeams(teamsFolder);
    }

    public void UpdateGameState(GameState gameState, IEnumerable<string> options, IEnumerable<string> order)
    {
        // La vista de consola no necesita actualización continua del estado
        // El estado se muestra a través de otros métodos específicos
    }

    public string GetSelectedAction()
    {
        // Esto debería delegarse al menú de acciones
        throw new NotImplementedException("Use ActionMenuView directly for console");
    }

    public int? GetSelectedTarget()
    {
        throw new NotImplementedException("Use TargetSelectionView directly for console");
    }

    public int? GetSelectedSkill()
    {
        throw new NotImplementedException("Use SkillSelectionView directly for console");
    }

    public int? GetSelectedMonster()
    {
        throw new NotImplementedException("Use menu views directly for console");
    }

    public int? GetSelectedPosition()
    {
        throw new NotImplementedException("Use PositionSelectionView directly for console");
    }
}
