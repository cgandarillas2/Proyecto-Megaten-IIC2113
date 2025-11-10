using Shin_Megami_Tensei_View.DisplayModels;

namespace Shin_Megami_Tensei_View.ConsoleLib;

public class SummonPositionMenuRenderer : IMenuRenderer<int>
{
    private readonly View _view;

    public SummonPositionMenuRenderer(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    public void Render(IEnumerable<int> positions, object context = null)
    {
        _view.WriteSeparation();
        _view.WriteLine("Seleccione una posición para invocar");

        var displayModels = context as List<PositionDisplayModel>;
        if (displayModels == null || displayModels.Count == 0)
        {
            foreach (var position in positions)
            {
                _view.WriteLine($"{position}-Posición {position + 1}");
            }
        }
        else
        {
            foreach (var display in displayModels)
            {
                _view.WriteLine($"{display.Position}-{display.DisplayText}");
            }
        }

        _view.WriteLine("4-Cancelar");
    }
}
