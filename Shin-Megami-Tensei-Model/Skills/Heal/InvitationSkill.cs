using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Skills.Heal;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Skills.Special;

public class InvitationSkill: ISkill
{
    private readonly int _healPower;
    private bool _isRevive { get; set; }
    public string Name { get; }
    public int Cost { get; }
    public HitRange HitRange { get; }
    public TargetType TargetType { get; }
    public Element Element => Element.Heal;

    public InvitationSkill(string name, int cost, HitRange hitRange)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Cost = cost;
        _healPower = 100;
        TargetType = TargetType.Ally;
        HitRange = hitRange;
    }

    public bool CanExecute(Unit user, GameState gameState)
    {
        bool isAlive = user.IsAlive();
        bool hasSufficientMP = user.CurrentStats.HasSufficientMP(Cost);
        return isAlive && hasSufficientMP;
    }

    public SkillResult Execute(Unit user, List<Unit> targets, GameState gameState)
    {
        user.ConsumeMP(Cost);

        var effects = new List<SkillEffect>();

        foreach (var target in targets)
        {
            if (target.IsAlive())
            {
                var effect = ExecuteSummon(target);
                effects.Add(effect);
            }
            
            if (!target.IsAlive())
            {
                var effect = ExecuteRevive(target);
                effects.Add(effect);
            }
            
        
        }
        
        gameState.IncrementSkillCount();
        var turnConsumption = TurnConsumption.NonOffensiveSkill();
        return new SkillResult(effects, turnConsumption, new List<string>());
    }

    private SkillEffect ExecuteSummon(Unit target)
    {
        return new SkillEffect(
            target.Name,
            0,
            0,
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
            SkillEffectType.Revive,
            true
        );
    }

    private int CalculateHealAmount(Unit target)
    {
        var maxHP = target.CurrentStats.MaxHP;
        var percentage = _healPower / 100.0;
        var healAmount = maxHP * percentage;
            
        return (int)Math.Floor(healAmount);
    }
    
    private bool HasMonstersToSummon(GameState gameState)
    {
        // Invitation puede invocar monstruos vivos o muertos
        return gameState.CurrentPlayer.GetAllReserveMonsters().Any();
    }

    public List<Monster> GetMostersFromReserve(GameState gameState)
    {
        return gameState.CurrentPlayer.GetAllReserveMonsters();
    }
}