using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_View.ConsoleLib;

public class TargetSelectionView
{
    private readonly View _view;

    public TargetSelectionView(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    public void ShowTargetMenu(string actorName, UnitsCollection targets)
    {
        _view.WriteSeparation();
        _view.WriteLine($"Seleccione un objetivo para {actorName}");

        for (int i = 0; i < targets.Count; i++)
        {
            ShowTargetOption(i + 1, targets[i]);
        }

        _view.WriteLine($"{targets.Count + 1}-Cancelar");
    }

    public void ShowSummonTargets(UnitsCollection targets)
    {
        _view.WriteSeparation();
        _view.WriteLine($"Seleccione un monstruo para invocar");

        for (int i = 0; i < targets.Count; i++)
        {
            ShowTargetOption(i + 1, targets[i]);
        }

        _view.WriteLine($"{targets.Count + 1}-Cancelar");
    }

    private void ShowTargetOption(int number, Unit target)
    {
        var hp = target.CurrentStats.CurrentHP;
        var maxHp = target.CurrentStats.MaxHP;
        var mp = target.CurrentStats.CurrentMP;
        var maxMp = target.CurrentStats.MaxMP;

        _view.WriteLine($"{number}-{target.Name} HP:{hp}/{maxHp} MP:{mp}/{maxMp}");
    }
}
