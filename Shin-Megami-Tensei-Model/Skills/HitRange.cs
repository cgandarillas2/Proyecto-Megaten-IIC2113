namespace Shin_Megami_Tensei_Model.Skills;

public class HitRange
{
    public int Minimum { get; }
    public int Maximum { get; }
    public bool IsFixed { get; }

    private HitRange(int minimum, int maximum)
    {
        Minimum = minimum;
        Maximum = maximum;
        IsFixed = minimum == maximum;
    }

    public static HitRange Fixed(int hits)
    {
        return new HitRange(hits, hits);
    }

    public static HitRange Variable(int min, int max)
    {
        if (min > max)
        {
            throw new ArgumentException("Minimum cannot be greater than maximum");
        }
        return new HitRange(min, max);
    }

    public int CalculateHits(int skillCount, TargetType targetType)
    {
        if (targetType == TargetType.Multi)
            return 1;

        return CalculateHits(skillCount);
    }

    public int CalculateHits(int skillCount)
    {
        if (IsFixed)
        {
            return Minimum;
        }

        var offset = skillCount % (Maximum - Minimum + 1);
        return Minimum + offset;
    }
    
    
}