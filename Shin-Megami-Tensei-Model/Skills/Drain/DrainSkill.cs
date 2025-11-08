using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Combat;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Skills.Drain;

public class DrainSkill: ISkill
{

    private readonly DamageCalculator _damageCalculator;
    private readonly AffinityHandler _affinityHandler;
    private readonly DrainType _drainType;
    public string Name { get; }
    public int Cost { get; }
    public int Power { get; }
    public HitRange HitRange { get; }
    public TargetType TargetType { get; }
    public Element Element { get; }
    
    
    public DrainSkill(
        string name,
        int cost,
        int power,
        Element element,
        HitRange hitRange,
        TargetType targetType,
        DrainType drainType)
    {
        Name = name;
        Cost = cost;
        Power = power;
        Element = element;
        TargetType = targetType;
        HitRange = hitRange ?? throw new ArgumentNullException(nameof(hitRange));
        _damageCalculator = new DamageCalculator();
        _affinityHandler = new AffinityHandler();
        _drainType = drainType;
    }

    public bool CanExecute(Unit user, GameState gameState)
    {
        return user.CurrentStats.HasSufficientMP(Cost);
    }

    public SkillResult Execute(Unit user, UnitsCollection targets, GameState gameState)
    {
        user.ConsumeMP(Cost);
        
        var hits = HitRange.CalculateHits(gameState.GetCurrentPlayerSkillCount());
        gameState.IncrementSkillCount();

        var effects = new List<SkillEffect>();

        foreach (var target in targets)
        {
            
            for (int i = 0; i < hits; i++)
            {
                var effect = ExecuteSingleHit(user, target);
                effects.Add(effect);
            }
        }

        var turnConsumption = TurnConsumption.NeutralOrResist();
        return new SkillResult(effects, turnConsumption, new List<string>());
    }
    
    private SkillEffect ExecuteSingleHit(Unit user, Unit target)
    {
        var baseDamage = _damageCalculator.CalculateMagicalDamage(user, Power);
        var affinity = Affinity.Neutral;
        var targetMaxHp = target.CurrentStats.CurrentHP;
        var targetMaxMp = target.CurrentStats.CurrentMP;
        var finalDrainHpDealt = (int)Math.Floor(Math.Min(baseDamage, targetMaxHp));
        var finalDrainMpDealt = (int)Math.Floor(Math.Min(baseDamage, targetMaxMp));
        var finalTargetHp = targetMaxHp - finalDrainHpDealt;
        var finalTargetMp = targetMaxMp - finalDrainMpDealt;

        if (_drainType == DrainType.HP)
        {
            target.TakeDamage(finalDrainHpDealt);
            user.Heal(finalDrainHpDealt);
            return new SkillEffectBuilder()
                .ForTarget(target)
                .WithAffinity(affinity)
                .WithFinalHP(finalTargetHp, target.CurrentStats.MaxHP)
                .WithElement(Element)
                .AsDrainHP()
                .WithDrainedHP(finalDrainHpDealt)
                .WithFinalMP(finalTargetMp, target.CurrentStats.MaxMP)
                .Build();
        }
        
        if (_drainType == DrainType.MP)
        {
            target.ConsumeMP(finalDrainMpDealt);
            user.GainMp(finalDrainMpDealt);
            return new SkillEffectBuilder()
                .ForTarget(target)
                .WithAffinity(affinity)
                .WithFinalHP(finalTargetHp, target.CurrentStats.MaxHP)
                .WithElement(Element)
                .AsDrainMP()
                .WithDrainedMP(finalDrainMpDealt)
                .WithFinalMP(finalTargetMp, target.CurrentStats.MaxMP)
                .Build();
        }
        
        target.TakeDamage(finalDrainHpDealt);
        target.ConsumeMP(finalDrainMpDealt);
        user.Heal(finalDrainHpDealt);
        user.GainMp(finalDrainMpDealt);
        return new SkillEffectBuilder()
            .ForTarget(target)
            .WithAffinity(affinity)
            .WithFinalHP(finalTargetHp, target.CurrentStats.MaxHP)
            .WithElement(Element)
            .AsDrainBoth()
            .WithDrainedHP(finalDrainHpDealt)
            .WithDrainedMP(finalDrainMpDealt)
            .WithFinalMP(finalTargetMp, target.CurrentStats.MaxMP)
            .Build();
    }
    
}