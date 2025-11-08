using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_View;

namespace Shin_Megami_Tensei.Services;

public class PositionSelector
{
    private readonly View _view;

    public PositionSelector(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    public int SelectPosition(GameState gameState)
    {
        while (true)
        {
            DisplayPositionMenu(gameState);

            var choice = _view.ReadLine();

            if (!int.TryParse(choice, out int selection))
            {
                continue;
            }

            if (selection == 4)
            {
                return -1;
            }

            if (selection >= 1 && selection <= 3)
            {
                return selection;
            }
        }
    }

    private void DisplayPositionMenu(GameState gameState)
    {
        _view.WriteSeparation();
        _view.WriteLine("Seleccione una posición para invocar");

        var board = gameState.CurrentPlayer.ActiveBoard;

        for (int i = 1; i <= 3; i++)
        {
            var unit = board.GetUnitAt(i);
            if (unit.IsEmpty())
            {
                _view.WriteLine($"{i}-Vacío (Puesto {i + 1})");
            }
            else
            {
                _view.WriteLine($"{i}-{unit.Name} HP:{unit.CurrentStats.CurrentHP}/{unit.CurrentStats.MaxHP} MP:{unit.CurrentStats.CurrentMP}/{unit.CurrentStats.MaxMP} (Puesto {i + 1})");
            }
        }

        _view.WriteLine("4-Cancelar");
    }
}
