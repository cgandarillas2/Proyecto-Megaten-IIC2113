using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Skills.Special;

public class SabbatmaSkill: ISkill
{
    public string Name { get; }
    public int Cost { get; }
    public TargetType TargetType { get; }
    public Element Element => Element.Almighty;
    
    public SabbatmaSkill(string name, int cost)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Cost = cost;
        TargetType = TargetType.Ally;
    }
    public bool CanExecute(Unit user, GameState gameState)
    {
        bool isAlive = user.IsAlive();
        bool hasSufficientMP = user.CurrentStats.HasSufficientMP(Cost);
        bool hasMonstersAliveReserve = HasAliveMonstersToSummon(gameState);
        return isAlive && hasSufficientMP && hasMonstersAliveReserve;
    }

    public SkillResult Execute(Unit user, List<Unit> targets, GameState gameState)
    {
        throw new InvalidOperationException("Sabbatma skill should be handled by controller");
    }
    
    private bool HasAliveMonstersToSummon(GameState gameState)
    {
        // Sabbatma solo puede invocar monstruos VIVOS
        return gameState.CurrentPlayer.GetAliveReserveMonsters().Any();
    }
}