using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Combat;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Skills.Offensive;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;
using Shin_Megami_Tensei_View;

namespace Shin_Megami_Tensei;

public class CombatController
{
    private readonly View _view;
    private readonly DamageCalculator _damageCalculator;
    private readonly SkillController _skillController;
    private readonly AttackAction _attackAction;
    private readonly ShootAction _shootAction;
    private readonly PassTurnAction _passTurnAction;
    private readonly SurrenderAction _surrenderAction;

    public CombatController(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _damageCalculator = new DamageCalculator();
        _attackAction = new AttackAction(_damageCalculator);
        _shootAction = new ShootAction(_damageCalculator);
        _skillController = new SkillController(view);
        _passTurnAction = new PassTurnAction();
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

        if (action is PassTurnAction)
        {
            return ExecutePassTurn(actor, gameState);
        }

        if (action is UseSkillAction skillAction)
        {
            return ExecuteUseSkillAction(skillAction, actor, gameState);
        }

        return ExecuteCombatAction(action, actor, gameState);
    }
    
    private bool ExecutePassTurn(Unit actor, GameState gameState)
    {
        var result = _passTurnAction.Execute(actor, null, gameState);
        var consumptionResult = gameState.ApplyTurnConsumption(result.TurnConsumption);
    
        DisplayTurnConsumption(consumptionResult);
    
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

        var result = ExecuteAttackAction(action, actor, target, gameState);
        var consumptionResult = gameState.ApplyTurnConsumption(result.TurnConsumption);
    
        DisplayTurnConsumption(consumptionResult);

        gameState.AdvanceActionQueue();
        CheckForDeaths(gameState);

        return true;
    }

    private bool ExecuteUseSkillAction(UseSkillAction skillAction, Unit actor, GameState gameState)
    {
        var targets = _skillController.SelectTargets(skillAction, actor, gameState);

        // CAMBIAR A TRY CATCH
        if (targets == null)
        {
            return ExecuteTurnForUnit(actor, gameState);
        }

        var skillResult = skillAction.ExecuteAndGetResult(actor, targets, gameState);
        DisplaySkillExecution(actor, skillResult);
    
        var consumptionResult = gameState.ApplyTurnConsumption(skillResult.TurnConsumption);
        DisplayTurnConsumption(consumptionResult);
        
        gameState.AdvanceActionQueue();
        CheckForDeaths(gameState);

        return true;
    }

    private void DisplaySkillExecution(Unit actor, SkillResult result)
    {
        // Agrupar efectos por target
        var effectsByTarget = result.Effects
            .GroupBy(e => e.TargetName)
            .ToList();

        foreach (var targetGroup in effectsByTarget)
        {
            DisplayTargetEffects(actor, targetGroup.Key, targetGroup.ToList());
        }

        foreach (var message in result.Messages)
        {
            _view.WriteLine(message);
        }
    }
    
    private void DisplayTargetEffects(Unit actor, string targetName, List<SkillEffect> effects)
    {
        if (effects.Count == 0)
        {
            return;
        }
        
        _view.WriteSeparation();

        // Tomar el último efecto para obtener HP final y verificar si murió
        var lastEffect = effects[effects.Count - 1];
    
        // Verificar si hay algún Repel o Drain (casos especiales)
        var hasRepel = effects.Any(e => e.AffinityResult == Affinity.Repel);
        var hasDrain = effects.Any(e => e.AffinityResult == Affinity.Drain);

        if (hasRepel)
        {
            DisplayRepelEffects(actor, targetName, effects);
            return;
        }

        if (hasDrain)
        {
            DisplayDrainEffects(actor, targetName, effects);
            return;
        }

        // Caso normal: mostrar cada hit individualmente
        foreach (var effect in effects)
        {
            DisplaySingleHit(actor, targetName, effect, isLastHit: false);
        }

        // Mostrar si murió (solo una vez al final)
        /*if (lastEffect.TargetDied)
        {
            _view.WriteLine($"{targetName} ha sido eliminado");
        }*/

        // Mostrar HP final (solo una vez al final)
        _view.WriteLine($"{targetName} termina con HP:{lastEffect.FinalHP}/{lastEffect.MaxHP}");
    }
    
    private void DisplaySingleHit(Unit actor, string targetName, SkillEffect effect, bool isLastHit)
    {
    
        var attackVerb = GetAttackVerbByElement(effect.Element);
        _view.WriteLine($"{actor.Name} {attackVerb} {targetName}");

        // Mostrar resultado de afinidad (solo si no es neutral)
        if (effect.AffinityResult == Affinity.Weak)
        {
            _view.WriteLine($"{targetName} es débil contra el ataque de {actor.Name}");
        }
        else if (effect.AffinityResult == Affinity.Resist)
        {
            _view.WriteLine($"{targetName} es resistente el ataque de {actor.Name}");
        }
        else if (effect.AffinityResult == Affinity.Null)
        {
            _view.WriteLine($"{targetName} bloquea el ataque de {actor.Name}");
        }

        // Mostrar daño si no es Null
        if (effect.AffinityResult != Affinity.Null && effect.DamageDealt > 0)
        {
            _view.WriteLine($"{targetName} recibe {effect.DamageDealt} de daño");
        }
    }
    
    private void DisplayRepelEffects(Unit actor, string targetName, List<SkillEffect> effects)
    {
        var lastEffect = effects[effects.Count - 1];

        foreach (var effect in effects)
        {
            var attackVerb = GetAttackVerbByElement(effect.Element);
            _view.WriteLine($"{actor.Name} {attackVerb} {targetName}");
            _view.WriteLine($"{targetName} devuelve {effect.DamageDealt} daño a {actor.Name}");
        }

        // Mostrar HP final del atacante (solo una vez al final)
        _view.WriteLine($"{actor.Name} termina con HP:{lastEffect.FinalHP}/{lastEffect.MaxHP}");
    }

    private void DisplayDrainEffects(Unit actor, string targetName, List<SkillEffect> effects)
    {
        var lastEffect = effects[effects.Count - 1];

        foreach (var effect in effects)
        {
            var attackVerb = GetAttackVerbByElement(effect.Element);
            _view.WriteLine($"{actor.Name} {attackVerb} {targetName}");
            _view.WriteLine($"{targetName} absorbe {effect.HealingDone} daño");
        }

        // Mostrar HP final del target (solo una vez al final)
        _view.WriteLine($"{targetName} termina con HP:{lastEffect.FinalHP}/{lastEffect.MaxHP}");
    }
    
    private string GetAttackVerbByElement(Element element)
    {
        return element switch
        {
            Element.Phys => "ataca a",
            Element.Gun => "dispara a",
            Element.Fire => "lanza fuego a",
            Element.Ice => "lanza hielo a",
            Element.Elec => "lanza electricidad a",
            Element.Force => "lanza viento a",
            Element.Light => "ataca con luz a",
            Element.Dark => "ataca con oscuridad a",
            Element.Almighty => "ataca a",
            _ => "ataca a"
        };
    }

    private void DisplayAffinityResultMessage(string targetName, Affinity affinity, string attackerName)
    {
        var message = affinity switch
        {
            Affinity.Weak => $"{targetName} es débil contra el ataque de {attackerName}",
            Affinity.Resist => $"{targetName} es resistente el ataque de {attackerName}",
            Affinity.Null => $"{targetName} bloquea el ataque de {attackerName}",
            _ => null
        };

        if (message != null)
        {
            _view.WriteLine(message);
        }
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
            var action = ParseActionChoice(choice, actor, gameState);

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
            "4" => _passTurnAction,
            _ => null
        };
    }

    private void ExecuteSurrender(Unit actor, GameState gameState)
    {
        var result = _surrenderAction.Execute(actor, null, gameState);
        _view.WriteSeparation();
        _view.WriteLine($"{actor.Name} ({gameState.CurrentPlayer.PlayerName}) se rinde");
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

        DisplayAttackResult(action, actor, target, damageInflicted, result.AffinityResult);
        
        return result;
    }

    private void DisplayAttackResult(IAction action, Unit actor, Unit target, int damage, Affinity affinity)
    {
        _view.WriteSeparation();
        
        var verb = GetAttackVerb(action);
        _view.WriteLine($"{actor.Name} {verb} {target.Name}");
        
        // Mostrar mensaje de afinidad antes del daño (excepto Neutral)
        if (affinity == Affinity.Weak)
        {
            _view.WriteLine($"{target.Name} es débil contra el ataque de {actor.Name}");
        }
        else if (affinity == Affinity.Resist)
        {
            _view.WriteLine($"{target.Name} es resistente el ataque de {actor.Name}");
        }
        else if (affinity == Affinity.Null)
        {
            _view.WriteLine($"{target.Name} bloquea el ataque de {actor.Name}");
        }
        else if (affinity == Affinity.Repel)
        {
            _view.WriteLine($"{target.Name} devuelve {damage} daño a {actor.Name}");
            var hp = actor.CurrentStats.CurrentHP;
            var maxHp = actor.CurrentStats.MaxHP;
            _view.WriteLine($"{actor.Name} termina con HP:{hp}/{maxHp}");
            return;
        }
        else if (affinity == Affinity.Drain)
        {
            _view.WriteLine($"{target.Name} absorbe {damage} daño");
            var hp = target.CurrentStats.CurrentHP;
            var maxHp = target.CurrentStats.MaxHP;
            _view.WriteLine($"{target.Name} termina con HP:{hp}/{maxHp}");
            return;
        }
        
        if (affinity != Affinity.Null && damage > 0)
        {
            _view.WriteLine($"{target.Name} recibe {damage} de daño");
        }
        
        var targetHp = target.CurrentStats.CurrentHP;
        var targetMaxHp = target.CurrentStats.MaxHP;
        _view.WriteLine($"{target.Name} termina con HP:{targetHp}/{targetMaxHp}");
    }

    private void DisplayTurnConsumption(TurnConsumptionResult result)
    {
        _view.WriteSeparation();
        _view.WriteLine($"Se han consumido {result.FullTurnsConsumed} Full Turn(s) y {result.BlinkingTurnsConsumed} Blinking Turn(s)");
        _view.WriteLine($"Se han obtenido {result.BlinkingTurnsGained} Blinking Turn(s)");
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