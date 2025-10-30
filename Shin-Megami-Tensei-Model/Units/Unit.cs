using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Utils;
using Shin_Megami_Tensei_Model.Stats;


namespace Shin_Megami_Tensei_Model.Units;

public abstract class Unit
{
    public string Name { get; }
    public UnitStats CurrentStats { get; protected set; }
    public AffinitySet Affinities { get; private set; }

    protected Unit(string name, UnitStats baseStats, AffinitySet affinities)
    {
        Name = ValidateName(name);
        CurrentStats = baseStats ?? throw new ArgumentNullException(nameof(baseStats));
        Affinities = affinities ?? throw new ArgumentNullException(nameof(affinities));
    }

    public virtual bool IsAlive()
    {
        return CurrentStats.IsAlive();
    }

    public virtual bool IsEmpty()
    {
        return false;
    }

    public abstract List<ISkill> GetSkills();

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
        Affinities = Affinities.WithAffinity(element, newAffinity);
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty");
        }
        return name;
    }

    public void KillInstantly()
    {
        CurrentStats = CurrentStats.KillInstantly();
    }
    
}