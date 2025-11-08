using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Game;

public class ActionQueue
{
    private readonly List<Unit> _queue;

    public ActionQueue(List<Unit> units)
    {
        _queue = SortBySpeed(units);
    }

    public Unit GetNext()
    {
        if (_queue.Count == 0)
        {
            return null;
        }

        return _queue[0];
    }

    public void MoveToEnd(Unit unit)
    {
        _queue.Remove(unit);
        _queue.Add(unit);
    }

    public List<Unit> GetOrderedUnits()
    {
        return new List<Unit>(_queue);
    }

    public void Rebuild(List<Unit> units)
    {
        _queue.Clear();
        _queue.AddRange(SortBySpeed(units));
    }

    public void PopDeadUnits()
    {
        _queue.RemoveAll(u => u == null || !u.IsAlive() || u.IsEmpty());
    }

    public void SwapUnit(Monster monster, int position)
    {
        if (monster == null) throw new ArgumentNullException(nameof(monster));
        if (position < 0 || position >= _queue.Count)
            throw new ArgumentOutOfRangeException(nameof(position), $"Posición inválida: {position}.");

        /*if (!monster.IsAlive() || monster.IsEmpty())
            throw new InvalidOperationException("No se puede ingresar un monstruo muerto o vacío.");*/

        if (_queue[position] is not Monster)
            throw new InvalidOperationException($"La unidad en la posición {position} no es un Monster.");

        /*int existingIndex = _queue.IndexOf(monster);*/
        /*if (existingIndex >= 0 && existingIndex != position)
            _queue.RemoveAt(existingIndex);*/

        _queue[position] = monster;
    }

    public int FindMonsterPosition(Unit monster)
    {
        return _queue.IndexOf(monster);
    }

    public void AddToEnd(Unit unit)
    {
        _queue.Add(unit);
    }


private static List<Unit> SortBySpeed(List<Unit> units)
    {
        var aliveUnits = new List<Unit>();
        for (int i = 0; i < units.Count; i++)
        {
            Unit unit = units[i];
            if (unit.IsAlive() && !unit.IsEmpty())
            {
                aliveUnits.Add(unit);
            }
        }

        for (int i = 0; i < aliveUnits.Count - 1; i++)
        {
            for (int j = i + 1; j < aliveUnits.Count; j++)
            {
                Unit unitI = aliveUnits[i];
                Unit unitJ = aliveUnits[j];

                int speedI = unitI.CurrentStats.Spd;
                int speedJ = unitJ.CurrentStats.Spd;

                bool shouldSwap = false;

                if (speedJ > speedI)
                {
                    shouldSwap = true;
                }
                else if (speedJ == speedI)
                {
                    int indexI = units.IndexOf(unitI);
                    int indexJ = units.IndexOf(unitJ);
                    if (indexJ < indexI)
                    {
                        shouldSwap = true;
                    }
                }

                if (shouldSwap)
                {
                    aliveUnits[i] = unitJ;
                    aliveUnits[j] = unitI;
                }
            }
        }

        return aliveUnits;
    }
}