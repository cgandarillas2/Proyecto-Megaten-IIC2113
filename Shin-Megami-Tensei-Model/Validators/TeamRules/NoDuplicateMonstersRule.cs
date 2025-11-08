namespace Shin_Megami_Tensei_Model.Validators.TeamRules;

public class NoDuplicateMonstersRule: ITeamValidationRule
{
    public bool IsValid(TeamData teamData)
    {
        for (int i = 0; i < teamData.MonsterNames.Count; i++)
        {
            for (int j = i + 1; j < teamData.MonsterNames.Count; j++)
            {
                if (teamData.MonsterNames[i] == teamData.MonsterNames[j])
                {
                    return false;
                }
            }
        }
        return true;
    }

    public string GetErrorMessage()
    {
        return "Team cannot have duplicate monsters";
    }
}