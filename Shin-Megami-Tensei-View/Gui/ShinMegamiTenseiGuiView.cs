using Shin_Megami_Tensei_GUI;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_View.Gui.Adapters;

namespace Shin_Megami_Tensei_View.Gui;

/// <summary>
/// Vista que implementa la interfaz gráfica usando SMTGUI.
/// </summary>
public class ShinMegamiTenseiGuiView : IShinMegamiTenseiView
{
    private readonly SMTGUI _window;
    private readonly TeamController _teamController;

    public ShinMegamiTenseiGuiView(TeamController teamController)
    {
        _window = new SMTGUI();
        _teamController = teamController ?? throw new ArgumentNullException(nameof(teamController));
    }

    /// <summary>
    /// Inicia la ventana GUI y ejecuta el callback del programa.
    /// </summary>
    public void Start(Action startProgram)
    {
        _window.Start(startProgram);
    }

    public void ShowEndGameMessage(string message)
    {
        _window.ShowEndGameMessage(message);
    }

    public (Team player1, Team player2) SelectTeams(string teamsFolder)
    {
        // Obtener información de equipos desde la GUI
        ITeamInfo team1Info = _window.GetTeamInfo(1);
        ITeamInfo team2Info = _window.GetTeamInfo(2);

        // Convertir ITeamInfo a Team usando el TeamController
        var player1 = ConvertTeamInfoToTeam(team1Info);
        var player2 = ConvertTeamInfoToTeam(team2Info);

        return (player1, player2);
    }

    public void UpdateGameState(GameState gameState, IEnumerable<string> options, IEnumerable<string> order)
    {
        var state = new StateAdapter(gameState, options, order);
        _window.Update(state);
    }

    public string GetSelectedAction()
    {
        IClickedElement clickedElement;
        do
        {
            clickedElement = _window.GetClickedElement();
        } while (clickedElement.Type != ClickedElementType.Button);

        return clickedElement.Text;
    }

    public int? GetSelectedTarget()
    {
        IClickedElement clickedElement;
        do
        {
            clickedElement = _window.GetClickedElement();
        } while (clickedElement.Type != ClickedElementType.UnitInBoard &&
                 clickedElement.Text != "Cancelar");

        if (clickedElement.Text == "Cancelar")
            return null;

        // El PlayerId indica el jugador, necesitamos determinar la posición en el board
        // Esto requeriría más lógica para mapear el unit seleccionado a un índice
        // Por ahora retornamos null como placeholder
        return null;
    }

    public int? GetSelectedSkill()
    {
        IClickedElement clickedElement;
        do
        {
            clickedElement = _window.GetClickedElement();
        } while (clickedElement.Type != ClickedElementType.Button ||
                 (!clickedElement.Text.Contains("MP:") && clickedElement.Text != "Cancelar"));

        if (clickedElement.Text == "Cancelar")
            return null;

        // Extraer el índice de la habilidad del texto del botón
        // Formato esperado: "1-Skill Name MP:X"
        if (clickedElement.Text.Contains("-"))
        {
            var parts = clickedElement.Text.Split('-');
            if (int.TryParse(parts[0], out int index))
                return index - 1; // Convertir a índice 0-based
        }

        return null;
    }

    public int? GetSelectedMonster()
    {
        IClickedElement clickedElement;
        do
        {
            clickedElement = _window.GetClickedElement();
        } while (clickedElement.Type != ClickedElementType.UnitInReserve &&
                 clickedElement.Text != "Cancelar");

        if (clickedElement.Text == "Cancelar")
            return null;

        // Similar al GetSelectedTarget, necesitamos mapear el unit a un índice
        return null;
    }

    public int? GetSelectedPosition()
    {
        IClickedElement clickedElement;
        do
        {
            clickedElement = _window.GetClickedElement();
        } while (clickedElement.Type != ClickedElementType.Button ||
                 (!char.IsDigit(clickedElement.Text[0]) && clickedElement.Text != "Cancelar"));

        if (clickedElement.Text == "Cancelar")
            return null;

        // Extraer el índice de posición del texto del botón
        if (int.TryParse(clickedElement.Text.Substring(0, 1), out int position))
            return position;

        return null;
    }

    private Team ConvertTeamInfoToTeam(ITeamInfo teamInfo)
    {
        // Usar el TeamController para crear un Team desde la información de la GUI
        // Esto requiere acceso a los repositorios de unidades

        // Por ahora, esto es un placeholder que necesita ser implementado
        // con la lógica correcta para convertir ITeamInfo a Team
        throw new NotImplementedException("ConvertTeamInfoToTeam needs to be implemented with proper team loading logic");
    }
}
