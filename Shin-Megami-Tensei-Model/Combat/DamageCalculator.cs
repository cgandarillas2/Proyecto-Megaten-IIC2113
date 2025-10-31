using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Combat;

public class DamageCalculator
{
    private const double AttackModifier = 54.0;
    private const double ShootModifier = 80.0;
    private const double BaseMultiplier = 0.0114;

    public double CalculateAttackDamage(Unit attacker)
    {
        var str = attacker.CurrentStats.Str;
        var damage = str * AttackModifier * BaseMultiplier;
        return damage;
    }

    public int CalculateShootDamage(Unit attacker)
    {
        var skl = attacker.CurrentStats.Skl;
        var damage = skl * ShootModifier * BaseMultiplier;
        return TruncateToInteger(damage);
    }

    private static int TruncateToInteger(double value)
    {
        return (int)Math.Floor(value);
    }

    public double CalculateMagicalDamage(Unit attacker, int skillPower)
    {
        var mag = attacker.CurrentStats.Mag;
        var damage = Math.Sqrt(mag * skillPower);
        return damage;
    }
}