namespace Shin_Megami_Tensei_Model.Action;

public abstract class TurnConsumptionStrategy
{
    public abstract TurnConsumptionResult Apply(int fullTurns, int blinkingTurns);
}