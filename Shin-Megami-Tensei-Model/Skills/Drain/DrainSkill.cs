using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Combat;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Skills.Drain;

public class DrainSkill: ISkill
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
    
    
    public DrainSkill(
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

    public SkillResult Execute(Unit user, List<Unit> targets, GameState gameState)
    {
        
        user.ConsumeMP(Cost);

        var hits = HitRange.CalculateHits(gameState.GetCurrentPlayerSkillCount());
        gameState.IncrementSkillCount();

        var effects = new List<SkillEffect>();
        var highestPriorityAffinity = Affinity.Neutral;

        /*foreach (var target in targets)
        {
            if (!target.IsAlive())
            {
                continue;
            }

            for (int i = 0; i < hits; i++)
            {
                var effect = ExecuteSingleHit(user, target);
                effects.Add(effect);

                if (GetAffinityPriority(effect.AffinityResult) > GetAffinityPriority(highestPriorityAffinity))
                {
                    highestPriorityAffinity = effect.AffinityResult;
                }
            }
        }*/
        
        Console.WriteLine($"[DEBUG] priority {highestPriorityAffinity}");
        
        var turnConsumption = CalculateTurnConsumption(highestPriorityAffinity);
        return new SkillResult(effects, turnConsumption, new List<string>());
    }
    
    /*private SkillEffect ExecuteSingleHit(Unit user, Unit target)
    {
        return new SkillEffect();
    }*/

    
    private TurnConsumption CalculateTurnConsumption(Affinity affinity)
    {
        return affinity switch
        {
            Affinity.Weak => TurnConsumption.Weak(),
            Affinity.Resist => TurnConsumption.NeutralOrResist(),
            Affinity.Null => TurnConsumption.Null(),
            Affinity.Miss => TurnConsumption.Miss(),
            Affinity.Repel => TurnConsumption.RepelOrDrain(),
            Affinity.Drain => TurnConsumption.RepelOrDrain(),
            _ => TurnConsumption.NeutralOrResist()
        };
    }
    
    private int GetAffinityPriority(Affinity affinity)
    {
        return affinity switch
        {
            Affinity.Repel => 6,
            Affinity.Drain => 6,
            Affinity.Null => 5,
            Affinity.Miss => 4,
            Affinity.Weak => 3,
            Affinity.Neutral => 1,
            Affinity.Resist => 1,
            _ => 0
        };
    }
    
}