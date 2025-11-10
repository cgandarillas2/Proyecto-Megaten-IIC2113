using System.Collections;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Collections;


public class UnitsCollection : IEnumerable<Unit>
{
    private readonly List<Unit> _units;

    public Unit? FirstSelectedTarget { get; private set; }

    public UnitsCollection()
    {
        _units = new List<Unit>();
    }

    public UnitsCollection(IEnumerable<Unit> units)
    {
        _units = new List<Unit>(units ?? throw new ArgumentNullException(nameof(units)));
    }

    public int Count => _units.Count;

    public bool IsEmpty => _units.Count == 0;

    public Unit this[int index] => _units[index];

    public void SetFirstSelectedTarget(Unit unit)
    {
        if (unit != null && !_units.Contains(unit))
        {
            throw new ArgumentException("Unit must be in the collection");
        }
        FirstSelectedTarget = unit;
    }

    public void Add(Unit unit)
    {
        if (unit == null)
        {
            throw new ArgumentNullException(nameof(unit));
        }
        _units.Add(unit);
    }

    public void Remove(Unit unit)
    {
        _units.Remove(unit);
    }

    public void AddRange(IEnumerable<Unit> units)
    {
        if (units == null)
        {
            throw new ArgumentNullException(nameof(units));
        }
        _units.AddRange(units);
    }

    public Unit First()
    {
        if (_units.Count == 0)
        {
            throw new InvalidOperationException("Collection is empty");
        }
        return _units[0];
    }

    public Unit Last()
    {
        if (_units.Count == 0)
        {
            throw new InvalidOperationException("Collection is empty");
        }
        return _units[_units.Count - 1];
    }
    

    public bool Any(Func<Unit, bool> predicate)
    {
        return _units.Any(predicate);
    }

    public bool All(Func<Unit, bool> predicate)
    {
        return _units.All(predicate);
    }

    public List<Unit> ToList()
    {
        return new List<Unit>(_units);
    }

    public IEnumerator<Unit> GetEnumerator()
    {
        return _units.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

}