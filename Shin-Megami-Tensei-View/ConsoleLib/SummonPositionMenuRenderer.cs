using Shin_Megami_Tensei_Model.Game;

namespace Shin_Megami_Tensei_View.ConsoleLib;

public class SummonPositionMenuRenderer: IMenuRenderer<int>
{
    private readonly View _view;
    private readonly GameState _gameState;

    public SummonPositionMenuRenderer(View view, GameState gameState)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
    }

    public void Render(List<int> positions, object context = null)
    {
        _view.WriteSeparation();
        _view.WriteLine("Seleccione una posición para invocar");

        var board = _gameState.CurrentPlayer.ActiveBoard;

        foreach (var position in positions)
        {
            var unit = board.GetUnitAt(position);
            if (unit.IsEmpty())
            {
                _view.WriteLine($"{position}-Vacío (Puesto {position + 1})");
            }
            else
            {
                _view.WriteLine($"{position}-{unit.Name} HP:{unit.CurrentStats.CurrentHP}/{unit.CurrentStats.MaxHP} MP:{unit.CurrentStats.CurrentMP}/{unit.CurrentStats.MaxMP} (Puesto {position + 1})");
            }
        }

        _view.WriteLine("4-Cancelar");
    }
}