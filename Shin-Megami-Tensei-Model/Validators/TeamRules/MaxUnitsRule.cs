namespace Shin_Megami_Tensei_Model.Validators.TeamRules;

public class MaxUnitsRule: ITeamValidationRule
{
    private const int MaxTotalUnits = 8;

    public bool IsValid(TeamData teamData)
    {
        return teamData.GetTotalUnitCount() <= MaxTotalUnits;
    }

    public string GetErrorMessage()
    {
        return $"Team cannot have more than {MaxTotalUnits} units";
    }
}