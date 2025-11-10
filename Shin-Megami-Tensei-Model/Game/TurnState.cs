using Shin_Megami_Tensei_Model.Action;

namespace Shin_Megami_Tensei_Model.Game;

public class TurnState
{
    public int FullTurns { get; private set; }
    public int BlinkingTurns { get; private set; }

    public TurnState(int fullTurns)
    {
        FullTurns = fullTurns;
        BlinkingTurns = 0;
    }

    public bool HasTurns()
    {
        return FullTurns > 0 || BlinkingTurns > 0;
    }

    public TurnConsumptionResult ApplyConsumption(TurnConsumption consumption)
    {
        var result = consumption.Strategy.Apply(FullTurns, BlinkingTurns);

        if (result.ConsumedAll)
        {
            FullTurns = 0;
            BlinkingTurns = 0;
        }
        else
        {
            FullTurns -= result.FullTurnsConsumed;
            BlinkingTurns -= result.BlinkingTurnsConsumed;
            BlinkingTurns += result.BlinkingTurnsGained;
        }

        return result;
    }
}