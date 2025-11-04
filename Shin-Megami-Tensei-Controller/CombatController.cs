using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Combat;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Skills.Offensive;
using Shin_Megami_Tensei_Model.Skills.Special;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;
using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei_View.ConsoleLib;

namespace Shin_Megami_Tensei;

public class CombatController
{
    private readonly View _view;
    private readonly CombatView _combatView;
    private readonly BoardView _boardView;
    private readonly ActionMenuView _actionMenuView;
    private readonly SkillResultView _skillResultView;
    
    private readonly IMenuSelector<Unit> _targetSelector;
    private readonly IMenuSelector<Monster> _monsterSelector;
    private IMenuSelector<int> _positionSelector;
    
    private readonly DamageCalculator _damageCalculator;
    private readonly SkillController _skillController;
    private readonly AttackAction _attackAction;
    private readonly ShootAction _shootAction;
    private readonly PassTurnAction _passTurnAction;
    private readonly SummonAction _summonAction;
    private readonly SurrenderAction _surrenderAction;

    public CombatController(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _combatView = new CombatView(view);
        _boardView = new BoardView(view);
        _actionMenuView = new ActionMenuView(view);
        _skillResultView = new SkillResultView(view);
        
        var targetRenderer = new UnitMenuRenderer(view, "Seleccione un objetivo");
        _targetSelector = new MenuSelector<Unit>(view, targetRenderer);
        
        var monsterRenderer = new MonsterMenuRenderer(view);
        _monsterSelector = new MenuSelector<Monster>(view, monsterRenderer);

        _damageCalculator = new DamageCalculator();
        _attackAction = new AttackAction(_damageCalculator);
        _shootAction = new ShootAction(_damageCalculator);
        _skillController = new SkillController(view);
        _passTurnAction = new PassTurnAction();
        _summonAction = new SummonAction();
        _surrenderAction = new SurrenderAction();
    }
    
    public void InitialRoundHeaderMessage(GameState gameState)
    {
        var player = gameState.CurrentPlayer;
        var samurai = player.ActiveBoard.GetSamurai();
        _combatView.ShowRoundHeader(samurai.Name, player.PlayerName);
    }

    public bool ExecuteRound(GameState gameState)
    {
        DisplayGameState(gameState);
        var actingUnit = gameState.GetCurrentActingUnit();
        return ExecuteTurnForUnit(actingUnit, gameState);
    }
    
    private void DisplayGameState(GameState gameState)
    {
        _boardView.ShowBoardState(gameState.Player1, gameState.Player2);
        _boardView.ShowTurnState(
            gameState.CurrentTurnState.FullTurns,
            gameState.CurrentTurnState.BlinkingTurns
        );
        _boardView.ShowActionOrder(gameState.ActionQueue.GetOrderedUnits());
    }

    private bool ExecuteTurnForUnit(Unit actor, GameState gameState)
    {
        var action = SelectAction(actor, gameState);
        
        if (action == null) 
            return false;

        return action switch
        {
            SurrenderAction => ExecuteSurrenderAction(actor, gameState),
            SummonAction => ExecuteSummonAction(actor, gameState),
            PassTurnAction => ExecutePassTurnAction(actor, gameState),
            UseSkillAction skillAction => ExecuteUseSkillAction(skillAction, actor, gameState),
            _ => ExecuteCombatAction(action, actor, gameState)
        };
        
    }
    
    private IAction SelectAction(Unit actor, GameState gameState)
    {
        while (true)
        {
            _actionMenuView.ShowActionMenu(actor);
            var choice = ReadActionChoice();
            var action = ParseActionChoice(choice, actor, gameState);

            if (action != null)
            {
                return action;
            }
        }
    }
    
    private IAction ParseActionChoice(string choice, Unit actor, GameState gameState)
    {
        if (actor is Samurai)
        {
            return ParseSamuraiAction(choice, actor, gameState);
        }
            
        return ParseMonsterAction(choice, actor, gameState);
    }

    private IAction? ParseSamuraiAction(string choice, Unit actor, GameState gameState)
    {
        return choice switch
        {
            "1" => _attackAction,
            "2" => _shootAction,
            "3" => _skillController.SelectSkill(actor, gameState),
            "4" => _summonAction,
            "5" => _passTurnAction,
            "6" => _surrenderAction,
            _ => null
        };
    }

    private IAction? ParseMonsterAction(string choice, Unit actor, GameState gameState)
    {
        return choice switch
        {
            "1" => _attackAction,
            "2" => _skillController.SelectSkill(actor, gameState),
            "3" => _summonAction,
            "4" => _passTurnAction,
            _ => null
        };
    }
    
    private bool ExecuteSurrenderAction(Unit actor, GameState gameState)
    {
        _surrenderAction.Execute(actor, null, gameState);
        _combatView.ShowSurrender(actor.Name, gameState.CurrentPlayer.PlayerName);
        return true;
    }
    
    private bool ExecutePassTurnAction(Unit actor, GameState gameState)
    {
        var result = _passTurnAction.Execute(actor, null, gameState);
        var consumptionResult = gameState.ApplyTurnConsumption(result.TurnConsumption);
        _combatView.ShowTurnConsumption(consumptionResult);
        gameState.AdvanceActionQueue();
        return true;
    }

    private bool ExecuteSummonAction(Unit actor, GameState gameState)
    {
        gameState.CurrentPlayer.ReorderReserveFromSelectionFile();
        var target = SelectSummonTarget(gameState);
        if (target == null)
        {
            return ExecuteTurnForUnit(actor, gameState);
        }

        int position;
        if (actor is Samurai)
        {
            position = SelectSummonPosition(gameState);
            if (position == -1)
            {
                return ExecuteTurnForUnit(actor, gameState);
            }
        }
        else
        {
            position = FindMonsterPosition(actor, gameState);
        }
        
        if (IsEmptyPosition(position, gameState))
        {
            gameState.ActionQueue.AddToEnd(target);
        }
        else
        {
            var queuePosition = GetQuehuePositionToAdd(actor, position, gameState);
            gameState.ActionQueue.SwapUnit(target, queuePosition);
        }
        
        PerformSummon(target, position, gameState);
        
        _combatView.ShowSummonSuccess(target.Name);
        
        var result = _summonAction.Execute(actor, target, gameState);
        var consumptionResult = gameState.ApplyTurnConsumption(result.TurnConsumption);
        _combatView.ShowTurnConsumption(consumptionResult);
        
        gameState.AdvanceActionQueue();
        
        return true;
    }
    
    private bool ExecuteCombatAction(IAction action, Unit actor, GameState gameState)
    {
        var target = SelectTarget(actor, gameState);

        if (target == null)
        {
            return ExecuteTurnForUnit(actor, gameState);
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

    private bool ExecuteUseSkillAction(UseSkillAction skillAction, Unit actor, GameState gameState)
    {
        if (skillAction.GetSkill() is InvitationSkill)
        {
            return ExecuteInvitationSkill(skillAction, actor, gameState);
        }
        
        if (skillAction.GetSkill() is SabbatmaSkill)
        {
           return ExecuteSabbatmaSkill(skillAction, actor, gameState);
        }
        
        var targets = _skillController.SelectTargets(skillAction, actor, gameState);
        
        
        // CAMBIAR A TRY CATCH
        if (targets == null)
        {
            return ExecuteTurnForUnit(actor, gameState);
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
        // CAMBIAR A TRY CATCH
        if (targets == null)
        {
            return ExecuteTurnForUnit(actor, gameState);
        }
        
        DisplaySummonTargets(targets);
        
        var choice = _view.ReadLine();
        var target = ParseTargetChoiceMonster(choice, targets);
        
        if (target == null)
        {
            return ExecuteTurnForUnit(actor, gameState);
        }
        
        var position = SelectSummonPosition(gameState);
        if (position == -1)
        {
            return ExecuteTurnForUnit(actor, gameState);
        }
        
        if (IsEmptyPosition(position, gameState))
        {
            gameState.ActionQueue.AddToEnd(target);
        }
        else
        {
            var queuePosition = GetQuehuePositionToAdd(actor, position, gameState);
            gameState.ActionQueue.SwapUnit(target, queuePosition);
        }
        
        PerformSummon(target, position, gameState);
        
        _combatView.ShowSummonSuccess(target.Name);
        
        var skillResult = skillAction.ExecuteAndGetResult(actor, new List<Unit>{target}, gameState);
        
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
    
    private bool ExecuteSabbatmaSkill(UseSkillAction skillAction, Unit actor, GameState gameState)
    {
        var target = SelectSummonTarget(gameState);
        if (target == null)
        {
            return ExecuteTurnForUnit(actor, gameState);
        }

        int position;
        position = SelectSummonPosition(gameState);
        if (position == -1)
        {
            return ExecuteTurnForUnit(actor, gameState);
        }
        
        if (IsEmptyPosition(position, gameState))
        {
            gameState.ActionQueue.AddToEnd(target);
        }
        else
        {
            var queuePosition = GetQuehuePositionToAdd(actor, position, gameState);
            gameState.ActionQueue.SwapUnit(target, queuePosition);
        }
        
        PerformSummon(target, position, gameState);
        
        _combatView.ShowSummonSuccess(target.Name);
        
        var result = skillAction.Execute(actor, target, gameState);
        
        var consumptionResult = gameState.ApplyTurnConsumption(result.TurnConsumption);
        _combatView.ShowTurnConsumption(consumptionResult);
        
        gameState.AdvanceActionQueue();
        return true;
    }

    private int GetQuehuePositionToAdd(Unit actor, int position, GameState gameState)
    {
        var board = gameState.CurrentPlayer.ActiveBoard;
        var unitInPosition = board.GetUnitAt(position);
        
        return gameState.ActionQueue.FindMonsterPosition(unitInPosition);
    }

    private bool IsEmptyPosition(int position, GameState gameState)
    {
        var board = gameState.CurrentPlayer.ActiveBoard;

        if (board.IsPositionEmpty(position))
        {
            return true;
        }

        return false;
    }

    private void PerformSummon(Monster monster, int position, GameState gameState)
    {
        var board = gameState.CurrentPlayer.ActiveBoard;
        var currentUnit = board.GetUnitAt(position);

        if (!currentUnit.IsEmpty() && currentUnit is Monster existingMonster)
        {
            gameState.CurrentPlayer.AddMonsterToReserve(existingMonster);
        }
        
        gameState.CurrentPlayer.RemoveMonsterFromReserve(monster);
        
        gameState.CurrentPlayer.ReorderReserveFromSelectionFile();
        
        board.PlaceUnit(monster, position);
    }
    
    public Monster SelectSummonTarget(GameState gameState)
    {
        while (true)
        {
            var targets = _summonAction.GetTargets(gameState);
            DisplaySummonTargets(targets);
        
            var choice = _view.ReadLine();
            var target = ParseTargetChoiceMonster(choice, targets);
        
            if (target != null)
            {
                return target;
            }

            return null;
        }
        
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

    private int SelectSummonPosition(GameState gameState)
    {
        while (true)
        {
            DisplayPositionMenu(gameState);

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
    private void DisplayPositionMenu(GameState gameState)
    {
        _view.WriteSeparation();
        _view.WriteLine("Seleccione una posición para invocar");

        var board = gameState.CurrentPlayer.ActiveBoard;

        for (int i = 1; i <= 3; i++)
        {
            var unit = board.GetUnitAt(i);
            if (unit.IsEmpty())
            {
                _view.WriteLine($"{i}-Vacío (Puesto {i + 1})");
            }
            else
            {
                _view.WriteLine($"{i}-{unit.Name} HP:{unit.CurrentStats.CurrentHP}/{unit.CurrentStats.MaxHP} MP:{unit.CurrentStats.CurrentMP}/{unit.CurrentStats.MaxMP} (Puesto {i + 1})");
            }
        }

        _view.WriteLine("4-Cancelar");
    }
    
    private int FindMonsterPosition(Unit monster, GameState gameState)
    {
        var board = gameState.CurrentPlayer.ActiveBoard;
        for (int i = 1; i <= 3; i++)
        {
            if (board.GetUnitAt(i) == monster)
            {
                return i;
            }
        }
        return -1;
    }
    
    private void DisplayTargetMenu(Unit actor, List<Unit> targets)
    {
        _view.WriteSeparation();
        _view.WriteLine($"Seleccione un objetivo para {actor.Name}");
        
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
    
    private string ReadActionChoice()
    {
        return _view.ReadLine();
    }
    

    private Unit SelectTarget(Unit actor, GameState gameState)
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
    
    private Monster ParseTargetChoiceMonster(string choice, List<Monster> targets)
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
    private Unit ParseTargetChoice(string choice, List<Unit> targets)
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

    private bool IsCancelChoice(string choice, List<Unit> targets)
    {
        if (!int.TryParse(choice, out int selection))
        {
            return false;
        }

        return selection == targets.Count + 1;
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
    }
}