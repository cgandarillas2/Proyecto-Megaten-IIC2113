using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_View.DisplayModels;

namespace Shin_Megami_Tensei_View.ConsoleLib;


public class ActionMenuView
{
    private readonly View _view;

    public ActionMenuView(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    public void ShowActionMenu(string actorName, StringCollection actions)
    {
        _view.WriteSeparation();
        _view.WriteLine($"Seleccione una acción para {actorName}");

        for (int i = 0; i < actions.Count; i++)
        {
            _view.WriteLine($"{i + 1}: {actions[i]}");
        }
    }
    
}
