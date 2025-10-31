using Shin_Megami_Tensei_Model.Stats;

namespace Shin_Megami_Tensei_Model.Skills;

public class SkillEffect
{
    public string TargetName { get; }
    public int DamageDealt { get; }
    public int HealingDone { get; }
    public bool TargetDied { get; }
    public Affinity AffinityResult { get; }
    public int FinalHP { get; }
    public int MaxHP { get; }
    public Element Element { get; }

    public SkillEffect(
        string targetName,
        int damageDealt,
        int healingDone,
        bool targetDied,
        Affinity affinityResult,
        int finalHP,
        int maxHP,
        Element element)
    {
        TargetName = targetName;
        DamageDealt = damageDealt;
        HealingDone = healingDone;
        TargetDied = targetDied;
        AffinityResult = affinityResult;
        FinalHP = finalHP;
        MaxHP = maxHP;
        Element = element;
    }
}