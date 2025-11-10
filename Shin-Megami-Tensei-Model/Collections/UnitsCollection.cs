using System.Collections;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Collections;

/// <summary>
/// Encapsulates a collection of units following the "Encapsulate Collections" principle.
/// Provides controlled access to units without exposing the internal list.
/// </summary>
public class UnitsCollection : IEnumerable<Unit>
{
    private readonly List<Unit> _units;

    /// <summary>
    /// Marks the first target selected for Multi-target skills before reordering.
    /// This ensures buffs (Charge/Concentrate) are applied to the correct target.
    /// </summary>
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

    /// <summary>
    /// Sets the first selected target for buff application in Multi-target skills.
    /// </summary>
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

    public void Clear()
    {
        _units.Clear();
    }

    public bool Contains(Unit unit)
    {
        return _units.Contains(unit);
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

    public UnitsCollection Where(Func<Unit, bool> predicate)
    {
        return new UnitsCollection(_units.Where(predicate));
    }

    public bool Any(Func<Unit, bool> predicate)
    {
        return _units.Any(predicate);
    }

    public bool All(Func<Unit, bool> predicate)
    {
        return _units.All(predicate);
    }

    /// <summary>
    /// Returns a defensive copy of the internal list.
    /// Use this only when absolutely necessary (e.g., for compatibility with legacy code).
    /// </summary>
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

    public static UnitsCollection Empty()
    {
        return new UnitsCollection();
    }
}