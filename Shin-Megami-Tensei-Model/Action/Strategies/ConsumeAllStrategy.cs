namespace Shin_Megami_Tensei_Model.Action.Strategies;

public class ConsumeAllStrategy: TurnConsumptionStrategy
{
    public override TurnConsumptionResult Apply(int fullTurns, int blinkingTurns)
    {
        return new TurnConsumptionResult(fullTurns, blinkingTurns, 0, consumedAll: true);
    }
}