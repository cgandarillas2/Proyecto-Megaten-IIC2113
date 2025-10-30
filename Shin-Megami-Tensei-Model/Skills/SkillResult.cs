using Shin_Megami_Tensei_Model.Action;

namespace Shin_Megami_Tensei_Model.Skills;

public class SkillResult
{
    public List<SkillEffect> Effects { get; }
    public TurnConsumption TurnConsumption { get; }
    public List<string> Messages { get; }

    public SkillResult(
        List<SkillEffect> effects,
        TurnConsumption turnConsumption,
        List<string> messages)
    {
        Effects = effects ?? new List<SkillEffect>();
        TurnConsumption = turnConsumption;
        Messages = messages ?? new List<string>();
    }

    public static SkillResult Empty()
    {
        return new SkillResult(
            new List<SkillEffect>(),
            TurnConsumption.NonOffensiveSkill(),
            new List<string>()
        );
    }
}