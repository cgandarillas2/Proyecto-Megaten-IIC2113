using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Skills;

public interface ISkill
{
    string Name { get; }
    int Cost { get; }
    TargetType TargetType { get; }
    Element Element { get; }
        
    bool CanExecute(Unit user, GameState gameState);
    SkillResult Execute(Unit user, List<Unit> targets, GameState gameState);
}