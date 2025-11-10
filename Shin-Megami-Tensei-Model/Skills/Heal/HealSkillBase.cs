using System;
using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Skills.Heal;

public abstract class HealSkillBase : ISkill
{
    protected readonly int _healPower;

    public string Name { get; }
    public int Cost { get; }
    public HitRange HitRange { get; }
    public TargetType TargetType { get; }
    public Element Element => Element.Heal;

    protected HealSkillBase(
        string name,
        int cost,
        int healPower,
        TargetType targetType,
        HitRange hitRange)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Cost = cost;
        _healPower = healPower;
        TargetType = targetType;
        HitRange = hitRange;
    }

    public bool CanExecute(Unit user, GameState gameState)
    {
        return user.IsAlive() && user.CurrentStats.HasSufficientMP(Cost);
    }

    public abstract SkillResult Execute(Unit user, UnitsCollection targets, GameState gameState);

    protected int CalculateHealAmount(Unit target)
    {
        var maxHP = target.CurrentStats.MaxHP;
        var percentage = _healPower / 100.0;
        var healAmount = maxHP * percentage;

        return (int)Math.Floor(healAmount);
    }

    protected SkillEffect ExecuteHeal(Unit target)
    {
        var healAmount = CalculateHealAmount(target);
        target.Heal(healAmount);

        return new SkillEffectBuilder()
            .ForTarget(target)
            .WithHealing(healAmount)
            .WithAffinity(Affinity.Neutral)
            .WithFinalHP(target.CurrentStats.CurrentHP, target.CurrentStats.MaxHP)
            .WithElement(Element.Heal)
            .AsHealing()
            .Build();
    }

    protected SkillEffect ExecuteRevive(Unit target)
    {
        var healAmount = CalculateHealAmount(target);
        target.Revive(healAmount);

        return new SkillEffectBuilder()
            .ForTarget(target)
            .WithHealing(healAmount)
            .WithAffinity(Affinity.Neutral)
            .WithFinalHP(target.CurrentStats.CurrentHP, target.CurrentStats.MaxHP)
            .WithElement(Element.Heal)
            .AsRevive()
            .Build();
    }
}
