using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Stats;

namespace Shin_Megami_Tensei_Model.Action;

public class AffinityPriorityResolver
{
    public Affinity GetHighestPriorityAffinity(SkillResult skillResult)
    {
        if (skillResult.Effects.Count == 0)
        {
            return Affinity.Neutral;
        }

        var highestPriority = Affinity.Neutral;
        var highestPriorityValue = 0;

        foreach (var effect in skillResult.Effects)
        {
            var priority = GetAffinityPriority(effect.AffinityResult);
            if (priority > highestPriorityValue)
            {
                highestPriorityValue = priority;
                highestPriority = effect.AffinityResult;
            }
        }

        return highestPriority;
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