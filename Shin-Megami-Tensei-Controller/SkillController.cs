using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Game.TargetFilters;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Skills.Heal;
using Shin_Megami_Tensei_Model.Skills.Special;
using Shin_Megami_Tensei_Model.Units;
using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei_View.ConsoleLib;

namespace Shin_Megami_Tensei;

public class SkillController
{
    private readonly IMenuSelector<ISkill> _skillSelector;
    private readonly IMenuSelector<Unit> _targetSelector;
    private readonly TargetFilterFactory _filterFactory;
    private readonly View _view;

    public SkillController(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        
        var skillRenderer = new SkillMenuRenderer(view);
        _skillSelector = new MenuSelector<ISkill>(view, skillRenderer);
        
        var targetRenderer = new UnitMenuRenderer(view, "Seleccione un objetivo");
        _targetSelector = new MenuSelector<Unit>(view, targetRenderer);
        
        _filterFactory = new TargetFilterFactory();
    }
    
    public SkillController(
        IMenuSelector<ISkill> skillSelector,
        IMenuSelector<Unit> targetSelector,
        TargetFilterFactory filterFactory,
        View view)
    {
        _skillSelector = skillSelector ?? throw new ArgumentNullException(nameof(skillSelector));
        _targetSelector = targetSelector ?? throw new ArgumentNullException(nameof(targetSelector));
        _filterFactory = filterFactory ?? throw new ArgumentNullException(nameof(filterFactory));
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }
    
    public UseSkillAction SelectSkill(Unit actor, GameState gameState)
    {
        var availableSkills = actor.GetSkillsWithEnoughMana();
        
        if (availableSkills.Count == 0)
        {
            ShowNoSkillsAvailableMessage(actor);
            return null;
        }

        while (true)
        {
            var selectedSkill = _skillSelector.SelectFrom(availableSkills, actor);
            
            if (selectedSkill == null)
            {
                return null; // Cancelado
            }

            if (selectedSkill.CanExecute(actor, gameState))
            {
                return new UseSkillAction(selectedSkill);
            }

            ShowInsufficientMPMessage();
        }
    }
    
    public List<Unit> SelectTargets(UseSkillAction skillAction, Unit actor, GameState gameState)
    {
        var skill = skillAction.GetSkill();

        var targetFilter = DetermineTargetFilter(skill);
        var validTargets = targetFilter.GetValidTargets(gameState, actor);

        if (validTargets.Count == 0)
        {
            ShowNoValidTargetsMessage(actor);
            return null;
        }

        // Si el target es automático (Self, All, Party, Universal)
        if (IsAutomaticTarget(skill.TargetType))
        {
            return validTargets;
        }

        var selectedTarget = _targetSelector.SelectFrom(validTargets, actor);
        
        if (selectedTarget == null)
        {
            return null; // Usuario canceló
        }

        return new List<Unit> { selectedTarget };
    }
    
    private bool IsAutomaticTarget(TargetType targetType)
    {
        return targetType is TargetType.Self 
            or TargetType.All 
            or TargetType.Party 
            or TargetType.Universal;
    }
    
    private ITargetFilter DetermineTargetFilter(ISkill skill)
    {
        bool isRevive = skill is HealSkill healSkill && healSkill.IsReviveSkill();
        return _filterFactory.CreateFilter(skill.TargetType, isRevive);
    }
    
    private void ShowNoSkillsAvailableMessage(Unit actor)
    {
        _view.WriteSeparation();
        _view.WriteLine($"Seleccione una habilidad para que {actor.Name} use");
        _view.WriteLine("1-Cancelar");
        _view.ReadLine(); // Simula la interacción
    }
    
    private void ShowInsufficientMPMessage()
    {
        _view.WriteLine("MP insuficiente para usar esta habilidad");
    }

    private void ShowNoValidTargetsMessage(Unit actor)
    {
        _view.WriteSeparation();
        _view.WriteLine($"Seleccione un objetivo para {actor.Name}");
        _view.WriteLine("1-Cancelar");
        _view.ReadLine();
    }
    
    
}