using Shin_Megami_Tensei_Model.Action;
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

    public SkillResult Execute(Unit user, List<Unit> targets, GameState gameState)
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
        
        Console.WriteLine($"[DEBUG] ");
        
        var turnConsumption = TurnConsumption.NeutralOrResist();
        return new SkillResult(effects, turnConsumption, new List<string>());
    }
    
    private SkillEffect ExecuteSingleHit(Unit user, Unit target)
    {
        var baseDamage = _damageCalculator.CalculateMagicalDamage(user, Power);
        Console.WriteLine($"[DEBUG] drain: {baseDamage}");
        var affinity = Affinity.Neutral;
        var targetMaxHp = target.CurrentStats.CurrentHP;
        var targetMaxMp = target.CurrentStats.CurrentMP;
        var finalDrainHpDealt = (int)Math.Floor(Math.Min(baseDamage, targetMaxHp));
        var finalDrainMpDealt = (int)Math.Floor(Math.Min(baseDamage, targetMaxMp));
        var finalTargetHp = targetMaxHp - finalDrainHpDealt;
        var finalTargetMp = targetMaxMp - finalDrainMpDealt;

        Console.WriteLine($"[DEBUG] skillname: {Name} type: {_drainType}");
        
        if (_drainType == DrainType.HP)
        {
            target.TakeDamage(finalDrainHpDealt);
            user.Heal(finalDrainHpDealt);
            return new SkillEffect(
                target,
                0,
                0,
                false,
                affinity,
                finalTargetHp,
                target.CurrentStats.MaxHP,
                Element,
                SkillEffectType.DrainHP,
                false,
                false,
                finalDrainHpDealt,
                0,
                finalTargetMp,
                target.CurrentStats.MaxMP
            );
        }
        
        if (_drainType == DrainType.MP)
        {
            target.ConsumeMP(finalDrainMpDealt);
            user.GainMp(finalDrainMpDealt);
            return new SkillEffect(
                target,
                0,
                0,
                false,
                affinity,
                finalTargetHp,
                target.CurrentStats.MaxHP,
                Element,
                SkillEffectType.DrainMP,
                false,
                false,
                0,
                finalDrainMpDealt,
                finalTargetMp,
                target.CurrentStats.MaxMP
            );
        }
        
        target.TakeDamage(finalDrainHpDealt);
        target.ConsumeMP(finalDrainMpDealt);
        user.Heal(finalDrainHpDealt);
        user.GainMp(finalDrainMpDealt);
        return new SkillEffect(
            target,
            0,
            0,
            false,
            affinity,
            finalTargetHp,
            target.CurrentStats.MaxHP,
            Element,
            SkillEffectType.DrainBoth,
            false,
            false,
            finalDrainHpDealt,
            finalDrainMpDealt,
            finalTargetMp,
            target.CurrentStats.MaxMP
        );
    }
    
}