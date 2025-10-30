namespace Shin_Megami_Tensei_Model.Stats;

public class AffinitySet
{
    private readonly Dictionary<Element, Affinity> _affinities;

    private AffinitySet(Dictionary<Element, Affinity> affinities)
    {
        _affinities = new Dictionary<Element, Affinity>(affinities);
    }

    public static AffinitySet Create(Dictionary<Element, Affinity> affinities)
    {
        ValidateAllElementsPresent(affinities);
        return new AffinitySet(affinities);
    }
    
    public static AffinitySet CreateAllNeutral()
    {
        var affinities = new Dictionary<Element, Affinity>();
            
        foreach (Element element in Enum.GetValues(typeof(Element)))
        {
            affinities[element] = Affinity.Neutral;
        }
            
        return new AffinitySet(affinities);
    }

    public Affinity GetAffinity(Element element)
    {
        return _affinities[element];
    }
    
    public AffinitySet ChangeAffinity(Element element, Affinity newAffinity)
    {
        var newAffinities = new Dictionary<Element, Affinity>(_affinities)
        {
            [element] = newAffinity
        };
        return new AffinitySet(newAffinities);
    }

    private static void ValidateAllElementsPresent(Dictionary<Element, Affinity> affinities)
    {
        if (affinities == null)
        {
            throw new ArgumentNullException(nameof(affinities));
        }

        foreach (Element element in Enum.GetValues(typeof(Element)))
        {
            if (!affinities.ContainsKey(element))
            {
                throw new ArgumentException($"Missing element: {element}");
            }
        }
    }

    public AffinitySet WithAffinity(Element element, Affinity newAffinity)
    {
        var newAffinities = new Dictionary<Element, Affinity>(_affinities)
        {
            [element] = newAffinity
        };
        return new AffinitySet(newAffinities);
    }
}