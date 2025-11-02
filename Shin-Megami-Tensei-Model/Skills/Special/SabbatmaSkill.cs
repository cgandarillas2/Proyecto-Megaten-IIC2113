using Shin_Megami_Tensei_Model.Action;
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
        user.ConsumeMP(Cost);

        var effects = new List<SkillEffect>();

        foreach (var target in targets)
        {
            var effect = ExecuteSabbatma(target);
            effects.Add(effect);
        }
        
        var turnConsumption = TurnConsumption.NonOffensiveSkill();
        return new SkillResult(effects, turnConsumption, new List<string>());
    }
    
    private SkillEffect ExecuteSabbatma(Unit target)
    {

        return new SkillEffect(
            target.Name,
            0,
            0,
            false,
            Affinity.Neutral,
            target.CurrentStats.CurrentHP,
            target.CurrentStats.MaxHP,
            Element.Almighty,
            SkillEffectType.Revive
        );
    }
    
    private bool HasAliveMonstersToSummon(GameState gameState)
    {
        // Sabbatma solo puede invocar monstruos VIVOS
        return gameState.CurrentPlayer.GetAliveReserveMonsters().Any();
    }
    
    public List<Monster> GetMostersFromReserve(GameState gameState)
    {
        return gameState.CurrentPlayer.GetAliveReserveMonsters();
    }
}