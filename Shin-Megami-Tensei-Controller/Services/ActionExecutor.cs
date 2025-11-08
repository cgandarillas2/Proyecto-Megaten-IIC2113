using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Skills.Special;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;
using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei_View.ConsoleLib;

namespace Shin_Megami_Tensei.Services;

public class ActionExecutor
{
    private readonly View _view;
    private readonly CombatView _combatView;
    private readonly SkillResultView _skillResultView;
    private readonly SkillController _skillController;
    private readonly TargetSelector _targetSelector;
    private readonly PositionSelector _positionSelector;
    private readonly SummonCoordinator _summonCoordinator;
    private readonly PassTurnAction _passTurnAction;
    private readonly SummonAction _summonAction;
    private readonly SurrenderAction _surrenderAction;

    public ActionExecutor(
        View view,
        SkillController skillController,
        TargetSelector targetSelector,
        PositionSelector positionSelector,
        SummonCoordinator summonCoordinator)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _skillController = skillController ?? throw new ArgumentNullException(nameof(skillController));
        _targetSelector = targetSelector ?? throw new ArgumentNullException(nameof(targetSelector));
        _positionSelector = positionSelector ?? throw new ArgumentNullException(nameof(positionSelector));
        _summonCoordinator = summonCoordinator ?? throw new ArgumentNullException(nameof(summonCoordinator));

        _combatView = new CombatView(view);
        _skillResultView = new SkillResultView(view);
        _passTurnAction = new PassTurnAction();
        _summonAction = new SummonAction();
        _surrenderAction = new SurrenderAction();
    }

    public bool ExecuteSurrender(Unit actor, GameState gameState)
    {
        _surrenderAction.Execute(actor, null, gameState);
        _combatView.ShowSurrender(actor.Name, gameState.CurrentPlayer.PlayerName);
        return true;
    }

    public bool ExecutePassTurn(Unit actor, GameState gameState)
    {
        var result = _passTurnAction.Execute(actor, null, gameState);
        var consumptionResult = gameState.ApplyTurnConsumption(result.TurnConsumption);
        _combatView.ShowTurnConsumption(consumptionResult);
        gameState.AdvanceActionQueue();
        return true;
    }

    public bool ExecuteSummon(Unit actor, GameState gameState)
    {
        gameState.CurrentPlayer.ReorderReserveFromSelectionFile();
        var target = _targetSelector.SelectSummonTarget(gameState);
        if (target == null)
        {
            return false;
        }

        int position;
        if (actor is Samurai)
        {
            position = _positionSelector.SelectPosition(gameState);
            if (position == -1)
            {
                return false;
            }
        }
        else
        {
            position = _summonCoordinator.FindMonsterPosition(actor, gameState);
        }

        _summonCoordinator.AddToActionQueue(target, position, gameState, actor);
        _summonCoordinator.PerformSummon(target, position, gameState);

        _combatView.ShowSummonSuccess(target.Name);

        var result = _summonAction.Execute(actor, target, gameState);
        var consumptionResult = gameState.ApplyTurnConsumption(result.TurnConsumption);
        _combatView.ShowTurnConsumption(consumptionResult);

        gameState.AdvanceActionQueue();

        return true;
    }

    public bool ExecuteCombatAction(IAction action, Unit actor, GameState gameState)
    {
        var target = _targetSelector.SelectCombatTarget(actor, gameState);

        if (target == null)
        {
            return false;
        }

        var result = action.Execute(actor, target, gameState);

        _view.WriteSeparation();
        _combatView.ShowAttackMessage(actor.Name, target.Name, GetAttackVerb(action));
        _combatView.ShowAffinityMessage(target.Name, result.AffinityResult, actor.Name);

        if (result.AffinityResult == Affinity.Repel)
        {
            _combatView.ShowRepelMessage(target.Name, result.Damage, actor.Name);
            _combatView.ShowFinalHP(actor.Name, actor.CurrentStats.CurrentHP, actor.CurrentStats.MaxHP);
        }
        else if (result.AffinityResult == Affinity.Drain)
        {
            _combatView.ShowDrainMessage(target.Name, result.Damage);
            _combatView.ShowFinalHP(target.Name, target.CurrentStats.CurrentHP, target.CurrentStats.MaxHP);
        }
        else
        {
            if (result.AffinityResult != Affinity.Null && result.Damage > 0)
            {
                _combatView.ShowDamageMessage(target.Name, result.Damage);
            }
            _combatView.ShowFinalHP(target.Name, target.CurrentStats.CurrentHP, target.CurrentStats.MaxHP);
        }

        var consumptionResult = gameState.ApplyTurnConsumption(result.TurnConsumption);
        _combatView.ShowTurnConsumption(consumptionResult);

        gameState.AdvanceActionQueue();
        CheckForDeaths(gameState);

        return true;
    }

    public bool ExecuteSkill(UseSkillAction skillAction, Unit actor, GameState gameState)
    {
        if (skillAction.GetSkill() is InvitationSkill)
        {
            return ExecuteInvitationSkill(skillAction, actor, gameState);
        }

        if (skillAction.GetSkill() is SabbatmaSkill)
        {
            return ExecuteSabbatmaSkill(skillAction, actor, gameState);
        }

        gameState.CurrentPlayer.ReorderReserveFromSelectionFile();
        var targets = _skillController.SelectTargets(skillAction, actor, gameState);

        if (targets == null)
        {
            return false;
        }

        var skillResult = skillAction.ExecuteAndGetResult(actor, targets, gameState);
        _view.WriteSeparation();
        _skillResultView.Present(actor, skillResult);

        var consumptionResult = gameState.ApplyTurnConsumption(skillResult.TurnConsumption);
        _combatView.ShowTurnConsumption(consumptionResult);

        gameState.AdvanceActionQueue();
        CheckForDeaths(gameState);

        return true;
    }

    private bool ExecuteInvitationSkill(UseSkillAction skillAction, Unit actor, GameState gameState)
    {
        gameState.CurrentPlayer.ReorderReserveFromSelectionFile();
        var targets = gameState.CurrentPlayer.GetAllReserveMonsters();

        if (targets == null)
        {
            return false;
        }

        DisplaySummonTargets(targets);
        var choice = _view.ReadLine();
        var target = ParseMonsterChoice(choice, targets);

        if (target == null)
        {
            return false;
        }

        var position = _positionSelector.SelectPosition(gameState);
        if (position == -1)
        {
            return false;
        }

        _summonCoordinator.AddToActionQueue(target, position, gameState, actor);
        _summonCoordinator.PerformSummon(target, position, gameState);

        _combatView.ShowSummonSuccess(target.Name);

        var skillResult = skillAction.ExecuteAndGetResult(actor, new List<Unit> { target }, gameState);

        if (skillResult.Effects[0].WasRevived)
        {
            _skillResultView.Present(actor, skillResult);
        }

        var consumptionResult = gameState.ApplyTurnConsumption(skillResult.TurnConsumption);
        _combatView.ShowTurnConsumption(consumptionResult);

        gameState.AdvanceActionQueue();
        CheckForDeaths(gameState);

        return true;
    }

    private void DisplaySummonTargets(List<Monster> targets)
    {
        _view.WriteSeparation();
        _view.WriteLine($"Seleccione un monstruo para invocar");

        for (int i = 0; i < targets.Count; i++)
        {
            var target = targets[i];
            DisplayTargetOption(i + 1, target);
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

    private Monster ParseMonsterChoice(string choice, List<Monster> targets)
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

    private bool ExecuteSabbatmaSkill(UseSkillAction skillAction, Unit actor, GameState gameState)
    {
        var target = _targetSelector.SelectSummonTarget(gameState);
        if (target == null)
        {
            return false;
        }

        int position = _positionSelector.SelectPosition(gameState);
        if (position == -1)
        {
            return false;
        }

        _summonCoordinator.AddToActionQueue(target, position, gameState, actor);
        _summonCoordinator.PerformSummon(target, position, gameState);

        _combatView.ShowSummonSuccess(target.Name);

        var result = skillAction.Execute(actor, target, gameState);

        var consumptionResult = gameState.ApplyTurnConsumption(result.TurnConsumption);
        _combatView.ShowTurnConsumption(consumptionResult);

        gameState.AdvanceActionQueue();

        return true;
    }

    private string GetAttackVerb(IAction action)
    {
        return action switch
        {
            AttackAction => "ataca a",
            ShootAction => "dispara a",
            _ => "ataca a"
        };
    }

    private void CheckForDeaths(GameState gameState)
    {
        gameState.CurrentPlayer.RemoveDeadMonstersFromBoard();
        gameState.GetOpponent().RemoveDeadMonstersFromBoard();
        gameState.ActionQueue.PopDeadUnits();
    }
}
