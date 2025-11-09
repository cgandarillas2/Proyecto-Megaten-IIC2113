using Shin_Megami_Tensei.Exceptions;
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

            if (IsCancelChoice(choice, targets))
            {
                throw new OperationCancelledException("Selección de objetivo cancelada");
            }

            try
            {
                return ParseTargetChoice(choice, targets);
            }
            catch (InvalidSelectionException)
            {
                // Continue loop to ask for valid input
            }
        }
    }

    public Monster SelectSummonTarget(GameState gameState)
    {
        while (true)
        {
            var targets = gameState.CurrentPlayer.GetAliveReserveMonsters();
            _targetSelectionView.ShowSummonTargets(targets);

            var choice = _view.ReadLine();

            try
            {
                return ParseMonsterChoice(choice, targets);
            }
            catch (InvalidSelectionException)
            {
                // Continue loop to ask for valid input
            }
        }
    }

    private Unit ParseTargetChoice(string choice, UnitsCollection targets)
    {
        if (!int.TryParse(choice, out int selection))
        {
            throw new InvalidSelectionException("La entrada debe ser un número");
        }

        if (selection < 1 || selection > targets.Count)
        {
            throw new InvalidSelectionException($"La selección debe estar entre 1 y {targets.Count}");
        }

        return targets[selection - 1];
    }

    private Monster ParseMonsterChoice(string choice, UnitsCollection targets)
    {
        if (!int.TryParse(choice, out int selection))
        {
            throw new InvalidSelectionException("La entrada debe ser un número");
        }

        if (selection == targets.Count + 1)
        {
            throw new OperationCancelledException("Selección de monstruo cancelada");
        }

        if (selection < 1 || selection > targets.Count)
        {
            throw new InvalidSelectionException($"La selección debe estar entre 1 y {targets.Count}");
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
