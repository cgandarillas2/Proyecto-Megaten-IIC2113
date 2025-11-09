using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_View.ConsoleLib;

public class UnitMenuRenderer: IMenuRenderer<Unit>
{
    private readonly View _view;
    private readonly string _promptMessage;

    public UnitMenuRenderer(View view, string promptMessage)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _promptMessage = promptMessage ?? "Seleccione una unidad";
    }

    public void Render(UnitsCollection units, object context = null)
    {
        var actor = context as Unit;

        _view.WriteSeparation();
        _view.WriteLine($"{_promptMessage} para {actor?.Name ?? ""}");

        for (int i = 0; i < units.Count; i++)
        {
            var unit = units[i];
            var hp = unit.CurrentStats.CurrentHP;
            var maxHp = unit.CurrentStats.MaxHP;
            var mp = unit.CurrentStats.CurrentMP;
            var maxMp = unit.CurrentStats.MaxMP;

            _view.WriteLine($"{i + 1}-{unit.Name} HP:{hp}/{maxHp} MP:{mp}/{maxMp}");
        }

        _view.WriteLine($"{units.Count + 1}-Cancelar");
    }
}