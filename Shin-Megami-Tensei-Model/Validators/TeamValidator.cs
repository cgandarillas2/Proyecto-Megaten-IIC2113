using Shin_Megami_Tensei_Model.Validators.TeamRules;
namespace Shin_Megami_Tensei_Model.Validators;

public class TeamValidator
{
    private readonly List<ITeamValidationRule> _rules;

    public TeamValidator()
    {
        _rules = CreateValidationRules();
    }

    public bool IsValid(TeamData teamData)
    {
        foreach (var rule in _rules)
        {
            if (!rule.IsValid(teamData))
            {
                return false;
            }
        }
        return true;
    }

    public string GetFirstErrorMessage(TeamData teamData)
    {
        foreach (var rule in _rules)
        {
            if (!rule.IsValid(teamData))
            {
                return rule.GetErrorMessage();
            }
        }
        return string.Empty;
    }

    private static List<ITeamValidationRule> CreateValidationRules()
    {
        return new List<ITeamValidationRule>
        {
            new SingleSamuraiRule(),
            new MaxUnitsRule(),
            new NoDuplicateMonstersRule(),
            new MaxSkillsRule(),
            new NoDuplicateSkillsRule()
        };
    }
}