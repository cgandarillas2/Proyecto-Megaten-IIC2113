using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Collections;

namespace Shin_Megami_Tensei_Model.Skills;

public class SkillResult
{
    public SkillEffectsCollection Effects { get; }
    public TurnConsumption TurnConsumption { get; }
    public StringCollection Messages { get; }

    public SkillResult(
        SkillEffectsCollection effects,
        TurnConsumption turnConsumption,
        StringCollection messages)
    {
        Effects = effects;
        TurnConsumption = turnConsumption;
        Messages = messages;
    }
}