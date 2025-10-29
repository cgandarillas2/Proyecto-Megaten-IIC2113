namespace Shin_Megami_Tensei_Model.Action;

public class TurnConsumption
{
    public int FullTurnsToConsume { get; }
    public int BlinkingTurnsToConsume { get; }
    public int BlinkingTurnsToGain { get; }
    public bool ConsumeAll { get; }

    private TurnConsumption(
        int fullTurnsToConsume,
        int blinkingTurnsToConsume,
        int blinkingTurnsToGain,
        bool consumeAll)
    {
        FullTurnsToConsume = fullTurnsToConsume;
        BlinkingTurnsToConsume = blinkingTurnsToConsume;
        BlinkingTurnsToGain = blinkingTurnsToGain;
        ConsumeAll = consumeAll;
    }

    // REFACTORIZAR CON ALGÚN PATRON
    
    // Consume 1 Blinking (si no hay, consume 1 Full)
    public static TurnConsumption NeutralOrResist()
    {
        return new TurnConsumption(0, 1, 0, false);
    }

    // Consume 1 Full Turn y otorga 1 Blinking Turn
    public static TurnConsumption Weak()
    {
        return new TurnConsumption(1, 0, 1, false);
    }

    // Consume 2 Blinking (si no hay suficientes, consume Full)
    public static TurnConsumption Null()
    {
        return new TurnConsumption(0, 2, 0, false);
    }

    // Consume 1 Blinking (si no hay, consume 1 Full)
    public static TurnConsumption Miss()
    {
        return new TurnConsumption(0, 1, 0, false);
    }

    // Consume TODOS los turnos
    public static TurnConsumption RepelOrDrain()
    {
        return new TurnConsumption(0, 0, 0, true);
    }

    // Consume 1 Blinking (si no hay, consume 1 Full y otorga 1 Blinking)
    public static TurnConsumption PassOrSummon()
    {
        return new TurnConsumption(0, 1, 1, false);
    }

    // Para habilidades no ofensivas (E2+)
    public static TurnConsumption NonOffensiveSkill()
    {
        return new TurnConsumption(0, 1, 0, false);
    }
}