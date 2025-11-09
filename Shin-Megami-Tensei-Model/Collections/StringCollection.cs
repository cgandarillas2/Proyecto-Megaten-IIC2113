using System.Collections;

namespace Shin_Megami_Tensei_Model.Collections;

/// <summary>
/// Encapsulates a collection of strings following the "Encapsulate Collections" principle.
/// Provides controlled access to strings without exposing the internal list.
/// </summary>
public class StringCollection : IEnumerable<string>
{
    private readonly List<string> _strings;

    public StringCollection()
    {
        _strings = new List<string>();
    }

    public StringCollection(IEnumerable<string> strings)
    {
        _strings = new List<string>(strings ?? throw new ArgumentNullException(nameof(strings)));
    }

    public int Count => _strings.Count;

    public bool IsEmpty => _strings.Count == 0;

    public string this[int index] => _strings[index];

    public void Add(string str)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }
        _strings.Add(str);
    }

    public void Remove(string str)
    {
        _strings.Remove(str);
    }

    public void Clear()
    {
        _strings.Clear();
    }

    public bool Contains(string str)
    {
        return _strings.Contains(str);
    }

    public StringCollection Where(Func<string, bool> predicate)
    {
        return new StringCollection(_strings.Where(predicate));
    }

    public bool Any(Func<string, bool> predicate)
    {
        return _strings.Any(predicate);
    }

    /// <summary>
    /// Returns a defensive copy of the internal list.
    /// Use this only when absolutely necessary (e.g., for compatibility with legacy code).
    /// </summary>
    public List<string> ToList()
    {
        return new List<string>(_strings);
    }

    public IEnumerator<string> GetEnumerator()
    {
        return _strings.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static StringCollection Empty()
    {
        return new StringCollection();
    }
}
