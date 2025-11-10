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

    public IEnumerator<int> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}