namespace Shin_Megami_Tensei_Model.Validators.TeamRules;

public class MaxSkillsRule: ITeamValidationRule
{
    private const int MaxSkills = 8;

    public bool IsValid(TeamData teamData)
    {
        return teamData.SamuraiSkills.Count <= MaxSkills;
    }

    public string GetErrorMessage()
    {
        return $"Samurai cannot have more than {MaxSkills} skills";
    }
}