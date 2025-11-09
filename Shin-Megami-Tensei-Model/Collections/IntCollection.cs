using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Shin_Megami_Tensei_Model.Collections;

public class IntCollection : IEnumerable<int>
{
    private readonly List<int> _values;

    public IntCollection()
    {
        _values = new List<int>();
    }

    public IntCollection(IEnumerable<int> values)
    {
        _values = new List<int>(values ?? throw new ArgumentNullException(nameof(values)));
    }

    public int Count => _values.Count;
    public bool IsEmpty => _values.Count == 0;
    public int this[int index] => _values[index];

    public void Add(int value)
    {
        _values.Add(value);
    }

    public bool Contains(int value)
    {
        return _values.Contains(value);
    }

    public IntCollection Where(Func<int, bool> predicate)
    {
        var filtered = _values.Where(predicate);
        return new IntCollection(filtered);
    }

    public List<int> ToList()
    {
        return new List<int>(_values);
    }

    public IEnumerator<int> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static IntCollection Empty()
    {
        return new IntCollection();
    }
}