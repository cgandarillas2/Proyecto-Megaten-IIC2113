using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Skills.Heal;

public class HealSkill: ISkill
{
    private readonly int _healPower;
    private readonly bool _isRevive;

    public string Name { get; }
    public int Cost { get; }
    public TargetType TargetType { get; }
    public Element Element => Element.Almighty;
    
    public HealSkill(
        string name,
        int cost,
        int healPower,
        TargetType targetType,
        bool isRevive = false)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Cost = cost;
        _healPower = healPower;
        TargetType = targetType;
        _isRevive = isRevive;
    }
    
    public bool CanExecute(Unit user, GameState gameState)
    {
        return user.IsAlive() && user.CurrentStats.HasSufficientMP(Cost);
    }

    public SkillResult Execute(Unit user, List<Unit> targets, GameState gameState)
    {
        user.ConsumeMP(Cost);

        var effects = new List<SkillEffect>();

        foreach (var target in targets)
        {
            if (_isRevive)
            {
                if (!target.IsAlive())
                {
                    var effect = ExecuteRevive(target);
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
            Element.Almighty,
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
            Element.Almighty,
            SkillEffectType.Revive
        );
    }

    private int CalculateHealAmount(Unit target)
    {
        // _healPower es un porcentaje (25, 50, 100)
        // Calcular el porcentaje del HP máximo
        var maxHP = target.CurrentStats.MaxHP;
        var percentage = _healPower / 100.0;
        var healAmount = maxHP * percentage;
            
        return (int)Math.Floor(healAmount);
    }

    public bool IsReviveSkill()
    {
        return _isRevive;
    }
}