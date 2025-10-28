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
            "Bind" => Element.Bind,
            "Sleep" => Element.Sleep,
            "Panic" => Element.Panic,
            "Poison" => Element.Poison,
            "Sick" => Element.Sick,
            _ => throw new ArgumentException($"Unknown element: {elementName}")
        };
    }
    
    public AffinitySet ParseAffinitySet(Dictionary<string, string> affinityData)
    {
        var affinities = new Dictionary<Element, Affinity>();

        // Parse affinities from JSON
        foreach (var kvp in affinityData)
        {
            var element = ParseElement(kvp.Key);
            var affinity = ParseAffinityCode(kvp.Value);
            affinities[element] = affinity;
        }

        // Fill missing elements with Neutral affinity
        foreach (Element element in Enum.GetValues(typeof(Element)))
        {
            if (!affinities.ContainsKey(element))
            {
                affinities[element] = Affinity.Neutral;
            }
        }

        return AffinitySet.Create(affinities);
    }
}