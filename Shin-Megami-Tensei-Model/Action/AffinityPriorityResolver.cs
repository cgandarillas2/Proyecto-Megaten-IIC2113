using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Stats;

namespace Shin_Megami_Tensei_Model.Action;

public class AffinityPriorityResolver
{
    private readonly AffinityHandler _affinityHandler;

    public AffinityPriorityResolver()
    {
        _affinityHandler = new AffinityHandler();
    }

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
            var priority = _affinityHandler.GetAffinityPriority(effect.AffinityResult);
            if (priority > highestPriorityValue)
            {
                highestPriorityValue = priority;
                highestPriority = effect.AffinityResult;
            }
        }

        return highestPriority;
    }
}
