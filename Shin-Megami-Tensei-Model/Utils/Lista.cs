namespace Shin_Megami_Tensei_Model.Utils;

public class Lista<T>
{
    private readonly List<T> _items = new();

    public void Agregar(T item)
    {
        _items.Add(item);
    }

    public void Remover(T item)
    {
        _items.Remove(item);
    }

    public IReadOnlyList<T> Obtener()
    {
        return _items;
    }
}