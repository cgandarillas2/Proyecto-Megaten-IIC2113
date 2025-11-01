using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Skills.Special;

public class InvitationSkill: ISkill
{
    public string Name { get; }
    public int Cost { get; }
    public TargetType TargetType { get; }
    public Element Element => Element.Almighty;

    public InvitationSkill(string name, int cost)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Cost = cost;
        TargetType = TargetType.Ally;
    }

    public bool CanExecute(Unit user, GameState gameState)
    {
        bool isAlive = user.IsAlive();
        bool hasSufficientMP = user.CurrentStats.HasSufficientMP(Cost);
        bool hasMonstersReserve = HasMonstersToSummon(gameState);
        return isAlive && hasSufficientMP && hasMonstersReserve;
    }

    public SkillResult Execute(Unit user, List<Unit> targets, GameState gameState)
    {
        throw new NotImplementedException();
    }
    
    private bool HasMonstersToSummon(GameState gameState)
    {
        // Invitation puede invocar monstruos vivos o muertos
        return gameState.CurrentPlayer.GetAllReserveMonsters().Any();
    }
}