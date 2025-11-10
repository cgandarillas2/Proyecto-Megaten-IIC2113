namespace Shin_Megami_Tensei_View.ConsoleLib;

public interface IMenuSelector<T>
{
    // Interface for displaying different types of menus
    
    T SelectFrom(IEnumerable<T> options, object context = null);
}