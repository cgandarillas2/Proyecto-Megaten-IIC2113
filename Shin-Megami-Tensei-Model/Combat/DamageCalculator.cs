using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Combat;

public class DamageCalculator
{
    private const double AttackModifier = 54.0;
    private const double ShootModifier = 80.0;
    private const double BaseMultiplier = 0.0114;
    private const double ChargeMultiplier = 2.5;

    public double CalculateAttackDamage(Unit attacker)
    {
        var str = attacker.CurrentStats.Str;
        var damage = str * AttackModifier * BaseMultiplier;
        return damage;
    }

    public double CalculateShootDamage(Unit attacker)
    {
        var skl = attacker.CurrentStats.Skl;
        var damage = skl * ShootModifier * BaseMultiplier;
        return damage;
    }

    public int CalculateInstantKillDamage(Unit target)
    {
        return target.CurrentStats.CurrentHP;
    }

    public double CalculateMagicalDamage(Unit attacker, double skillPower)
    {
        var mag = attacker.CurrentStats.Mag;
        var damage = Math.Sqrt(mag * skillPower);
        return damage;
    }

    public double ApplyBuffMultipliers(
        double baseDamage,
        Unit attacker,
        Unit target,
        Element element,
        Affinity affinity,
        bool isFirstTarget)
    {
        if (affinity == Affinity.Drain || affinity == Affinity.Repel)
        {
            return baseDamage;
        }

        var damage = baseDamage;

        damage = ApplyChargeMultiplier(damage, attacker, element, isFirstTarget);
        damage = ApplyOffensiveGradeMultiplier(damage, attacker);
        damage = ApplyDefensiveGradeMultiplier(damage, target);

        return damage;
    }

    private double ApplyChargeMultiplier(
        double damage,
        Unit attacker,
        Element element,
        bool isFirstTarget)
    {
        if (!isFirstTarget)
        {
            return damage;
        }

        var isPhysicalAttack = element == Element.Phys || element == Element.Gun;
        if (isPhysicalAttack && attacker.BuffState.IsPhysicalCharged)
        {
            return damage * ChargeMultiplier;
        }

        var isMagicalAttack = element == Element.Fire || element == Element.Ice ||
                             element == Element.Elec || element == Element.Force ||
                             element == Element.Almighty;
        if (isMagicalAttack && attacker.BuffState.IsMagicalCharged)
        {
            return damage * ChargeMultiplier;
        }

        return damage;
    }

    private double ApplyOffensiveGradeMultiplier(double damage, Unit attacker)
    {
        return damage * attacker.BuffState.GetOffensiveMultiplier();
    }

    private double ApplyDefensiveGradeMultiplier(double damage, Unit target)
    {
        return damage * target.BuffState.GetDefensiveMultiplier();
    }
}