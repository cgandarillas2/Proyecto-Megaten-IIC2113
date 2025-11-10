namespace Shin_Megami_Tensei_Model.Action.Strategies;

/// Consume 2 Blinking Turns.
/// Si no hay suficientes, consume lo que falte en Full Turns.
public class NullStrategy: TurnConsumptionStrategy
{
    public override TurnConsumptionResult Apply(int fullTurns, int blinkingTurns)
    {
        var fullTurnsUsed = Math.Min(fullTurns, 2);
        
        if (blinkingTurns >= 2)
        {
            return new TurnConsumptionResult(0, 2, 0);
        }
        
        if (blinkingTurns == 1)
        {
            return new TurnConsumptionResult(Math.Min(fullTurnsUsed, 1), 1, 0);
        }

        return new TurnConsumptionResult(fullTurnsUsed, 0, 0);
    }
}