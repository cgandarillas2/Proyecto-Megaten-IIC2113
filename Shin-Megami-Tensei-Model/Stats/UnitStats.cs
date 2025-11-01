namespace Shin_Megami_Tensei_Model.Stats;

public class UnitStats
{
    private const int MinimumValue = 0;

    public int MaxHP { get; }
    public int CurrentHP { get; private set; }
    public int MaxMP { get; }
    public int CurrentMP { get; private set; }
    public int Str { get; }
    public int Skl { get; }
    public int Mag { get; }
    public int Spd { get; }
    public int Lck { get; }

    public UnitStats(int maxHP, int maxMP, int str, int skl, int mag, int spd, int lck)
        : this(maxHP, maxHP, maxMP, maxMP, str, skl, mag, spd, lck)
    {
    }

    private UnitStats(
        int maxHP,
        int currentHP,
        int maxMP,
        int currentMP,
        int str,
        int skl,
        int mag,
        int spd,
        int lck)
    {
        MaxHP = ValidateStat(maxHP, nameof(maxHP));
        CurrentHP = ValidateCurrentStat(currentHP, maxHP, nameof(currentHP));
        MaxMP = ValidateStat(maxMP, nameof(maxMP));
        CurrentMP = ValidateCurrentStat(currentMP, maxMP, nameof(currentMP));
        Str = ValidateStat(str, nameof(str));
        Skl = ValidateStat(skl, nameof(skl));
        Mag = ValidateStat(mag, nameof(mag));
        Spd = ValidateStat(spd, nameof(spd));
        Lck = ValidateStat(lck, nameof(lck));
    }

    public UnitStats TakeDamage(int damage)
    {
        var newHP = CalculateNewHP(CurrentHP - damage);
        return WithCurrentHP(newHP);
    }

    public UnitStats KillInstantly()
    {
        return WithCurrentHP(0);
    }

    public UnitStats Heal(int amount)
    {
        var newHP = CalculateNewHP(CurrentHP + amount);
        return WithCurrentHP(newHP);
    }

    public UnitStats ConsumeMP(int cost)
    {
        if (CurrentMP < cost)
        {
            throw new InvalidOperationException("Insufficient MP");
        }

        return WithCurrentMP(CurrentMP - cost);
    }

    public UnitStats RestoreMP(int amount)
    {
        var newMP = CalculateNewMP(CurrentMP + amount);
        return WithCurrentMP(newMP);
    }
    
    public void RestoreHP(int amount)
    {
        CurrentHP = Math.Min(MaxHP, amount);
        CurrentHP = Math.Max(0, CurrentHP);
    }

    public bool IsAlive()
    {
        return CurrentHP > MinimumValue;
    }

    public bool HasSufficientMP(int required)
    {
        return CurrentMP >= required;
    }

    private UnitStats WithCurrentHP(int newHP)
    {
        return new UnitStats(MaxHP, newHP, MaxMP, CurrentMP, Str, Skl, Mag, Spd, Lck);
    }

    private UnitStats WithCurrentMP(int newMP)
    {
        return new UnitStats(MaxHP, CurrentHP, MaxMP, newMP, Str, Skl, Mag, Spd, Lck);
    }

    private int CalculateNewHP(int proposedHP)
    {
        return Math.Max(MinimumValue, Math.Min(MaxHP, proposedHP));
    }

    private int CalculateNewMP(int proposedMP)
    {
        return Math.Max(MinimumValue, Math.Min(MaxMP, proposedMP));
    }

    private static int ValidateStat(int value, string paramName)
    {
        if (value < MinimumValue)
        {
            return MinimumValue;
        }
        return value;
    }

    private static int ValidateCurrentStat(int current, int max, string paramName)
    {
        if (current < MinimumValue || current > max)
        {
            return MinimumValue;
        }
        return current;
    }

}