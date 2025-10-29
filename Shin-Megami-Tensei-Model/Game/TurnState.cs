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
    

    public void ConsumeFullTurn()
    {
        if (FullTurns > 0)
        {
            FullTurns--;
        }
    }

    public void AddBlinkingTurns(int amount)
    {
        BlinkingTurns += amount;
    }

    public void ConsumeAllTurns()
    {
        FullTurns = 0;
        BlinkingTurns = 0;
    }
    
    public void ConsumeTurns(int blinkingToConsume, int fullToConsume, int blinkingToGain)
    {
        // Primero intentar consumir Blinking Turns
        var blinkingConsumed = 0;
        while (blinkingConsumed < blinkingToConsume && BlinkingTurns > 0)
        {
            BlinkingTurns--;
            blinkingConsumed++;
        }

        // Si faltan Blinking Turns por consumir, consumir Full Turns
        var remainingBlinking = blinkingToConsume - blinkingConsumed;
        var totalFullToConsume = fullToConsume + remainingBlinking;

        while (totalFullToConsume > 0 && FullTurns > 0)
        {
            FullTurns--;
            totalFullToConsume--;
        }

        // Finalmente, agregar Blinking Turns ganados
        BlinkingTurns += blinkingToGain;
    }
}