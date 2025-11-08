namespace Shin_Megami_Tensei_Model.Validators.TeamRules;

public class NoDuplicateSkillsRule: ITeamValidationRule
{
    public bool IsValid(TeamData teamData)
    {
        for (int i = 0; i < teamData.SamuraiSkills.Count; i++)
        {
            for (int j = i + 1; j < teamData.SamuraiSkills.Count; j++)
            {
                if (teamData.SamuraiSkills[i] == teamData.SamuraiSkills[j])
                {
                    return false;
                }
            }
        }
        return true;
    }

    public string GetErrorMessage()
    {
        return "Samurai cannot have duplicate skills";
    }
}