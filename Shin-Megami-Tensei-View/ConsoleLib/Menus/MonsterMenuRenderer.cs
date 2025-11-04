using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_View.ConsoleLib;

public class MonsterMenuRenderer: IMenuRenderer<Monster>
{
    private readonly View _view;

    public MonsterMenuRenderer(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    public void Render(List<Monster> monsters, object context = null)
    {
        _view.WriteSeparation();
        _view.WriteLine("Seleccione un monstruo para invocar");

        for (int i = 0; i < monsters.Count; i++)
        {
            var monster = monsters[i];
            _view.WriteLine($"{i + 1}-{monster.Name} HP:{monster.CurrentStats.CurrentHP}/{monster.CurrentStats.MaxHP} MP:{monster.CurrentStats.CurrentMP}/{monster.CurrentStats.MaxMP}");
        }

        _view.WriteLine($"{monsters.Count + 1}-Cancelar");
    }
}