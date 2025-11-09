using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Skills.Support;

public class SupportSkill : ISkill
{
    private readonly SupportEffectType _effectType;

    public string Name { get; }
    public int Cost { get; }
    public HitRange HitRange { get; }
    public TargetType TargetType { get; }
    public Element Element => Element.Support;

    public SupportSkill(
        string name,
        int cost,
        SupportEffectType effectType,
        TargetType targetType,
        HitRange hitRange)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Cost = cost;
        _effectType = effectType;
        TargetType = targetType;
        HitRange = hitRange ?? throw new ArgumentNullException(nameof(hitRange));
    }

    public bool CanExecute(Unit user, GameState gameState)
    {
        if (!user.IsAlive() || !user.CurrentStats.HasSufficientMP(Cost))
        {
            return false;
        }

        if (_effectType == SupportEffectType.BloodRitual)
        {
            return user.CurrentStats.CurrentHP >= 2;
        }

        return true;
    }

    public SkillResult Execute(Unit user, UnitsCollection targets, GameState gameState)
    {
        user.ConsumeMP(Cost);

        var effects = new List<SkillEffect>();

        foreach (var target in targets)
        {
            var effect = ApplyEffect(target);
            effects.Add(effect);
        }

        var turnConsumption = TurnConsumption.NeutralOrResist();
        var effectsCollection = new SkillEffectsCollection(effects);

        return new SkillResult(effectsCollection, turnConsumption, StringCollection.Empty());
    }

    private SkillEffect ApplyEffect(Unit target)
    {
        return _effectType switch
        {
            SupportEffectType.ChargePhysical => ApplyPhysicalCharge(target),
            SupportEffectType.ChargeMagical => ApplyMagicalCharge(target),
            SupportEffectType.BuffAttack => ApplyAttackBuff(target),
            SupportEffectType.BuffDefense => ApplyDefenseBuff(target),
            SupportEffectType.BloodRitual => ApplyBloodRitual(target),
            _ => throw new InvalidOperationException($"Unknown support effect: {_effectType}")
        };
    }

    private SkillEffect ApplyPhysicalCharge(Unit target)
    {
        target.ApplyPhysicalCharge();
        return CreateSupportEffect(target, SkillEffectType.ChargePhysical);
    }

    private SkillEffect ApplyMagicalCharge(Unit target)
    {
        target.ApplyMagicalCharge();
        return CreateSupportEffect(target, SkillEffectType.ChargeMagical);
    }

    private SkillEffect ApplyAttackBuff(Unit target)
    {
        target.IncreaseOffensiveGrade();
        return CreateSupportEffect(target, SkillEffectType.BuffAttack);
    }

    private SkillEffect ApplyDefenseBuff(Unit target)
    {
        target.IncreaseDefensiveGrade();
        return CreateSupportEffect(target, SkillEffectType.BuffDefense);
    }

    private SkillEffect ApplyBloodRitual(Unit target)
    {
        target.IncreaseOffensiveGrade();
        target.IncreaseDefensiveGrade();
        target.SetHP(1);

        return CreateSupportEffect(target, SkillEffectType.BloodRitual);
    }

    private SkillEffect CreateSupportEffect(Unit target, SkillEffectType effectType)
    {
        return new SkillEffectBuilder()
            .ForTarget(target)
            .WithAffinity(Affinity.Neutral)
            .WithFinalHP(target.CurrentStats.CurrentHP, target.CurrentStats.MaxHP)
            .WithElement(Element.Support)
            .WithEffectType(effectType)
            .Build();
    }
    
}
