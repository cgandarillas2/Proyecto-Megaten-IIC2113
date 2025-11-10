using Shin_Megami_Tensei_Model.Collections;

namespace Shin_Megami_Tensei_Model.Validators;

public class TeamData
{
    public string SamuraiName { get; }
    public StringCollection SamuraiSkills { get; }
    public StringCollection MonsterNames { get; }

    public TeamData(
        string samuraiName,
        StringCollection samuraiSkills,
        StringCollection monsterNames)
    {
        SamuraiName = samuraiName;
        SamuraiSkills = samuraiSkills ?? StringCollection.Empty();
        MonsterNames = monsterNames ?? StringCollection.Empty();
    }

    public int GetTotalUnitCount()
    {
        return 1 + MonsterNames.Count;
    }
}