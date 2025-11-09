using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Units;
using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei_View.ConsoleLib;

namespace Shin_Megami_Tensei.Services;

public class TargetSelector
{
    private readonly View _view;
    private readonly TargetSelectionView _targetSelectionView;

    public TargetSelector(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _targetSelectionView = new TargetSelectionView(view);
    }

    public Unit SelectCombatTarget(Unit actor, GameState gameState)
    {
        while (true)
        {
            var targets = gameState.GetOpponentAliveUnits();
            _targetSelectionView.ShowTargetMenu(actor.Name, targets);

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
        _targetSelectionView.ShowSummonTargets(targets);

        var choice = _view.ReadLine();
        return ParseMonsterChoice(choice, targets);
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
