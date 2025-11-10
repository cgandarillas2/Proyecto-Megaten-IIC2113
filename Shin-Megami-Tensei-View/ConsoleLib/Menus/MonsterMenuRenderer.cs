using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_View.ConsoleLib;

public class MonsterMenuRenderer: IMenuRenderer<Monster>
{
    private readonly View _view;

    public MonsterMenuRenderer(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    public void Render(IEnumerable<Monster> monsters, object context = null)
    {
        var monsterList = monsters.ToList();
        _view.WriteSeparation();
        _view.WriteLine("Seleccione un monstruo para invocar");

        for (int i = 0; i < monsterList.Count; i++)
        {
            var monster = monsterList[i];
            var hp = $"HP:{monster.CurrentStats.CurrentHP}/{monster.CurrentStats.MaxHP}";
            var mp = $"MP:{monster.CurrentStats.CurrentMP}/{monster.CurrentStats.MaxMP}";
            _view.WriteLine($"{i + 1}-{monster.Name} {hp} {mp}");
        }

        _view.WriteLine($"{monsterList.Count + 1}-Cancelar");
    }
}