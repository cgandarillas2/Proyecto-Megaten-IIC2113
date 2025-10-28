namespace Shin_Megami_Tensei_Model.Validators.TeamRules;

public interface ITeamValidationRule
{
    bool IsValid(TeamData teamData);
    string GetErrorMessage();
}