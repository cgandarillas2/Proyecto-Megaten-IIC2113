using Shin_Megami_Tensei.Exceptions;
using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Combat;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Units;
using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei_View.ConsoleLib;

namespace Shin_Megami_Tensei.Services;

public class ActionSelector
{
    private readonly View _view;
    private readonly ActionMenuView _actionMenuView;
    private readonly SkillController _skillController;
    private readonly AttackAction _attackAction;
    private readonly ShootAction _shootAction;
    private readonly PassTurnAction _passTurnAction;
    private readonly SummonAction _summonAction;
    private readonly SurrenderAction _surrenderAction;

    public ActionSelector(View view, SkillController skillController)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _skillController = skillController ?? throw new ArgumentNullException(nameof(skillController));
        _actionMenuView = new ActionMenuView(view);

        var damageCalculator = new DamageCalculator();
        _attackAction = new AttackAction(damageCalculator);
        _shootAction = new ShootAction(damageCalculator);
        _passTurnAction = new PassTurnAction();
        _summonAction = new SummonAction();
        _surrenderAction = new SurrenderAction();
    }

    public IAction SelectAction(Unit actor, GameState gameState)
    {
        while (true)
        {
            var actions = GetActionsForUnit(actor);
            _actionMenuView.ShowActionMenu(actor.Name, actions);
            var choice = _view.ReadLine().Trim();
            var action = ParseActionChoice(choice, actor, gameState);

            if (action != null)
            {
                return action;
            }
        }
    }

    private StringCollection GetActionsForUnit(Unit actor)
    {
        if (actor is Samurai)
        {
            return new StringCollection(new[]
            {
                "Atacar",
                "Disparar",
                "Usar Habilidad",
                "Invocar",
                "Pasar Turno",
                "Rendirse"
            });
        }

        return new StringCollection(new[]
        {
            "Atacar",
            "Usar Habilidad",
            "Invocar",
            "Pasar Turno"
        });
    }

    private IAction ParseActionChoice(string choice, Unit actor, GameState gameState)
    {
        if (actor is Samurai)
        {
            return ParseSamuraiAction(choice, actor, gameState);
        }

        return ParseMonsterAction(choice, actor, gameState);
    }

    private IAction ParseSamuraiAction(string choice, Unit actor, GameState gameState)
    {
        return choice switch
        {
            "1" => _attackAction,
            "2" => _shootAction,
            "3" => SelectSkillSafely(actor, gameState),
            "4" => _summonAction,
            "5" => _passTurnAction,
            "6" => _surrenderAction,
            _ => throw new ArgumentNullException(nameof(choice))
        };
    }

    private IAction ParseMonsterAction(string choice, Unit actor, GameState gameState)
    {
        return choice switch
        {
            "1" => _attackAction,
            "2" => SelectSkillSafely(actor, gameState),
            "3" => _summonAction,
            "4" => _passTurnAction,
            _ => throw new ArgumentNullException(nameof(choice))
        };
    }

    private IAction SelectSkillSafely(Unit actor, GameState gameState)
    {
        try
        {
            return _skillController.SelectSkill(actor, gameState);
        }
        catch (OperationCancelledException)
        {
            return null;
        }
        catch (NoSkillsAvailableException)
        {
            return null;
        }
        catch (NoValidTargetsException)
        {
            return null;
        }
    }
}