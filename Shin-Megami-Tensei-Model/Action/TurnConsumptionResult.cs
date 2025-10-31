namespace Shin_Megami_Tensei_Model.Action;

public class TurnConsumptionResult
{
    public int FullTurnsConsumed { get; }
    public int BlinkingTurnsConsumed { get; }
    public int BlinkingTurnsGained { get; }
    public bool ConsumedAll { get; }

    public TurnConsumptionResult(
        int fullTurnsConsumed,
        int blinkingTurnsConsumed,
        int blinkingTurnsGained,
        bool consumedAll = false)
    {
        FullTurnsConsumed = fullTurnsConsumed;
        BlinkingTurnsConsumed = blinkingTurnsConsumed;
        BlinkingTurnsGained = blinkingTurnsGained;
        ConsumedAll = consumedAll;
    }
}