namespace Shin_Megami_Tensei_Model.Validators;

public class TeamData
{
    public string SamuraiName { get; }
    public List<string> SamuraiSkills { get; }
    public List<string> MonsterNames { get; }
    
    public TeamData(
        string samuraiName,
        List<string> samuraiSkills,
        List<string> monsterNames)
    {
        SamuraiName = samuraiName;
        SamuraiSkills = samuraiSkills ?? new List<string>();
        MonsterNames = monsterNames ?? new List<string>();
    }
    
    public int GetTotalUnitCount()
    {
        return 1 + MonsterNames.Count;
    }
}