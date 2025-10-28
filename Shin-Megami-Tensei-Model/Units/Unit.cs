using Shin_Megami_Tensei_Model.Utils;
using Shin_Megami_Tensei_Model.Stats;


namespace Shin_Megami_Tensei_Model.Units;

public abstract class Unit
{
    public string Name { get; }
    public UnitStats CurrentStats { get; protected set; }
    public AffinitySet Affinities { get; protected set; }

    protected Unit(string name, UnitStats baseStats, AffinitySet affinities)
    {
        Name = ValidateName(name);
        CurrentStats = baseStats ?? throw new ArgumentNullException(nameof(baseStats));
        Affinities = affinities ?? throw new ArgumentNullException(nameof(affinities));
    }
    
    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty");
        }
        return name;
    }
    
    public bool IsAlive()
    {
        return CurrentStats.IsAlive();
    }
    
    public virtual bool IsEmpty()
    {
        return false;
    }
    
    public void TakeDamage(int damage)
    {
        CurrentStats = CurrentStats.TakeDamage(damage);
    }

    public void Heal(int amount)
    {
        CurrentStats = CurrentStats.Heal(amount);
    }

    public void ConsumeMP(int cost)
    {
        CurrentStats = CurrentStats.ConsumeMP(cost);
    }
    
    public void ChangeAffinity(Element element, Affinity newAffinity)
    {
        Affinities = Affinities.ChangeAffinity(element, newAffinity);
    }
    
}