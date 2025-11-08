using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Skills;

public class SkillEffect
{
    public Unit Target { get; }
    public int DamageDealt { get; }
    public int HealingDone { get; }
    public bool TargetDied { get; }
    public bool WasRevived { get; }
    public bool IsInstantKill { get; }
    public Affinity AffinityResult { get; }
    public int FinalHP { get; }
    public int MaxHP { get; }
    public Element Element { get; }
    public SkillEffectType EffectType { get; }
    public int HPDrained { get; }
    public int MPDrained { get; }
    public int FinalMP { get; }
    public int MaxMP { get; }

    public SkillEffect(
        Unit target,
        int damageDealt,
        int healingDone,
        bool targetDied,
        Affinity affinityResult,
        int finalHP,
        int maxHP,
        Element element,
        SkillEffectType effectType = SkillEffectType.Offensive,
        bool wasRevived = false,
        bool isIntantKill = false,
        int hpDrained = 0,
        int mpDrained = 0,
        int finalMp = 0,
        int maxMp = 0)
    {
        Target = target;
        DamageDealt = damageDealt;
        HealingDone = healingDone;
        TargetDied = targetDied;
        AffinityResult = affinityResult;
        FinalHP = finalHP;
        MaxHP = maxHP;
        Element = element;
        EffectType = effectType;
        WasRevived = wasRevived;
        IsInstantKill = isIntantKill;
        HPDrained = hpDrained;
        MPDrained = mpDrained;
        FinalMP = finalMp;
        MaxMP = maxMp;
    }

    public bool IsHealEffect()
    {
        return EffectType == SkillEffectType.Healing;
    }

    public bool IsDrainHealEffect()
    {
        return EffectType == SkillEffectType.HealAndDie;
    }

    public bool IsReviveEffect()
    {
        return EffectType == SkillEffectType.Revive;
    }

    public bool IsDrainEffect()
    {
        var isDrainEffect = EffectType == SkillEffectType.DrainHP || 
            EffectType == SkillEffectType.DrainMP || 
            EffectType == SkillEffectType.DrainBoth;
        return isDrainEffect;
    }

    public bool IsOffensiveEffect()
    {
        return EffectType == SkillEffectType.Offensive;
    }
}