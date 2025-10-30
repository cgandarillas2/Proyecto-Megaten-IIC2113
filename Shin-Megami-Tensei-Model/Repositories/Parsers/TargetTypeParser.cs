using Shin_Megami_Tensei_Model.Skills;

namespace Shin_Megami_Tensei_Model.Repositories.Parsers;

public class TargetTypeParser
{
    public TargetType ParseTarget(string target)
    {
        return target switch
        {
            "Single" => TargetType.Single,
            "All" => TargetType.All,
            "Multi" => TargetType.Multi,
            "Ally" => TargetType.Ally,
            "Party" => TargetType.Party,
            "Self" => TargetType.Self,
            "Universal" => TargetType.Universal,
            _ => throw new ArgumentException($"Unknown target type: {target}")
        };
    }
}