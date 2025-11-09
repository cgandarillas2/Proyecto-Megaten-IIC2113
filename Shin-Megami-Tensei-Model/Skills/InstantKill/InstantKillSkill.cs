using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Combat;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Skills.InstantKill;

public class InstantKillSkill: ISkill
{
    
    private readonly int _power;

    private readonly DamageCalculator _damageCalculator;
    private readonly AffinityHandler _affinityHandler;
    public string Name { get; }
    public int Cost { get; }
    public int Power { get; }
    public HitRange HitRange { get; }
    public TargetType TargetType { get; }
    public Element Element { get; }
    
    
    public InstantKillSkill(
        string name,
        int cost,
        int power,
        Element element,
        HitRange hitRange,
        TargetType targetType)
    {
        Name = name;
        Cost = cost;
        Power = power;
        Element = element;
        TargetType = targetType;
        HitRange = hitRange ?? throw new ArgumentNullException(nameof(hitRange));
        _damageCalculator = new DamageCalculator();
        _affinityHandler = new AffinityHandler();
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
        var highestPriorityAffinity = Affinity.Neutral;

        foreach (var target in targets)
        {
            /*if (!target.IsAlive())
            {
                continue;
            }
            */

            for (int i = 0; i < hits; i++)
            {
                var effect = ExecuteSingleHit(user, target);
                effects.Add(effect);

                if (_affinityHandler.GetAffinityPriority(effect.AffinityResult) > _affinityHandler.GetAffinityPriority(highestPriorityAffinity))
                {
                    highestPriorityAffinity = effect.AffinityResult;
                }
            }
        }

        var turnConsumption = _affinityHandler.CalculateTurnConsumption(highestPriorityAffinity);
        return new SkillResult(new SkillEffectsCollection(effects), turnConsumption, StringCollection.Empty());
    }
    
    private SkillEffect ExecuteSingleHit(Unit user, Unit target)
    {
        var luckAttacker = user.CurrentStats.Lck;
        var luckTarget = target.CurrentStats.Lck;

        var affinity = target.Affinities.GetAffinity(Element);

        var isInstantKill = IsInstantKill(luckAttacker, Power, luckTarget, affinity);
        var isNeutralOrResist = affinity == Affinity.Neutral || affinity == Affinity.Resist;
        if (isNeutralOrResist && !isInstantKill)
        {
            affinity = Affinity.Miss;
        }
        
        var baseDamage = _damageCalculator.CalculateInstantKillDamage(target);

        var finalDamage = isInstantKill ? baseDamage : 0;

        if (affinity == Affinity.Repel)
        {
            user.KillInstantly();
            return new SkillEffectBuilder()
                .ForTarget(target)
                .WithDamage(finalDamage)
                .TargetDied(!user.IsAlive())
                .WithAffinity(affinity)
                .WithFinalHP(user.CurrentStats.CurrentHP, user.CurrentStats.MaxHP)
                .WithElement(Element)
                .AsInstantKill()
                .Build();
        }
        
        target.TakeDamage(finalDamage);
        var died = !target.IsAlive();

        return new SkillEffectBuilder()
            .ForTarget(target)
            .WithDamage(finalDamage)
            .TargetDied(died)
            .WithAffinity(affinity)
            .WithFinalHP(target.CurrentStats.CurrentHP, target.CurrentStats.MaxHP)
            .WithElement(Element)
            .AsInstantKill()
            .Build();
    }

    private bool IsInstantKill(int luckAttacker, int skillPowerAttacker, int luckTarget, Affinity affinity)
    {
        return affinity switch
        {
            Affinity.Weak => true,
            Affinity.Resist => IsSuccessFromAffinity(luckAttacker, skillPowerAttacker, luckTarget * 2),
            Affinity.Null => false,
            Affinity.Neutral => IsSuccessFromAffinity(luckAttacker, skillPowerAttacker, luckTarget),
            _ => false
        };
    }

    private bool IsSuccessFromAffinity(int luckAttacker, int skillPowerAttacker, int luckTarget)
    {
        return luckAttacker + skillPowerAttacker >= luckTarget;
    }
}