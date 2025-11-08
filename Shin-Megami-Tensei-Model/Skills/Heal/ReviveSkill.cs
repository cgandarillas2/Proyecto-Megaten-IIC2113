using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Skills.Heal;

public class ReviveSkill : HealSkillBase
{
    public ReviveSkill(
        string name,
        int cost,
        int healPower,
        TargetType targetType,
        HitRange hitRange)
        : base(name, cost, healPower, targetType, hitRange)
    {
    }

    public override SkillResult Execute(Unit user, UnitsCollection targets, GameState gameState)
    {
        user.ConsumeMP(Cost);

        var effects = new List<SkillEffect>();

        foreach (var target in targets)
        {
            if (!target.IsAlive())
            {
                var effect = ExecuteRevive(target);
                effects.Add(effect);

                if (target is Samurai)
                {
                    gameState.ActionQueue.AddToEnd(target);
                }
            }
        }

        gameState.IncrementSkillCount();
        var turnConsumption = TurnConsumption.NonOffensiveSkill();
        return new SkillResult(effects, turnConsumption, new List<string>());
    }
}
