namespace Shin_Megami_Tensei_Model.Validators.TeamRules;

public class NoDuplicateMonstersRule: ITeamValidationRule
{
    public bool IsValid(TeamData teamData)
    {
        var distinctCount = teamData.MonsterNames.Distinct().Count();
        return distinctCount == teamData.MonsterNames.Count;
    }

    public string GetErrorMessage()
    {
        return "Team cannot have duplicate monsters";
    }
}