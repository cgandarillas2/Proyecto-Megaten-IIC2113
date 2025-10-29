using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Combat;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Units;
using Shin_Megami_Tensei_View;

namespace Shin_Megami_Tensei;

public class CombatController
{
private readonly View _view;
    private readonly DamageCalculator _damageCalculator;
    private readonly AttackAction _attackAction;
    private readonly ShootAction _shootAction;
    private readonly SurrenderAction _surrenderAction;

    public CombatController(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _damageCalculator = new DamageCalculator();
        _attackAction = new AttackAction(_damageCalculator);
        _shootAction = new ShootAction(_damageCalculator);
        _surrenderAction = new SurrenderAction();
    }
    
    public void InitialRoundHeaderMessage(GameState gameState)
    {
        DisplayRoundHeader(gameState);
    }

    public bool ExecuteRound(GameState gameState)
    {
        DisplayBoardState(gameState);
        DisplayTurnState(gameState);
        DisplayActionOrder(gameState);
        
        var actingUnit = gameState.GetCurrentActingUnit();
        return ExecuteTurnForUnit(actingUnit, gameState);
    }

    private bool ExecuteTurnForUnit(Unit actor, GameState gameState)
    {
        var action = SelectAction(actor, gameState);
        
        if (action == null)
        {
            return false;
        }

        if (action is SurrenderAction)
        {
            ExecuteSurrender(actor, gameState);
            return true;
        }

        return ExecuteCombatAction(action, actor, gameState);
    }

    private bool ExecuteCombatAction(IAction action, Unit actor, GameState gameState)
    {
        var target = SelectTarget(actor, gameState);
    
        if (target == null)
        {
            return ExecuteTurnForUnit(actor, gameState);
        }

        var turnsBefore = CopyTurnState(gameState.CurrentTurnState);
    
        var result = ExecuteAttackAction(action, actor, target, gameState);
        gameState.ApplyTurnConsumption(result.TurnConsumption);
    
        var turnsAfter = gameState.CurrentTurnState;
        DisplayTurnConsumption(result.TurnConsumption, turnsBefore, turnsAfter);
    
        gameState.AdvanceActionQueue();
        CheckForDeaths(gameState);
    
        return true;
    }

    private TurnState CopyTurnState(TurnState original)
    {
        var copy = new TurnState(original.FullTurns);
        copy.AddBlinkingTurns(original.BlinkingTurns);
        return copy;
    }

    private void DisplayRoundHeader(GameState gameState)
    {
        var samurai = gameState.CurrentPlayer.ActiveBoard.GetUnitAt(0);
        _view.WriteSeparation();
        _view.WriteLine($"Ronda de {samurai.Name} ({gameState.CurrentPlayer.PlayerName})");
    }

    private void DisplayBoardState(GameState gameState)
    {
        _view.WriteSeparation();
        DisplayTeamUnits("Equipo de", gameState.Player1);
        DisplayTeamUnits("Equipo de", gameState.Player2);
    }

    private void DisplayTurnState(GameState gameState)
    {
        _view.WriteSeparation();
        _view.WriteLine($"Full Turns: {gameState.CurrentTurnState.FullTurns}");
        _view.WriteLine($"Blinking Turns: {gameState.CurrentTurnState.BlinkingTurns}");
    }

    private void DisplayActionOrder(GameState gameState)
    {
        _view.WriteSeparation();
        _view.WriteLine("Orden:");
        
        var units = gameState.ActionQueue.GetOrderedUnits();
        for (int i = 0; i < units.Count; i++)
        {
            _view.WriteLine($"{i + 1}-{units[i].Name}");
        }
    }

    private void DisplayTeamUnits(string prefix, Team team)
    {
        var samurai = team.ActiveBoard.GetUnitAt(0);
        _view.WriteLine($"{prefix} {samurai.Name} ({team.PlayerName})");
        
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
                DisplayUnitStats(position, unit);
            }
        }
    }

    private void DisplayUnitStats(string position, Unit unit)
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

    private IAction SelectAction(Unit actor, GameState gameState)
    {
        while (true)
        {
            DisplayActionMenu(actor);
            var choice = ReadActionChoice();
            var action = ParseActionChoice(choice, actor);

            if (action != null)
            {
                return action;
            }
        }
    }

    private void DisplayActionMenu(Unit actor)
    {
        _view.WriteSeparation();
        _view.WriteLine($"Seleccione una acción para {actor.Name}");
        
        if (actor is Samurai)
        {
            DisplaySamuraiActionMenu();
        }
        else
        {
            DisplayMonsterActionMenu();
        }
    }

    private void DisplaySamuraiActionMenu()
    {
        _view.WriteLine("1: Atacar");
        _view.WriteLine("2: Disparar");
        _view.WriteLine("3: Usar Habilidad");
        _view.WriteLine("4: Invocar");
        _view.WriteLine("5: Pasar Turno");
        _view.WriteLine("6: Rendirse");
    }

    private void DisplayMonsterActionMenu()
    {
        _view.WriteLine("1: Atacar");
        _view.WriteLine("2: Usar Habilidad");
        _view.WriteLine("3: Invocar");
        _view.WriteLine("4: Pasar Turno");
    }

    private string ReadActionChoice()
    {
        return _view.ReadLine();
    }

    private IAction ParseActionChoice(string choice, Unit actor)
    {
        if (actor is Samurai)
        {
            return ParseSamuraiAction(choice);
        }
        
        return ParseMonsterAction(choice);
    }

    private IAction? ParseSamuraiAction(string choice)
    {
        return choice switch
        {
            "1" => _attackAction,
            "2" => _shootAction,
            "6" => _surrenderAction,
            _ => null
        };
    }

    private IAction? ParseMonsterAction(string choice)
    {
        return choice switch
        {
            "1" => _attackAction,
            _ => null
        };
    }

    private void ExecuteSurrender(Unit actor, GameState gameState)
    {
        var result = _surrenderAction.Execute(actor, null, gameState);
        _view.WriteSeparation();
        _view.WriteLine($"{actor.Name} ({gameState.CurrentPlayer.PlayerName}) se rinde");
        /*_view.WriteSeparation();*/
        /*_view.WriteLine($"Ganador: {result.WinnerName}");*/
    }

    private void ExecuteUseSkill(Unit actor, GameState gameState)
    {
        
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

    private void DisplaySkillMenu(Unit actor)
    {
        _view.WriteSeparation();
        _view.WriteLine($"Seleccione una habilidad para que {actor.Name} use");
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

    private ActionResult ExecuteAttackAction(IAction action, Unit actor, Unit target, GameState gameState)
    {
        var initialHp = target.CurrentStats.CurrentHP;
        var result = action.Execute(actor, target, gameState);
        var finalHp = target.CurrentStats.CurrentHP;
        int damageInflicted = result.Damage;
        var damage = initialHp - finalHp;

        DisplayAttackResult(action, actor, target, damageInflicted);
        
        return result;
    }

    private void DisplayAttackResult(IAction action, Unit actor, Unit target, int damage)
    {
        _view.WriteSeparation();
        
        var verb = GetAttackVerb(action);
        _view.WriteLine($"{actor.Name} {verb} {target.Name}");
        _view.WriteLine($"{target.Name} recibe {damage} de daño");
        
        var hp = target.CurrentStats.CurrentHP;
        var maxHp = target.CurrentStats.MaxHP;
        _view.WriteLine($"{target.Name} termina con HP:{hp}/{maxHp}");
    }

    private void DisplayTurnConsumption(TurnConsumption consumption, TurnState stateBefore, TurnState stateAfter)
    {
        if (consumption.ConsumeAll)
        {
            _view.WriteSeparation();
            _view.WriteLine("Se han consumido todos los turnos");
            return;
        }

        var fullConsumed = CalculateFullTurnsConsumed(stateBefore, stateAfter, consumption);
        var blinkingConsumed = CalculateBlinkingTurnsConsumed(stateBefore, stateAfter, consumption);

        _view.WriteSeparation();
        _view.WriteLine($"Se han consumido {fullConsumed} Full Turn(s) y {blinkingConsumed} Blinking Turn(s)");
        _view.WriteLine($"Se han obtenido {consumption.BlinkingTurnsToGain} Blinking Turn(s)");
    }
    
    private int CalculateFullTurnsConsumed(TurnState before, TurnState after, TurnConsumption consumption)
    {
        var fullDiff = before.FullTurns - after.FullTurns;
    
        if (consumption.BlinkingTurnsToGain > 0 && before.BlinkingTurns == 0)
        {
            return fullDiff;
        }
    
        return fullDiff;
    }

    private int CalculateBlinkingTurnsConsumed(TurnState before, TurnState after, TurnConsumption consumption)
    {
        var blinkingBefore = before.BlinkingTurns;
        var blinkingAfter = after.BlinkingTurns;
        var blinkingGained = consumption.BlinkingTurnsToGain;
    
        return (blinkingBefore + blinkingGained) - blinkingAfter;
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