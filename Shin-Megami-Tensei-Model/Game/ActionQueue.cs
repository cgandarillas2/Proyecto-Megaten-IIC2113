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

    private static List<Unit> SortBySpeed(List<Unit> units)
    {
        var aliveUnits = units.Where(u => u.IsAlive() && !u.IsEmpty()).ToList();
            
        return aliveUnits
            .OrderByDescending(u => u.CurrentStats.Spd)
            .ThenBy(u => units.IndexOf(u))
            .ToList();
    }
}