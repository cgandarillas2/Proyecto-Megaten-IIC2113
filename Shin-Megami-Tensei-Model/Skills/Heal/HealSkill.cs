using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Skills.Heal;

public class HealSkill : HealSkillBase
{
    public HealSkill(
        string name,
        int cost,
        int healPower,
        TargetType targetType,
        HitRange hitRange)
        : base(name, cost, healPower, targetType, hitRange)
    {
    }

    public override SkillResult Execute(Unit user, List<Unit> targets, GameState gameState)
    {
        user.ConsumeMP(Cost);

        var effects = new List<SkillEffect>();

        foreach (var target in targets)
        {
            if (target.IsAlive())
            {
                var effect = ExecuteHeal(target);
                effects.Add(effect);
            }
        }

        gameState.IncrementSkillCount();
        var turnConsumption = TurnConsumption.NonOffensiveSkill();
        return new SkillResult(effects, turnConsumption, new List<string>());
    }
}