namespace Shin_Megami_Tensei_Model.Stats;

public class UnitBuffState
{
    private const int MinGrade = -3;
    private const int MaxGrade = 3;

    public bool IsPhysicalCharged { get; }
    public bool IsMagicalCharged { get; }
    public int OffensiveGrade { get; }
    public int DefensiveGrade { get; }

    public UnitBuffState()
        : this(false, false, 0, 0)
    {
    }

    private UnitBuffState(
        bool isPhysicalCharged,
        bool isMagicalCharged,
        int offensiveGrade,
        int defensiveGrade)
    {
        IsPhysicalCharged = isPhysicalCharged;
        IsMagicalCharged = isMagicalCharged;
        OffensiveGrade = ClampGrade(offensiveGrade);
        DefensiveGrade = ClampGrade(defensiveGrade);
    }

    public UnitBuffState WithPhysicalCharge()
    {
        return new UnitBuffState(true, IsMagicalCharged, OffensiveGrade, DefensiveGrade);
    }

    public UnitBuffState WithMagicalCharge()
    {
        return new UnitBuffState(IsPhysicalCharged, true, OffensiveGrade, DefensiveGrade);
    }

    public UnitBuffState WithoutPhysicalCharge()
    {
        return new UnitBuffState(false, IsMagicalCharged, OffensiveGrade, DefensiveGrade);
    }

    public UnitBuffState WithoutMagicalCharge()
    {
        return new UnitBuffState(IsPhysicalCharged, false, OffensiveGrade, DefensiveGrade);
    }

    public UnitBuffState WithOffensiveGradeIncrease()
    {
        return new UnitBuffState(
            IsPhysicalCharged,
            IsMagicalCharged,
            OffensiveGrade + 1,
            DefensiveGrade);
    }

    public UnitBuffState WithDefensiveGradeIncrease()
    {
        return new UnitBuffState(
            IsPhysicalCharged,
            IsMagicalCharged,
            OffensiveGrade,
            DefensiveGrade + 1);
    }

    public double GetOffensiveMultiplier()
    {
        return OffensiveGrade switch
        {
            -3 => 0.625,
            -2 => 0.75,
            -1 => 0.875,
            0 => 1.0,
            1 => 1.25,
            2 => 1.5,
            3 => 1.75,
            _ => 1.0
        };
    }

    public double GetDefensiveMultiplier()
    {
        return DefensiveGrade switch
        {
            -3 => 1.75,
            -2 => 1.5,
            -1 => 1.25,
            0 => 1.0,
            1 => 0.875,
            2 => 0.75,
            3 => 0.625,
            _ => 1.0
        };
    }

    private static int ClampGrade(int grade)
    {
        return Math.Max(MinGrade, Math.Min(MaxGrade, grade));
    }
}
