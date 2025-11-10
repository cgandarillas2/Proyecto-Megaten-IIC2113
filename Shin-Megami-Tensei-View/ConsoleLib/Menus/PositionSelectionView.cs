using Shin_Megami_Tensei_Model.Game;

namespace Shin_Megami_Tensei_View.ConsoleLib;

public class PositionSelectionView
{
    private readonly View _view;

    public PositionSelectionView(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    public void ShowPositionMenu(Board board)
    {
        _view.WriteSeparation();
        _view.WriteLine("Seleccione una posición para invocar");

        for (int i = 1; i <= 3; i++)
        {
            var unit = board.GetUnitAt(i);
            if (unit.IsEmpty())
            {
                _view.WriteLine($"{i}-Vacío (Puesto {i + 1})");
            }
            else
            {
                var hp = unit.CurrentStats.CurrentHP;
                var maxHp = unit.CurrentStats.MaxHP;
                var mp = unit.CurrentStats.CurrentMP;
                var maxMp = unit.CurrentStats.MaxMP;
                _view.WriteLine($"{i}-{unit.Name} HP:{hp}/{maxHp} MP:{mp}/{maxMp} (Puesto {i + 1})");
            }
        }

        _view.WriteLine("4-Cancelar");
    }
}
