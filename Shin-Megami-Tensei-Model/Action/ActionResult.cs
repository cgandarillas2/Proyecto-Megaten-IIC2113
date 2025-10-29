namespace Shin_Megami_Tensei_Model.Action;

public class ActionResult
{
    public bool Success { get; }
    public bool GameEnded { get; }
    public string WinnerName { get; }
    public TurnConsumption TurnConsumption { get; }
    public int Damage { get; }

    private ActionResult(
        bool success,
        bool gameEnded,
        string winnerName,
        TurnConsumption turnConsumption,
        int damage)
    {
        Success = success;
        GameEnded = gameEnded;
        WinnerName = winnerName;
        TurnConsumption = turnConsumption;
        Damage = damage;
    }

    public static ActionResult Successful(TurnConsumption turnConsumption, int damage)
    {
        return new ActionResult(true, false, null, turnConsumption, damage);
    }

    public static ActionResult Failed()
    {
        return new ActionResult(false, false, null, null, 0);
    }

    public static ActionResult GameOver(string winnerName)
    {
        return new ActionResult(true, true, winnerName, null, 0);
    }
}