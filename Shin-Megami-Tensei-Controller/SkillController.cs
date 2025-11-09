using Shin_Megami_Tensei.Exceptions;
using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Game.TargetFilters;
using Shin_Megami_Tensei_Model.Services;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Skills.Heal;
using Shin_Megami_Tensei_Model.Units;
using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei_View.ConsoleLib;

namespace Shin_Megami_Tensei;

/// <summary>
/// Controller responsible for skill and target selection following SRP.
/// Sorting algorithms moved to TargetSorter service in model layer.
/// </summary>
public class SkillController
{
    private readonly IMenuSelector<ISkill> _skillSelector;
    private readonly IMenuSelector<Unit> _targetSelector;
    private readonly TargetFilterFactory _filterFactory;
    private readonly TargetSorter _targetSorter;
    private readonly View _view;
    private readonly SkillSelectionView _skillSelectionView;

    public SkillController(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _skillSelectionView = new SkillSelectionView(view);

        var skillRenderer = new SkillMenuRenderer(view);
        _skillSelector = new MenuSelector<ISkill>(view, skillRenderer);

        var targetRenderer = new UnitMenuRenderer(view, "Seleccione un objetivo");
        _targetSelector = new MenuSelector<Unit>(view, targetRenderer);

        _filterFactory = new TargetFilterFactory();
        _targetSorter = new TargetSorter();
    }

    public SkillController(
        IMenuSelector<ISkill> skillSelector,
        IMenuSelector<Unit> targetSelector,
        TargetFilterFactory filterFactory,
        TargetSorter targetSorter,
        View view)
    {
        _skillSelector = skillSelector ?? throw new ArgumentNullException(nameof(skillSelector));
        _targetSelector = targetSelector ?? throw new ArgumentNullException(nameof(targetSelector));
        _filterFactory = filterFactory ?? throw new ArgumentNullException(nameof(filterFactory));
        _targetSorter = targetSorter ?? throw new ArgumentNullException(nameof(targetSorter));
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _skillSelectionView = new SkillSelectionView(view);
    }

    public UseSkillAction SelectSkill(Unit actor, GameState gameState)
    {
        var availableSkills = actor.GetSkillsWithEnoughMana();

        if (availableSkills.Count == 0)
        {
            _skillSelectionView.ShowNoSkillsAvailable(actor.Name);
            _view.ReadLine();
            throw new NoSkillsAvailableException(actor.Name);
        }

        while (true)
        {
            var selectedSkill = _skillSelector.SelectFrom(availableSkills, actor);

            if (selectedSkill == null)
            {
                throw new OperationCancelledException("Selección de habilidad cancelada");
            }

            if (selectedSkill.CanExecute(actor, gameState))
            {
                return new UseSkillAction(selectedSkill);
            }

            _skillSelectionView.ShowInsufficientMP();
        }
    }

    public UnitsCollection SelectTargets(UseSkillAction skillAction, Unit actor, GameState gameState)
    {
        var skill = skillAction.GetSkill();

        var targetFilter = DetermineTargetFilter(skill);
        var validTargets = targetFilter.GetValidTargets(gameState, actor);

        if (validTargets.Count == 0)
        {
            _skillSelectionView.ShowNoValidTargets(actor.Name);
            _view.ReadLine();
            throw new NoValidTargetsException(actor.Name);
        }

        if (skill.TargetType == TargetType.Multi)
        {
            validTargets = _targetSorter.ApplyMultiTargetSort(skill, validTargets, gameState.CurrentPlayer.SkillCount);
            validTargets = _targetSorter.OrderByBoardPosition(validTargets, gameState);
        }

        if (IsAutomaticTarget(skill.TargetType))
        {
            return validTargets;
        }

        var selectedTarget = _targetSelector.SelectFrom(validTargets, actor);

        if (selectedTarget == null)
        {
            throw new OperationCancelledException("Selección de objetivo cancelada");
        }

        return new UnitsCollection(new[] { selectedTarget });
    }

    private bool IsAutomaticTarget(TargetType targetType)
    {
        return targetType is TargetType.Self
            or TargetType.All
            or TargetType.Multi
            or TargetType.Party
            or TargetType.Universal;
    }

    private ITargetFilter DetermineTargetFilter(ISkill skill)
    {
        bool isRevive = skill is ReviveSkill;
        bool isDrainHeal = skill is DrainHealSkill;

        if (isRevive || isDrainHeal)
        {
            return _filterFactory.CreateFilter(skill.TargetType, isRevive, isDrainHeal);
        }

        return _filterFactory.CreateFilter(skill.TargetType, false);
    }
}
