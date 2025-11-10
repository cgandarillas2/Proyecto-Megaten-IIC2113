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
    private readonly TeamBuilder _teamBuilder;
    private GameState? _currentGameState;

    public ShinMegamiTenseiGuiView(TeamBuilder teamBuilder)
    {
        _window = new SMTGUI();
        _teamBuilder = teamBuilder ?? throw new ArgumentNullException(nameof(teamBuilder));
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

        // Construir equipos usando TeamBuilder
        var player1 = _teamBuilder.BuildFromTeamInfo("J1", team1Info);
        var player2 = _teamBuilder.BuildFromTeamInfo("J2", team2Info);

        return (player1, player2);
    }

    public void UpdateGameState(GameState gameState, IEnumerable<string> options, IEnumerable<string> order)
    {
        // Guardar referencia al estado actual para mapear selecciones
        _currentGameState = gameState;

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
                 clickedElement.Type != ClickedElementType.Button);

        // Si es botón, debe ser "Cancelar"
        if (clickedElement.Type == ClickedElementType.Button)
            return null;

        // Mapear el unit seleccionado a su posición en el board
        if (_currentGameState == null)
            return null;

        var targetTeam = clickedElement.PlayerId == 1
            ? _currentGameState.Player1
            : _currentGameState.Player2;

        // Buscar la posición del unit en el board
        for (int i = 0; i < 4; i++)
        {
            var unit = targetTeam.ActiveBoard.GetUnitAt(i);
            if (!unit.IsEmpty() && unit.Name == clickedElement.Text)
                return i;
        }

        return null;
    }

    public int? GetSelectedSkill()
    {
        IClickedElement clickedElement;
        do
        {
            clickedElement = _window.GetClickedElement();
        } while (clickedElement.Type != ClickedElementType.Button);

        if (clickedElement.Text == "Cancelar")
            return null;

        // Extraer el índice de la habilidad del texto del botón
        // Formato esperado: "1-Skill Name MP:X" o solo "Cancelar"
        if (clickedElement.Text.Contains("-"))
        {
            var parts = clickedElement.Text.Split('-');
            if (parts.Length > 0 && int.TryParse(parts[0], out int index))
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
                 clickedElement.Type != ClickedElementType.Button);

        // Si es botón, debe ser "Cancelar"
        if (clickedElement.Type == ClickedElementType.Button)
            return null;

        // Mapear el monster seleccionado a su índice en la reserva
        if (_currentGameState == null)
            return null;

        var reserveList = _currentGameState.CurrentPlayer.Reserve.ToList();
        for (int i = 0; i < reserveList.Count; i++)
        {
            if (reserveList[i].Name == clickedElement.Text)
                return i;
        }

        return null;
    }

    public int? GetSelectedPosition()
    {
        IClickedElement clickedElement;
        do
        {
            clickedElement = _window.GetClickedElement();
        } while (clickedElement.Type != ClickedElementType.Button);

        if (clickedElement.Text == "Cancelar")
            return null;

        // El texto del botón debería ser algo como "A-Empty" o "B-UnitName"
        // Extraemos la letra de la posición
        if (clickedElement.Text.Length > 0)
        {
            char positionLetter = clickedElement.Text[0];
            return positionLetter switch
            {
                'A' => 0,
                'B' => 1,
                'C' => 2,
                'D' => 3,
                _ => null
            };
        }

        return null;
    }
}
