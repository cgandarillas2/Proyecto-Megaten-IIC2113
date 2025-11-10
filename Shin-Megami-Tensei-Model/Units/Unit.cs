using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Stats;

namespace Shin_Megami_Tensei_Model.Units;

public abstract class Unit
{
    public string Name { get; }
    public UnitStats CurrentStats { get; protected set; }
    public AffinitySet Affinities { get; private set; }
    public UnitBuffState BuffState { get; private set; }

    protected Unit(string name, UnitStats baseStats, AffinitySet affinities)
    {
        Name = ValidateName(name);
        CurrentStats = baseStats;
        Affinities = affinities;
        BuffState = new UnitBuffState();
    }

    public virtual bool IsAlive()
    {
        return CurrentStats.IsAlive();
    }

    public virtual bool IsEmpty()
    {
        return false;
    }

    public abstract SkillsCollection GetSkills();

    public abstract SkillsCollection GetSkillsWithEnoughMana();

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

    public void GainMp(int amount)
    {
        CurrentStats = CurrentStats.RestoreMP(amount);
    }

    public void Revive(int hp)
    {
        if (IsAlive())
        {
            return;
        }

        CurrentStats.RestoreHP(hp);
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

    public void ApplyPhysicalCharge()
    {
        BuffState = BuffState.WithPhysicalCharge();
    }

    public void ApplyMagicalCharge()
    {
        BuffState = BuffState.WithMagicalCharge();
    }

    public void ConsumePhysicalCharge()
    {
        BuffState = BuffState.WithoutPhysicalCharge();
    }

    public void ConsumeMagicalCharge()
    {
        BuffState = BuffState.WithoutMagicalCharge();
    }

    public void IncreaseOffensiveGrade()
    {
        BuffState = BuffState.WithOffensiveGradeIncrease();
    }

    public void IncreaseDefensiveGrade()
    {
        BuffState = BuffState.WithDefensiveGradeIncrease();
    }

    public void SetHP(int newHP)
    {
        if (newHP > CurrentStats.MaxHP)
        {
            throw new ArgumentException("HP cannot exceed MaxHP");
        }
        CurrentStats.RestoreHP(newHP);
    }
}