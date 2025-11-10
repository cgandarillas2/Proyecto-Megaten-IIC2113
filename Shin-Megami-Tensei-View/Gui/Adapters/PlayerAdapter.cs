using Shin_Megami_Tensei_GUI;
using Shin_Megami_Tensei_Model.Game;

namespace Shin_Megami_Tensei_View.Gui.Adapters;

/// <summary>
/// Adapter que convierte un Team del modelo a IPlayer para la GUI.
/// </summary>
public class PlayerAdapter : IPlayer
{
    private readonly Team _team;

    public PlayerAdapter(Team team)
    {
        _team = team ?? throw new ArgumentNullException(nameof(team));
    }

    public IUnit?[] UnitsInBoard
    {
        get
        {
            var board = new IUnit?[4];
            for (int i = 0; i < 4; i++)
            {
                var unit = _team.ActiveBoard.GetUnitAt(i);
                board[i] = unit.IsEmpty() ? null : new UnitAdapter(unit);
            }
            return board;
        }
    }

    public IEnumerable<IUnit> UnitsInReserve
    {
        get
        {
            return _team.Reserve.Select(unit => new UnitAdapter(unit));
        }
    }
}
