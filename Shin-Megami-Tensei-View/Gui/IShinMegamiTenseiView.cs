using Shin_Megami_Tensei_Model.Game;

namespace Shin_Megami_Tensei_View.Gui;

/// <summary>
/// Interfaz común para todas las vistas del juego (consola y GUI).
/// Permite que el controlador funcione con diferentes implementaciones de vista.
/// </summary>
public interface IShinMegamiTenseiView
{
    /// <summary>
    /// Muestra un mensaje de fin de juego (equipo inválido, rendición, victoria).
    /// </summary>
    void ShowEndGameMessage(string message);

    /// <summary>
    /// Solicita al usuario que seleccione los equipos.
    /// </summary>
    /// <param name="teamsFolder">Carpeta donde están los archivos de equipos</param>
    /// <returns>Tupla con los dos equipos seleccionados</returns>
    (Team player1, Team player2) SelectTeams(string teamsFolder);

    /// <summary>
    /// Actualiza la vista con el estado actual del juego.
    /// </summary>
    void UpdateGameState(GameState gameState, IEnumerable<string> options, IEnumerable<string> order);

    /// <summary>
    /// Obtiene la acción seleccionada por el usuario.
    /// </summary>
    string GetSelectedAction();

    /// <summary>
    /// Obtiene el objetivo seleccionado por el usuario.
    /// </summary>
    int? GetSelectedTarget();

    /// <summary>
    /// Obtiene la habilidad seleccionada por el usuario.
    /// </summary>
    int? GetSelectedSkill();

    /// <summary>
    /// Obtiene el monstruo seleccionado para invocar.
    /// </summary>
    int? GetSelectedMonster();

    /// <summary>
    /// Obtiene la posición seleccionada en el tablero.
    /// </summary>
    int? GetSelectedPosition();
}
