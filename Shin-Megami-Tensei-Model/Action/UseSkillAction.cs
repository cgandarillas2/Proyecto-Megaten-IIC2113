using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Action;

public class UseSkillAction: IAction
{
    private readonly ISkill _skill;
    private readonly AffinityPriorityResolver _priorityResolver;

    public UseSkillAction(ISkill skill)
        : this(skill, new AffinityPriorityResolver())
    {
    }

    public UseSkillAction(ISkill skill, AffinityPriorityResolver priorityResolver)
    {
        _skill = skill;
        _priorityResolver = priorityResolver;
    }

    public bool CanExecute(Unit actor, GameState gameState)
    {
        return _skill.CanExecute(actor, gameState);
    }

    public ActionResult Execute(Unit actor, Unit target, GameState gameState)
    {
        var targets = new List<Unit> { target };
        return ExecuteMultiTarget(actor, targets, gameState);
    }

    public ActionResult ExecuteMultiTarget(Unit actor, List<Unit> targets, GameState gameState)
    {
        var skillResult = _skill.Execute(actor, targets, gameState);
        
        var totalDamage = CalculateTotalDamage(skillResult);
        var highestPriorityAffinity = _priorityResolver.GetHighestPriorityAffinity(skillResult);
        
        return ActionResult.Successful(
            skillResult.TurnConsumption, 
            totalDamage, 
            highestPriorityAffinity
        );
    }

    public SkillResult ExecuteAndGetResult(Unit actor, List<Unit> targets, GameState gameState)
    {
        return _skill.Execute(actor, targets, gameState);
    }

    public ISkill GetSkill()
    {
        return _skill;
    }

    private int CalculateTotalDamage(SkillResult skillResult)
    {
        return skillResult.Effects.Sum(effect => effect.DamageDealt);
    }
    
}