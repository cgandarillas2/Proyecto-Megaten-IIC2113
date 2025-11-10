using Shin_Megami_Tensei_GUI;
using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Exceptions;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Repositories;
using Shin_Megami_Tensei_Model.Units;
using Shin_Megami_Tensei_Model.Validators;

namespace Shin_Megami_Tensei_View.Gui;

/// <summary>
/// Helper class para construir Teams desde ITeamInfo (GUI).
/// </summary>
public class TeamBuilder
{
    private readonly JsonUnitRepository _unitRepository;
    private readonly TeamValidator _validator;

    public TeamBuilder(JsonUnitRepository unitRepository)
    {
        _unitRepository = unitRepository ?? throw new ArgumentNullException(nameof(unitRepository));
        _validator = new TeamValidator();
    }

    /// <summary>
    /// Construye un Team desde la información de la GUI.
    /// </summary>
    public Team BuildFromTeamInfo(string playerName, ITeamInfo teamInfo)
    {
        if (teamInfo == null)
            throw new ArgumentNullException(nameof(teamInfo));

        // Validar datos del equipo
        var teamData = new TeamData(
            teamInfo.SamuraiName,
            new StringCollection(teamInfo.SkillNames),
            new StringCollection(teamInfo.DemonNames)
        );

        if (!_validator.IsValid(teamData))
        {
            var error = _validator.GetFirstErrorMessage(teamData);
            throw new InvalidTeamException($"{playerName}: {error}");
        }

        // Construir el equipo
        return BuildTeam(playerName, teamData);
    }

    private Team BuildTeam(string playerName, TeamData teamData)
    {
        var samurai = _unitRepository.CreateSamurai(
            teamData.SamuraiName,
            teamData.SamuraiSkills);

        var monsters = BuildMonsters(teamData.MonsterNames);

        return new Team(playerName, samurai, monsters.Cast<Monster>());
    }

    private UnitsCollection BuildMonsters(StringCollection monsterNames)
    {
        var monsters = new UnitsCollection();
        foreach (var name in monsterNames)
        {
            var monster = _unitRepository.CreateMonster(name);
            monsters.Add(monster);
        }
        return monsters;
    }
}
