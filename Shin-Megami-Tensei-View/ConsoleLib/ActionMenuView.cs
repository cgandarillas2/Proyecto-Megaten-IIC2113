using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_View.ConsoleLib;

public class ActionMenuView
{
    private readonly View _view;

    public ActionMenuView(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    public void ShowActionMenu(Unit actor)
    {
        _view.WriteSeparation();
        _view.WriteLine($"Seleccione una acción para {actor.Name}");

        if (actor is Samurai)
        {
            ShowSamuraiActions();
        }
        else
        {
            ShowMonsterActions();
        }
    }

    private void ShowSamuraiActions()
    {
        _view.WriteLine("1: Atacar");
        _view.WriteLine("2: Disparar");
        _view.WriteLine("3: Usar Habilidad");
        _view.WriteLine("4: Invocar");
        _view.WriteLine("5: Pasar Turno");
        _view.WriteLine("6: Rendirse");
    }

    private void ShowMonsterActions()
    {
        _view.WriteLine("1: Atacar");
        _view.WriteLine("2: Usar Habilidad");
        _view.WriteLine("3: Invocar");
        _view.WriteLine("4: Pasar Turno");
    }  
}