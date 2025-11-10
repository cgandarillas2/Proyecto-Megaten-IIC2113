using Shin_Megami_Tensei_GUI;
using Shin_Megami_Tensei_Model.Game;

namespace Shin_Megami_Tensei_View.Gui.Adapters;

/// <summary>
/// Adapter que convierte el GameState del modelo a IState para la GUI.
/// </summary>
public class StateAdapter : IState
{
    private readonly GameState _gameState;
    private readonly IEnumerable<string> _options;
    private readonly IEnumerable<string> _order;

    public StateAdapter(
        GameState gameState,
        IEnumerable<string> options,
        IEnumerable<string> order)
    {
        _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _order = order ?? throw new ArgumentNullException(nameof(order));
    }

    public IPlayer Player1 => new PlayerAdapter(_gameState.Player1);
    public IPlayer Player2 => new PlayerAdapter(_gameState.Player2);
    public IEnumerable<string> Options => _options;
    public int Turns => _gameState.CurrentPlayer.GetFullTurns();
    public int BlinkingTurns => _gameState.CurrentPlayer.GetBlinkingTurns();
    public IEnumerable<string> Order => _order;
}
