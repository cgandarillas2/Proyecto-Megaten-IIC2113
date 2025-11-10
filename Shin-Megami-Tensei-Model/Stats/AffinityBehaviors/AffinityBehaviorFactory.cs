namespace Shin_Megami_Tensei_Model.Stats.AffinityBehaviors;

public class AffinityBehaviorFactory
{
    private readonly WeakAffinityBehavior _weakBehavior = new();
    private readonly ResistAffinityBehavior _resistBehavior = new();
    private readonly NullAffinityBehavior _nullBehavior = new();
    private readonly RepelAffinityBehavior _repelBehavior = new();
    private readonly DrainAffinityBehavior _drainBehavior = new();
    private readonly NeutralAffinityBehavior _neutralBehavior = new();
    private readonly MissAffinityBehavior _missBehavior = new();

    public IAffinityBehavior GetBehavior(Affinity affinity)
    {
        return affinity switch
        {
            Affinity.Weak => _weakBehavior,
            Affinity.Resist => _resistBehavior,
            Affinity.Null => _nullBehavior,
            Affinity.Repel => _repelBehavior,
            Affinity.Drain => _drainBehavior,
            Affinity.Neutral => _neutralBehavior,
            Affinity.Miss => _missBehavior,
            _ => _neutralBehavior
        };
    }
}
