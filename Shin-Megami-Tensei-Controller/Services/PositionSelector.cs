using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei_View.ConsoleLib;

namespace Shin_Megami_Tensei.Services;

public class PositionSelector
{
    private readonly View _view;
    private readonly PositionSelectionView _positionSelectionView;

    public PositionSelector(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _positionSelectionView = new PositionSelectionView(view);
    }

    public int SelectPosition(GameState gameState)
    {
        while (true)
        {
            var board = gameState.CurrentPlayer.ActiveBoard;
            _positionSelectionView.ShowPositionMenu(board);

            var choice = _view.ReadLine();

            if (!int.TryParse(choice, out int selection))
            {
                continue;
            }

            if (selection == 4)
            {
                return -1;
            }

            if (selection >= 1 && selection <= 3)
            {
                return selection;
            }
        }
    }
}
