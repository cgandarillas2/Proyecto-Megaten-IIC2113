namespace Shin_Megami_Tensei_View.ConsoleLib;

/// <summary>
/// Implementación genérica de selección de menú.
/// Maneja el loop de selección, validación y cancelación.
/// </summary>
public class MenuSelector<T>: IMenuSelector<T>
{
    private readonly View _view;
    private readonly IMenuRenderer<T> _renderer;

    public MenuSelector(View view, IMenuRenderer<T> renderer)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
    }

    public T SelectFrom(IEnumerable<T> options, object context = null)
    {
        if (options == null)
        {
            return default(T);
        }

        var optionsList = options.ToList();

        if (optionsList.Count == 0)
        {
            return default(T);
        }

        while (true)
        {
            _renderer.Render(optionsList, context);
            var input = _view.ReadLine();

            if (TryParseSelection(input, optionsList.Count, out int selectedIndex))
            {
                return optionsList[selectedIndex];
            }

            if (IsCancelSelection(input, optionsList.Count))
            {
                return default(T);
            }
        }
    }

    private bool TryParseSelection(string input, int optionsCount, out int index)
    {
        index = -1;
        
        if (!int.TryParse(input, out int selection))
        {
            return false;
        }

        if (selection < 1 || selection > optionsCount)
        {
            return false;
        }

        index = selection - 1;
        return true;
    }

    private bool IsCancelSelection(string input, int optionsCount)
    {
        if (!int.TryParse(input, out int selection))
        {
            return false;
        }

        return selection == optionsCount + 1;
    } 
}