using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Skills.Special;

public class SabbatmaSkill: ISkill
{
    public string Name { get; }
    public int Cost { get; }
    public HitRange HitRange { get; }
    public TargetType TargetType { get; }
    public Element Element => Element.Special;
    
    public SabbatmaSkill(string name, int cost, HitRange hitRange)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Cost = cost;
        TargetType = TargetType.Ally;
        HitRange = hitRange;

    }
    public bool CanExecute(Unit user, GameState gameState)
    {
        bool isAlive = user.IsAlive();
        bool hasSufficientMP = user.CurrentStats.HasSufficientMP(Cost);
        return isAlive && hasSufficientMP;
    }

    public SkillResult Execute(Unit user, UnitsCollection targets, GameState gameState)
    {
        user.ConsumeMP(Cost);

        var effects = new List<SkillEffect>();

        foreach (var target in targets)
        {
            var effect = ExecuteSabbatma(target);
            effects.Add(effect);
        }
        
        gameState.IncrementSkillCount();
        var turnConsumption = TurnConsumption.NonOffensiveSkill();
        return new SkillResult(effects, turnConsumption, new List<string>());
    }
    
    private SkillEffect ExecuteSabbatma(Unit target)
    {

        return new SkillEffectBuilder()
            .ForTarget(target)
            .WithAffinity(Affinity.Neutral)
            .WithFinalHP(target.CurrentStats.CurrentHP, target.CurrentStats.MaxHP)
            .WithElement(Element.Special)
            .AsRevive()
            .Build();
    }
}