using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Skills.Heal;
using Shin_Megami_Tensei_Model.Skills.Special;
using Shin_Megami_Tensei_Model.Units;
using Shin_Megami_Tensei_View;

namespace Shin_Megami_Tensei;

public class SkillController
{
    private readonly View _view;

    public SkillController(View view)
    {
        _view = view;
    }
    
    public UseSkillAction SelectSkill(Unit actor, GameState gameState)
    {
        var skills = actor.GetSkillsWithEnoughMana();

        while (true)
        {
            DisplaySkillMenu(actor, skills);
            var choice = _view.ReadLine();
            var skill = ParseSkillChoice(choice, skills);
            if (skill != null)
            {
                if (skill.CanExecute(actor, gameState))
                {
                    return new UseSkillAction(skill);
                }
                DisplayInsufficientMP();
                continue;
            }

            if (IsCancelChoice(choice, skills))
            {
                return null;
            }
        }
    }
    
    public List<Unit> SelectTargets(UseSkillAction skillAction, Unit actor, GameState gameState)
    {
        var skill = skillAction.GetSkill();

        // Manejar Invitation y Sabbatma especialmente
        if (skill is InvitationSkill || skill is SabbatmaSkill)
        {
            // Estas se manejan en el CombatController
            throw new InvalidOperationException("Summon skills should be handled by CombatController");
        }

        // Manejar HealSkill según si es revive o heal normal
        if (skill is HealSkill healSkill)
        {
            return SelectHealTargets(healSkill, actor, gameState);
        }

        return skill.TargetType switch
        {
            TargetType.Single => SelectSingleTarget(actor, gameState),
            TargetType.All => SelectAllEnemies(gameState),
            TargetType.Multi => SelectMultipleTargets(actor, gameState),
            TargetType.Ally => SelectSingleAlly(actor, gameState),
            TargetType.Party => SelectAllAllies(gameState),
            TargetType.Self => SelectSelf(gameState),
            TargetType.Universal => SelectUniversalTarget(gameState),
            _ => SelectSingleTarget(actor, gameState)
        };
    }
    
    private List<Unit> SelectHealTargets(HealSkill healSkill, Unit actor, GameState gameState)
    {
        // Si es una habilidad de revivir, mostrar solo unidades muertas
        if (healSkill.IsReviveSkill())
        {
            return SelectDeadAlly(actor, gameState);
        }
            
        // Si es una habilidad de curación normal, mostrar solo unidades vivas
        return SelectAliveAlly(actor, gameState);
    }
    
    private List<Unit> SelectAliveAlly(Unit actor, GameState gameState)
    {
        var allies = gameState.CurrentPlayer.ActiveBoard.GetAliveUnits();

        while (true)
        {
            DisplayTargetMenu(actor, allies);
            var choice = _view.ReadLine();
            var target = ParseTargetChoice(choice, allies);

            if (target != null)
            {
                return new List<Unit> { target };
            }

            if (IsCancelTargetChoice(choice, allies))
            {
                return null;
            }
        }
    }
    
    private List<Unit> SelectDeadAlly(Unit actor, GameState gameState)
    {
        var deadUnits = gameState.CurrentPlayer.ActiveBoard.GetAllUnits()
            .Where(u => !u.IsEmpty() && !u.IsAlive())
            .ToList();

        var deadReserveMonsters = gameState.CurrentPlayer.GetDeadReserveMonsters();
        deadUnits.AddRange(deadReserveMonsters);

        if (!deadUnits.Any())
        {
            _view.WriteSeparation();
            _view.WriteLine($"Seleccione un objetivo para {actor.Name}");
            _view.WriteLine("1-Cancelar");
            _view.ReadLine();
            return null;
        }

        while (true)
        {
            DisplayTargetMenu(actor, deadUnits);
            var choice = _view.ReadLine();
            var target = ParseTargetChoice(choice, deadUnits);

            if (target != null)
            {
                return new List<Unit> { target };
            }

            if (IsCancelTargetChoice(choice, deadUnits))
            {
                return null;
            }
        }
    }
    
    private void DisplaySkillMenu(Unit actor, List<ISkill> skills)
    {
        _view.WriteSeparation();
        _view.WriteLine($"Seleccione una habilidad para que {actor.Name} use");

        for (int i = 0; i < skills.Count; i++)
        {
            var skill = skills[i];
            var canUse = actor.CurrentStats.HasSufficientMP(skill.Cost);
            var indicator = canUse ? "" : " (Sin MP)";
            _view.WriteLine($"{i + 1}-{skill.Name} MP:{skill.Cost}");
        }

        _view.WriteLine($"{skills.Count + 1}-Cancelar");
    }
    
    private void DisplayInsufficientMP()
    {
        _view.WriteLine("MP insuficiente para usar esta habilidad");
    }

    private ISkill ParseSkillChoice(string choice, List<ISkill> skills)
    {
        if (!int.TryParse(choice, out int selection))
        {
            return null;
        }

        if (selection < 1 || selection > skills.Count)
        {
            return null;
        }

        return skills[selection - 1];
    }
    
    private bool IsCancelChoice(string choice, List<ISkill> skills)
    {
        if (!int.TryParse(choice, out int selection))
        {
            return false;
        }

        return selection == skills.Count + 1;
    }
    
    private List<Unit> SelectSingleTarget(Unit actor, GameState gameState)
    {
        var opponents = gameState.GetOpponentAliveUnits();

        while (true)
        {
            DisplayTargetMenu(actor, opponents);
            var choice = _view.ReadLine();
            var target = ParseTargetChoice(choice, opponents);

            if (target != null)
            {
                return new List<Unit> { target };
            }

            if (IsCancelTargetChoice(choice, opponents))
            {
                return null;
            }
        }
    }
    
    private List<Unit> SelectAllEnemies(GameState gameState)
    {
        return gameState.GetOpponentAliveUnits();
    }

    private List<Unit> SelectMultipleTargets(Unit actor, GameState gameState)
    {
        var opponents = gameState.GetOpponentAliveUnits();
        var selected = new List<Unit>();

        while (true)
        {
            DisplayMultiTargetMenu(actor,opponents, selected);
            var choice = _view.ReadLine();

            if (IsConfirmChoice(choice))
            {
                return selected.Count > 0 ? selected : null;
            }

            if (IsCancelTargetChoice(choice, opponents))
            {
                return null;
            }

            var target = ParseTargetChoice(choice, opponents);
            if (target != null)
            {
                ToggleTarget(selected, target);
            }
        }
    }
    
    private List<Unit> SelectSingleAlly(Unit actor, GameState gameState)
    {
        var allies = gameState.CurrentPlayer.ActiveBoard.GetAliveUnits();

        while (true)
        {
            DisplayTargetMenu(actor, allies);
            var choice = _view.ReadLine();
            var target = ParseTargetChoice(choice, allies);

            if (target != null)
            {
                return new List<Unit> { target };
            }

            if (IsCancelTargetChoice(choice, allies))
            {
                return null;
            }
        }
    }
    
    private List<Unit> SelectAllAllies(GameState gameState)
    {
        return gameState.CurrentPlayer.ActiveBoard.GetAliveUnits();
    }

    private List<Unit> SelectSelf(GameState gameState)
    {
        var actor = gameState.GetCurrentActingUnit();
        return new List<Unit> { actor };
    }

    private List<Unit> SelectUniversalTarget(GameState gameState)
    {
        var allUnits = new List<Unit>();
        allUnits.AddRange(gameState.CurrentPlayer.ActiveBoard.GetAliveUnits());
        allUnits.AddRange(gameState.GetOpponent().ActiveBoard.GetAliveUnits());
        return allUnits;
    }
    
    private void DisplayTargetMenu(Unit actor, List<Unit> targets)
    {
        _view.WriteSeparation();
        _view.WriteLine($"Seleccione un objetivo para {actor.Name}");

        for (int i = 0; i < targets.Count; i++)
        {
            var target = targets[i];
            var hp = target.CurrentStats.CurrentHP;
            var maxHp = target.CurrentStats.MaxHP;
            var mp = target.CurrentStats.CurrentMP;
            var maxMp = target.CurrentStats.MaxMP;

            _view.WriteLine($"{i + 1}-{target.Name} HP:{hp}/{maxHp} MP:{mp}/{maxMp}");
        }

        _view.WriteLine($"{targets.Count + 1}-Cancelar");
    }
    
    private void DisplayMultiTargetMenu(Unit actor, List<Unit> targets, List<Unit> selected)
    {
        _view.WriteSeparation();
        _view.WriteLine($"Seleccione objetivos para {actor.Name} (puede elegir varios)");

        for (int i = 0; i < targets.Count; i++)
        {
            var target = targets[i];
            var isSelected = selected.Contains(target);
            var marker = isSelected ? "[X]" : "[ ]";
            var hp = target.CurrentStats.CurrentHP;
            var maxHp = target.CurrentStats.MaxHP;
            var mp = target.CurrentStats.CurrentMP;
            var maxMp = target.CurrentStats.MaxMP;

            _view.WriteLine($"{i + 1} {marker} {target.Name} HP:{hp}/{maxHp} MP:{mp}/{maxMp}");
        }

        _view.WriteLine($"{targets.Count + 1}-Confirmar");
        _view.WriteLine($"{targets.Count + 2}-Cancelar");
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

    private bool IsCancelTargetChoice(string choice, List<Unit> targets)
    {
        if (!int.TryParse(choice, out int selection))
        {
            return false;
        }

        return selection == targets.Count + 1;
    }
    
    private bool IsConfirmChoice(string choice)
    {
        return choice == "0";
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

    private void ToggleTarget(List<Unit> selected, Unit target)
    {
        if (selected.Contains(target))
        {
            selected.Remove(target);
        }
        else
        {
            selected.Add(target);
        }
    }
}