namespace Shin_Megami_Tensei_Model.Action.Strategies;

/// <summary>
/// Consume 1 Full Turn y otorga 1 Blinking Turn.
/// Si no hay Full, consume 1 Blinking Turn.
/// </summary>
public class WeakStrategy: TurnConsumptionStrategy
{
    public override TurnConsumptionResult Apply(int fullTurns, int blinkingTurns)
    {
        if (fullTurns > 0)
        {
            return new TurnConsumptionResult(1, 0, 1);
        }

        return new TurnConsumptionResult(0, 1, 0);
    }
}