using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Combat;

public class DamageCalculator
{
    private const double AttackModifier = 54.0;
    private const double ShootModifier = 80.0;
    private const double BaseMultiplier = 0.0114;

    public int CalculateAttackDamage(Unit attacker)
    {
        var str = attacker.CurrentStats.Str;
        var damage = str * AttackModifier * BaseMultiplier;
        return TruncateToInteger(damage);
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
}