namespace Shin_Megami_Tensei_Model.Action.Strategies;

/// <summary>
/// Consume 2 Blinking Turns.
/// Si no hay suficientes, consume lo que falte en Full Turns.
/// </summary>
public class NullStrategy: TurnConsumptionStrategy
{
    public override TurnConsumptionResult Apply(int fullTurns, int blinkingTurns)
    {
        
        
        if (blinkingTurns >= 2)
        {
            return new TurnConsumptionResult(0, 2, 0);
        }
        
        if (blinkingTurns == 1)
        {
            return new TurnConsumptionResult(1, 1, 0);
        }

        var fullTurnsUsed = Math.Min(fullTurns, 2);
        return new TurnConsumptionResult(fullTurnsUsed, 0, 0);
        
    }
}