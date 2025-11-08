using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Skills;

public class SkillEffectBuilder
{
    private Unit _target;
    private int _damageDealt;
    private int _healingDone;
    private bool _targetDied;
    private Affinity _affinityResult;
    private int _finalHP;
    private int _maxHP;
    private Element _element;
    private SkillEffectType _effectType;
    private bool _wasRevived;
    private bool _isInstantKill;
    private int _hpDrained;
    private int _mpDrained;
    private int _finalMP;
    private int _maxMP;

    public SkillEffectBuilder()
    {
        _effectType = SkillEffectType.Offensive;
        _affinityResult = Affinity.Neutral;
    }

    public SkillEffectBuilder ForTarget(Unit target)
    {
        _target = target;
        return this;
    }

    public SkillEffectBuilder WithDamage(int damage)
    {
        _damageDealt = damage;
        return this;
    }

    public SkillEffectBuilder WithHealing(int healing)
    {
        _healingDone = healing;
        return this;
    }

    public SkillEffectBuilder TargetDied(bool died)
    {
        _targetDied = died;
        return this;
    }

    public SkillEffectBuilder WithAffinity(Affinity affinity)
    {
        _affinityResult = affinity;
        return this;
    }

    public SkillEffectBuilder WithFinalHP(int finalHP, int maxHP)
    {
        _finalHP = finalHP;
        _maxHP = maxHP;
        return this;
    }

    public SkillEffectBuilder WithElement(Element element)
    {
        _element = element;
        return this;
    }

    public SkillEffectBuilder AsOffensive()
    {
        _effectType = SkillEffectType.Offensive;
        return this;
    }

    public SkillEffectBuilder AsHealing()
    {
        _effectType = SkillEffectType.Healing;
        return this;
    }

    public SkillEffectBuilder AsRevive()
    {
        _effectType = SkillEffectType.Revive;
        _wasRevived = true;
        return this;
    }

    public SkillEffectBuilder AsInstantKill()
    {
        _effectType = SkillEffectType.InstantKill;
        _isInstantKill = true;
        return this;
    }

    public SkillEffectBuilder AsDrainHP()
    {
        _effectType = SkillEffectType.DrainHP;
        return this;
    }

    public SkillEffectBuilder AsDrainMP()
    {
        _effectType = SkillEffectType.DrainMP;
        return this;
    }

    public SkillEffectBuilder AsDrainBoth()
    {
        _effectType = SkillEffectType.DrainBoth;
        return this;
    }

    public SkillEffectBuilder AsHealAndDie()
    {
        _effectType = SkillEffectType.HealAndDie;
        return this;
    }

    public SkillEffectBuilder WithDrainedHP(int hp)
    {
        _hpDrained = hp;
        return this;
    }

    public SkillEffectBuilder WithDrainedMP(int mp)
    {
        _mpDrained = mp;
        return this;
    }

    public SkillEffectBuilder WithFinalMP(int finalMP, int maxMP)
    {
        _finalMP = finalMP;
        _maxMP = maxMP;
        return this;
    }

    public SkillEffectBuilder WasRevived(bool revived)
    {
        _wasRevived = revived;
        return this;
    }

    public SkillEffectBuilder IsInstantKill(bool instantKill)
    {
        _isInstantKill = instantKill;
        return this;
    }

    public SkillEffect Build()
    {
        return new SkillEffect(
            _target,
            _damageDealt,
            _healingDone,
            _targetDied,
            _affinityResult,
            _finalHP,
            _maxHP,
            _element,
            _effectType,
            _wasRevived,
            _isInstantKill,
            _hpDrained,
            _mpDrained,
            _finalMP,
            _maxMP
        );
    }
}
