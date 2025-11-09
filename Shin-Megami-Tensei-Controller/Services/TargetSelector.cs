using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Units;
using Shin_Megami_Tensei_View;

namespace Shin_Megami_Tensei.Services;

public class TargetSelector
{
    private readonly View _view;

    public TargetSelector(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    public Unit SelectCombatTarget(Unit actor, GameState gameState)
    {
        while (true)
        {
            var targets = gameState.GetOpponentAliveUnits();
            DisplayTargetMenu(actor, targets);

            var choice = _view.ReadLine();
            var target = ParseTargetChoice(choice, targets);

            if (target != null)
            {
                return target;
            }

            if (IsCancelChoice(choice, targets))
            {
                return null;
            }
        }
    }

    public Monster SelectSummonTarget(GameState gameState)
    {
        var targets = gameState.CurrentPlayer.GetAliveReserveMonsters();
        DisplaySummonTargets(targets);

        var choice = _view.ReadLine();
        return ParseMonsterChoice(choice, targets);
    }

    private void DisplayTargetMenu(Unit actor, UnitsCollection targets)
    {
        _view.WriteSeparation();
        _view.WriteLine($"Seleccione un objetivo para {actor.Name}");

        for (int i = 0; i < targets.Count; i++)
        {
            DisplayTargetOption(i + 1, targets[i]);
        }

        _view.WriteLine($"{targets.Count + 1}-Cancelar");
    }

    private void DisplaySummonTargets(UnitsCollection targets)
    {
        _view.WriteSeparation();
        _view.WriteLine($"Seleccione un monstruo para invocar");

        for (int i = 0; i < targets.Count; i++)
        {
            DisplayTargetOption(i + 1, targets[i]);
        }

        _view.WriteLine($"{targets.Count + 1}-Cancelar");
    }

    private void DisplayTargetOption(int number, Unit target)
    {
        var hp = target.CurrentStats.CurrentHP;
        var maxHp = target.CurrentStats.MaxHP;
        var mp = target.CurrentStats.CurrentMP;
        var maxMp = target.CurrentStats.MaxMP;

        _view.WriteLine($"{number}-{target.Name} HP:{hp}/{maxHp} MP:{mp}/{maxMp}");
    }

    private Unit ParseTargetChoice(string choice, UnitsCollection targets)
    {
        if (!int.TryParse(choice, out int selection))
        {
            return null;
        }

        if (selection < 1 || selection > targets.Count)
        {
            return null;
        }

        return targets[selection - 1];
    }

    private Monster ParseMonsterChoice(string choice, UnitsCollection targets)
    {
        if (!int.TryParse(choice, out int selection))
        {
            return null;
        }

        if (selection < 1 || selection > targets.Count)
        {
            return null;
        }

        return targets[selection - 1] as Monster;
    }

    private bool IsCancelChoice(string choice, UnitsCollection targets)
    {
        if (!int.TryParse(choice, out int selection))
        {
            return false;
        }

        return selection == targets.Count + 1;
    }
}
