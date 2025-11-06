using Shin_Megami_Tensei_Model.Stats;

namespace Shin_Megami_Tensei_Model.Repositories.Parsers;

public class SkillTypeParser
{
    public Element ParseElement(string type)
    {
        return type switch
        {
            "Phys" => Element.Phys,
            "Gun" => Element.Gun,
            "Fire" => Element.Fire,
            "Ice" => Element.Ice,
            "Elec" => Element.Elec,
            "Force" => Element.Force,
            "Light" => Element.Light,
            "Dark" => Element.Dark,
            "Almighty" => Element.Almighty,
            "Passive" => Element.Passive,
            _ => Element.Phys
            /*_ => throw new ArgumentException($"Unknown skill type: {type}")*/
        };
    }

    public bool IsOffensiveType(string type)
    {
        return type switch
        {
            "Phys" => true,
            "Gun" => true,
            "Fire" => true,
            "Ice" => true,
            "Elec" => true,
            "Force" => true,
            "Almighty" => true,
            _ => false
        };
    }

    public bool IsInstantKillType(string type)
    {
        return type switch
        {
            "Light" => true,
            "Dark" => true,
            _ => false
        };
    }

    public bool IsHealType(string type)
    {
        return type == "Heal";
    }

    public bool IsSupportType(string type)
    {
        return type == "Support";
    }
    
    public bool IsSpecialType(string type)
    {
        return type == "Special";
    }
    
    
}