using Shin_Megami_Tensei_Model.Stats;

namespace Shin_Megami_Tensei_Model.Action
{
    public class ActionResult
    {
        public bool Success { get; }
        public bool GameEnded { get; }
        public string WinnerName { get; }
        public TurnConsumption TurnConsumption { get; }
        public int Damage { get; }
        public Affinity AffinityResult { get; }

        private ActionResult(
            bool success,
            bool gameEnded,
            string winnerName,
            TurnConsumption turnConsumption,
            int damage,
            Affinity affinityResult)
        {
            Success = success;
            GameEnded = gameEnded;
            WinnerName = winnerName;
            TurnConsumption = turnConsumption;
            Damage = damage;
            AffinityResult = affinityResult;
        }

        public static ActionResult Successful(TurnConsumption turnConsumption, int damage, Affinity affinity)
        {
            return new ActionResult(true, false, null, turnConsumption, damage, affinity);
        }

        public static ActionResult Failed()
        {
            return new ActionResult(false, false, null, null, 0, Affinity.Neutral);
        }

        public static ActionResult GameOver(string winnerName)
        {
            return new ActionResult(true, true, winnerName, null, 0, Affinity.Neutral);
        }
    }
}