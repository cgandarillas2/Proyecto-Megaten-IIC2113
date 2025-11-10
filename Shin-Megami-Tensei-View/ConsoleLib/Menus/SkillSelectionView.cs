using Shin_Megami_Tensei_View;

namespace Shin_Megami_Tensei_View.ConsoleLib;

/// <summary>
/// View responsible for displaying skill selection menus and messages.
/// Encapsulates all UI logic related to skill selection.
/// </summary>
public class SkillSelectionView
{
    private readonly View _view;

    public SkillSelectionView(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    public void ShowNoSkillsAvailable(string actorName)
    {
        _view.WriteSeparation();
        _view.WriteLine($"Seleccione una habilidad para que {actorName} use");
        _view.WriteLine("1-Cancelar");
    }

    public void ShowInsufficientMP()
    {
        _view.WriteLine("MP insuficiente para usar esta habilidad");
    }

    public void ShowNoValidTargets(string actorName)
    {
        _view.WriteSeparation();
        _view.WriteLine($"Seleccione un objetivo para {actorName}");
        _view.WriteLine("1-Cancelar");
    }
}
