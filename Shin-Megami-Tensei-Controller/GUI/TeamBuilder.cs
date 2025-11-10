using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Repositories;
using Shin_Megami_Tensei_Model.Units;
using Shin_Megami_Tensei_GUI;

namespace Shin_Megami_Tensei.GUI;

/// <summary>
/// Builds a Team from ITeamInfo provided by the GUI
/// </summary>
public class TeamBuilder
{
    private readonly JsonUnitRepository _unitRepository;

    public TeamBuilder(JsonUnitRepository unitRepository)
    {
        _unitRepository = unitRepository ?? throw new ArgumentNullException(nameof(unitRepository));
    }

    /// <summary>
    /// Converts ITeamInfo from GUI to a Team from the model
    /// </summary>
    public Team BuildTeam(ITeamInfo teamInfo, string playerName)
    {
        if (teamInfo == null)
        {
            throw new ArgumentNullException(nameof(teamInfo));
        }

        // Create the samurai with skills
        var samurai = _unitRepository.CreateSamurai(
            teamInfo.SamuraiName,
            teamInfo.SkillNames
        );

        // Create the monsters
        var monsters = new List<Monster>();
        foreach (var demonName in teamInfo.DemonNames)
        {
            var monster = _unitRepository.CreateMonster(demonName);
            monsters.Add(monster);
        }

        // Create and return the team
        return new Team(playerName, samurai, monsters);
    }
}
