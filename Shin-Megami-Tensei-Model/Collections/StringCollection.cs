using System.Collections;

namespace Shin_Megami_Tensei_Model.Collections;

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


    public string this[int index] => _strings[index];

    public void Add(string str)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }
        _strings.Add(str);
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
