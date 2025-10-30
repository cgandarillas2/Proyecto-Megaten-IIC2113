namespace Shin_Megami_Tensei_Model.Skills;

public class SkillEffect
{
    public string TargetName { get; }
    public int DamageDealt { get; }
    public int HealingDone { get; }
    public bool TargetDied { get; }
    public Stats.Affinity AffinityResult { get; }

    public SkillEffect(
        string targetName,
        int damageDealt,
        int healingDone,
        bool targetDied,
        Stats.Affinity affinityResult)
    {
        TargetName = targetName;
        DamageDealt = damageDealt;
        HealingDone = healingDone;
        TargetDied = targetDied;
        AffinityResult = affinityResult;
    }
}