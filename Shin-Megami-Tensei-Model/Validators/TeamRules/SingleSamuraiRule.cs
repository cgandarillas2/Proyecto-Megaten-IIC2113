namespace Shin_Megami_Tensei_Model.Validators.TeamRules;

public class SingleSamuraiRule: ITeamValidationRule
{
    public bool IsValid(TeamData teamData)
    {
        return !string.IsNullOrWhiteSpace(teamData.SamuraiName);
    }

    public string GetErrorMessage()
    {
        return "Team must have exactly one Samurai";
    }
}