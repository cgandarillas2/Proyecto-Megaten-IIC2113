using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_View.ConsoleLib;

public class BoardView
{
    private readonly View _view;

    public BoardView(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    public void ShowBoardState(Team player1, Team player2)
    {
        _view.WriteSeparation();
        ShowTeam(player1);
        ShowTeam(player2);
    }

    public void ShowTurnState(int fullTurns, int blinkingTurns)
    {
        _view.WriteSeparation();
        _view.WriteLine($"Full Turns: {fullTurns}");
        _view.WriteLine($"Blinking Turns: {blinkingTurns}");
    }

    public void ShowActionOrder(List<Unit> units)
    {
        _view.WriteSeparation();
        _view.WriteLine("Orden:");

        for (int i = 0; i < units.Count; i++)
        {
            _view.WriteLine($"{i + 1}-{units[i].Name}");
        }
    }

    private void ShowTeam(Team team)
    {
        var samurai = team.ActiveBoard.GetUnitAt(0);
        _view.WriteLine($"Equipo de {samurai.Name} ({team.PlayerName})");

        var units = team.ActiveBoard.GetAllUnits();
        for (int i = 0; i < units.Count; i++)
        {
            var unit = units[i];
            var position = GetPositionLabel(i);

            if (unit.IsEmpty())
            {
                _view.WriteLine($"{position}-");
            }
            else
            {
                ShowUnitStats(position, unit);
            }
        }
    }

    private void ShowUnitStats(string position, Unit unit)
    {
        var hp = unit.CurrentStats.CurrentHP;
        var maxHp = unit.CurrentStats.MaxHP;
        var mp = unit.CurrentStats.CurrentMP;
        var maxMp = unit.CurrentStats.MaxMP;

        _view.WriteLine($"{position}-{unit.Name} HP:{hp}/{maxHp} MP:{mp}/{maxMp}");
    }

    private string GetPositionLabel(int index)
    {
        return index switch
        {
            0 => "A",
            1 => "B",
            2 => "C",
            3 => "D",
            _ => throw new ArgumentException("Invalid position")
        };
    }
}