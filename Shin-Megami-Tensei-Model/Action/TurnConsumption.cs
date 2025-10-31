using Shin_Megami_Tensei_Model.Action.Strategies;

namespace Shin_Megami_Tensei_Model.Action;

public class TurnConsumption
{
    public TurnConsumptionStrategy Strategy { get; }

    private TurnConsumption(TurnConsumptionStrategy strategy)
    {
        Strategy = strategy;
    }

    public static TurnConsumption NeutralOrResist()
    {
        return new TurnConsumption(new ConsumeOneBlinkingStrategy());
    }

    public static TurnConsumption Weak()
    {
        return new TurnConsumption(new WeakStrategy());
    }

    public static TurnConsumption Null()
    {
        return new TurnConsumption(new NullStrategy());
    }

    public static TurnConsumption Miss()
    {
        return new TurnConsumption(new ConsumeOneBlinkingStrategy());
    }

    public static TurnConsumption RepelOrDrain()
    {
        return new TurnConsumption(new ConsumeAllStrategy());
    }

    public static TurnConsumption PassOrSummon()
    {
        return new TurnConsumption(new PassOrSummonStrategy());
    }

    public static TurnConsumption NonOffensiveSkill()
    {
        return new TurnConsumption(new ConsumeOneBlinkingStrategy());
    }
}