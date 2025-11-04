namespace Shin_Megami_Tensei_View.ConsoleLib;

public interface IMenuRenderer<T>
{
    void Render(List<T> options, object context = null);
}