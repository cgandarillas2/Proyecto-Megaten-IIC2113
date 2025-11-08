using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Skills.Heal;

public class DrainHealSkill : HealSkillBase
{
    public DrainHealSkill(
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
            else if (target == user)
            {
                var effect = ExecuteDrainHeal(user);
                effects.Add(effect);
            }
            else
            {
                var effect = ExecuteHeal(target);
                effects.Add(effect);
            }
        }

        gameState.IncrementSkillCount();
        var turnConsumption = TurnConsumption.NonOffensiveSkill();
        return new SkillResult(effects, turnConsumption, new List<string>());
    }

    private SkillEffect ExecuteDrainHeal(Unit actor)
    {
        var damage = actor.CurrentStats.CurrentHP;
        actor.KillInstantly();

        return new SkillEffectBuilder()
            .ForTarget(actor)
            .WithDamage(damage)
            .TargetDied(true)
            .WithAffinity(Affinity.Neutral)
            .WithFinalHP(actor.CurrentStats.CurrentHP, actor.CurrentStats.MaxHP)
            .WithElement(Element.Heal)
            .AsHealAndDie()
            .Build();
    }
}
