using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Skills.Heal;

public class HealSkill: ISkill
{
    private readonly int _healPower;
    private readonly bool _isRevive;
    private readonly bool _isDrainHeal;

    public string Name { get; }
    public int Cost { get; }
    public HitRange HitRange { get; }
    public TargetType TargetType { get; }
    public Element Element => Element.Heal;
    
    public HealSkill(
        string name,
        int cost,
        int healPower,
        TargetType targetType,
        HitRange hitRange,
        bool isRevive = false,
        bool isDrainHeal = false)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Cost = cost;
        _healPower = healPower;
        TargetType = targetType;
        HitRange = hitRange;
        _isRevive = isRevive;
        _isDrainHeal = isDrainHeal;
    }
    
    public bool CanExecute(Unit user, GameState gameState)
    {
        return user.IsAlive() && user.CurrentStats.HasSufficientMP(Cost);
    }

    public SkillResult Execute(Unit user, List<Unit> targets, GameState gameState)
    {
        user.ConsumeMP(Cost);

        var effects = new List<SkillEffect>();

        // ARREGLAR IDENTACIONES -> Logica revive y else
        foreach (var target in targets)
        {
            if (_isRevive)
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
            else if (_isDrainHeal)
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
            else
            {
                if (target.IsAlive())
                {
                    var effect = ExecuteHeal(target);
                    effects.Add(effect);
                }
            }
        }

        gameState.IncrementSkillCount();
        var turnConsumption = TurnConsumption.NonOffensiveSkill();
        return new SkillResult(effects, turnConsumption, new List<string>());
    }
    
    private SkillEffect ExecuteHeal(Unit target)
    {
        var healAmount = CalculateHealAmount(target);
        target.Heal(healAmount);

        return new SkillEffect(
            target.Name,
            0,
            healAmount,
            false,
            Affinity.Neutral,
            target.CurrentStats.CurrentHP,
            target.CurrentStats.MaxHP,
            Element.Heal,
            SkillEffectType.Healing
        );
    }

    private SkillEffect ExecuteRevive(Unit target)
    {
        var healAmount = CalculateHealAmount(target);
        target.Revive(healAmount);

        return new SkillEffect(
            target.Name,
            0,
            healAmount,
            false,
            Affinity.Neutral,
            target.CurrentStats.CurrentHP,
            target.CurrentStats.MaxHP,
            Element.Heal,
            SkillEffectType.Revive
        );
    }

    private SkillEffect ExecuteDrainHeal(Unit actor)
    {
        var damage = actor.CurrentStats.CurrentHP;
        actor.KillInstantly();

        return new SkillEffect(
            actor.Name,
            damage,
            0,
            true,
            Affinity.Neutral,
            actor.CurrentStats.CurrentHP,
            actor.CurrentStats.MaxHP,
            Element.Heal,
            SkillEffectType.HealAndDie);
    }
    

    private int CalculateHealAmount(Unit target)
    {
        var maxHP = target.CurrentStats.MaxHP;
        var percentage = _healPower / 100.0;
        var healAmount = maxHP * percentage;
            
        return (int)Math.Floor(healAmount);
    }

    public bool IsReviveSkill()
    {
        return _isRevive;
    }

    public bool IsDrainHeal()
    {
        return _isDrainHeal;
    }
}