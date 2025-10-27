using Shin_Megami_Tensei_Model.Stats;

namespace Shin_Megami_Tensei_Model.Repositories.Parsers;

public class AffinityParser
{
    public Affinity ParseAffinityCode(string code)
    {
        return code switch
        {
            "-" => Affinity.Neutral,
            "Wk" => Affinity.Weak,
            "Rs" => Affinity.Resist,
            "Nu" => Affinity.Null,
            "Rp" => Affinity.Repel,
            "Dr" => Affinity.Drain,
            _ => throw new ArgumentException($"Unknown affinity code: {code}")
        };
    }
    
    public Element ParseElement(string elementName)
    {
        return elementName switch
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
            _ => throw new ArgumentException($"Unknown element: {elementName}")
        };
    }
    
    public AffinitySet ParseAffinitySet(Dictionary<string, string> affinityData)
    {
        var affinities = new Dictionary<Element, Affinity>();

        foreach (var kvp in affinityData)
        {
            var element = ParseElement(kvp.Key);
            var affinity = ParseAffinityCode(kvp.Value);
            affinities[element] = affinity;
        }

        return AffinitySet.Create(affinities);
    }
}