using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Skills;

public class BaseSkill: ISkill
{
    public string Name { get; }
    public int Cost { get; }
    public TargetType TargetType { get; }
    public Element Element { get; }

    public BaseSkill(string name, int cost)
    {
        Name = name;
        Cost = cost;
    }
    
    public bool CanExecute(Unit user, GameState gameState)
    {
        throw new NotImplementedException();
    }

    public SkillResult Execute(Unit user, List<Unit> targets, GameState gameState)
    {
        throw new NotImplementedException();
    }
}