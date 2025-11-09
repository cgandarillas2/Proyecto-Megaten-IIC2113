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
        var messages = new List<string>();

        foreach (var target in targets)
        {
            var effect = ApplyEffect(target, messages);
            effects.Add(effect);
        }

        var turnConsumption = DetermineTurnConsumption();
        var effectsCollection = new SkillEffectsCollection(effects);
        var messagesCollection = new StringCollection(messages);

        return new SkillResult(effectsCollection, turnConsumption, messagesCollection);
    }

    private SkillEffect ApplyEffect(Unit target, List<string> messages)
    {
        return _effectType switch
        {
            SupportEffectType.ChargePhysical => ApplyPhysicalCharge(target, messages),
            SupportEffectType.ChargeMagical => ApplyMagicalCharge(target, messages),
            SupportEffectType.BuffAttack => ApplyAttackBuff(target, messages),
            SupportEffectType.BuffDefense => ApplyDefenseBuff(target, messages),
            SupportEffectType.BloodRitual => ApplyBloodRitual(target, messages),
            _ => throw new InvalidOperationException($"Unknown support effect: {_effectType}")
        };
    }

    private SkillEffect ApplyPhysicalCharge(Unit target, List<string> messages)
    {
        target.ApplyPhysicalCharge();
        messages.Add($"{target.Name} ha cargado su siguiente ataque físico o disparo a más del doble");

        return CreateSupportEffect(target, SkillEffectType.ChargePhysical);
    }

    private SkillEffect ApplyMagicalCharge(Unit target, List<string> messages)
    {
        target.ApplyMagicalCharge();
        messages.Add($"{target.Name} ha cargado su siguiente ataque mágico a más del doble");

        return CreateSupportEffect(target, SkillEffectType.ChargeMagical);
    }

    private SkillEffect ApplyAttackBuff(Unit target, List<string> messages)
    {
        target.IncreaseOffensiveGrade();
        messages.Add($"El ataque de {target.Name} ha aumentado");

        return CreateSupportEffect(target, SkillEffectType.BuffAttack);
    }

    private SkillEffect ApplyDefenseBuff(Unit target, List<string> messages)
    {
        target.IncreaseDefensiveGrade();
        messages.Add($"La defensa de {target.Name} ha aumentado");

        return CreateSupportEffect(target, SkillEffectType.BuffDefense);
    }

    private SkillEffect ApplyBloodRitual(Unit target, List<string> messages)
    {
        target.IncreaseOffensiveGrade();
        messages.Add($"El ataque de {target.Name} ha aumentado");

        target.IncreaseDefensiveGrade();
        messages.Add($"La defensa de {target.Name} ha aumentado");

        target.SetHP(1);
        messages.Add($"{target.Name} termina con HP:{target.CurrentStats.CurrentHP}/{target.CurrentStats.MaxHP}");

        return CreateSupportEffect(target, SkillEffectType.BloodRitual);
    }

    private SkillEffect CreateSupportEffect(Unit target, SkillEffectType effectType)
    {
        return new SkillEffect(
            target: target,
            damageDealt: 0,
            healingDone: 0,
            targetDied: false,
            affinityResult: Affinity.Neutral,
            finalHP: target.CurrentStats.CurrentHP,
            maxHP: target.CurrentStats.MaxHP,
            element: Element.Support,
            effectType: effectType);
    }

    private TurnConsumption DetermineTurnConsumption()
    {
        return _effectType == SupportEffectType.BloodRitual
            ? TurnConsumption.Blinking()
            : TurnConsumption.Full();
    }
}
