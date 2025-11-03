using Shin_Megami_Tensei_Model.Game;

namespace Shin_Megami_Tensei_View.ConsoleLib;

public class GameFlowView
{
    private readonly View _view;

    public GameFlowView(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    public void ShowGameOver(Team winner)
    {
        var samurai = winner.ActiveBoard.GetSamurai();
        
        _view.WriteSeparation();
        _view.WriteLine($"Ganador: {samurai.Name} ({winner.PlayerName})");
    }

    public void ShowInvalidTeamError()
    {
        _view.WriteLine("Archivo de equipos inválido");
    }

    public void ShowNoTeamsAvailable()
    {
        _view.WriteLine("No hay archivos de equipos disponibles");
    }
}