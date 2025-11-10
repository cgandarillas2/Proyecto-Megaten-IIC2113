namespace Shin_Megami_Tensei_Model.Action.Strategies;

/// Consume 1 Blinking Turn.
/// Si no hay, consume 1 Full Turn.
/// Usado para: Neutral, Resist, Miss, Non-Offensive Skills
public class ConsumeOneBlinkingStrategy: TurnConsumptionStrategy
{
    public override TurnConsumptionResult Apply(int fullTurns, int blinkingTurns)
    {
        if (blinkingTurns > 0)
        {
            return new TurnConsumptionResult(0, 1, 0);
        }

        return new TurnConsumptionResult(1, 0, 0);
    }
}