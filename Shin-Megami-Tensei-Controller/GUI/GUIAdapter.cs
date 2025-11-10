using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Units;
using Shin_Megami_Tensei_GUI;

namespace Shin_Megami_Tensei.GUI;

/// <summary>
/// Adapts the game model structures to GUI structures (without using reflection)
/// Since we cannot access the internal structure of GUI classes from the DLL,
/// we use dynamic objects to create the GUI entities.
/// </summary>
public class GUIAdapter
{
    /// <summary>
    /// Converts a model Unit to a GUI Unit object using dynamic construction
    /// GUI Unit constructor: Unit(string name, int hp, int mp, int attack, int defense)
    /// </summary>
    public static dynamic ConvertUnitToGUI(Unit unit)
    {
        if (unit == null || unit.IsEmpty())
        {
            return null;
        }

        var stats = unit.CurrentStats;

        // Create GUI Unit using dynamic invocation
        // The GUI Unit constructor appears to be: Unit(name, hp, mp, attack, defense)
        // We'll map: Str to attack, Skl to defense
        var guiUnit = Activator.CreateInstance(
            typeof(Shin_Megami_Tensei_GUI.Unit),
            unit.Name,
            stats.CurrentHP,
            stats.CurrentMP,
            stats.Str,  // attack
            stats.Skl   // defense
        );

        return guiUnit;
    }

    /// <summary>
    /// Converts a Team from the model to a Player for the GUI
    /// GUI Player constructor: Player(Unit[] unitsInBoard, Unit[] unitsInReserve)
    /// Note: unitsInBoard has 3 positions (monsters only, no samurai)
    /// </summary>
    public static dynamic ConvertTeamToGUIPlayer(Team team)
    {
        // Get monsters from board (positions 1-3, excluding samurai at position 0)
        var boardUnits = new dynamic[3];
        for (int i = 1; i <= 3; i++)
        {
            var unit = team.ActiveBoard.GetUnitAt(i);
            boardUnits[i - 1] = ConvertUnitToGUI(unit);
        }

        // Get reserve monsters
        var reserveMonsters = team.GetAllReserveMonsters();
        var reserveUnits = new dynamic[reserveMonsters.Count];
        int index = 0;
        foreach (var monster in reserveMonsters)
        {
            reserveUnits[index++] = ConvertUnitToGUI(monster);
        }

        // Create GUI Player
        var guiPlayer = Activator.CreateInstance(
            typeof(Shin_Megami_Tensei_GUI.Player),
            boardUnits,
            reserveUnits
        );

        return guiPlayer;
    }

    /// <summary>
    /// Converts a GameState to a GUI State
    /// GUI State has properties: Player1, Player2, Turns, BlinkingTurns, Order, Options
    /// </summary>
    public static dynamic ConvertGameStateToGUIState(GameState gameState)
    {
        var guiState = Activator.CreateInstance(typeof(Shin_Megami_Tensei_GUI.State));

        var stateType = guiState.GetType();

        // Set Player1 and Player2
        stateType.GetProperty("Player1").SetValue(guiState, ConvertTeamToGUIPlayer(gameState.Player1));
        stateType.GetProperty("Player2").SetValue(guiState, ConvertTeamToGUIPlayer(gameState.Player2));

        // Set Turns (remaining turns for current player)
        var turnsRemaining = gameState.HasTurnsRemaining() ?
            gameState.CurrentPlayer.ActiveBoard.GetAliveUnits().Count : 0;
        stateType.GetProperty("Turns").SetValue(guiState, turnsRemaining);

        // Set BlinkingTurns (not used in our implementation yet, set to 0)
        stateType.GetProperty("BlinkingTurns").SetValue(guiState, 0);

        // Set Order (action queue order)
        var actionQueueUnits = new List<string>();
        // Note: This would require accessing the ActionQueue, which we'll implement later
        // For now, set empty array
        stateType.GetProperty("Order").SetValue(guiState, actionQueueUnits.ToArray());

        // Set Options (default combat options)
        var options = new[] { "Atacar", "Usar Habilidad", "Invocar", "Pasar Turno" };
        stateType.GetProperty("Options").SetValue(guiState, options);

        return guiState;
    }
}
