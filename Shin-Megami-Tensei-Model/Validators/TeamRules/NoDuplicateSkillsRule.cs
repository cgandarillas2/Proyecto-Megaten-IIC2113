namespace Shin_Megami_Tensei_Model.Validators.TeamRules;

public class NoDuplicateSkillsRule: ITeamValidationRule
{
    public bool IsValid(TeamData teamData)
    {
        var distinctCount = teamData.SamuraiSkills.Distinct().Count();
        return distinctCount == teamData.SamuraiSkills.Count;
    }

    public string GetErrorMessage()
    {
        return "Samurai cannot have duplicate skills";
    }
}