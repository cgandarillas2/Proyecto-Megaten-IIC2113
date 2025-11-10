using Shin_Megami_Tensei_Model.Collections;
using Shin_Megami_Tensei_Model.Utils;
using Shin_Megami_Tensei_Model.Validators;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Exceptions;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Repositories;

public class TeamRepository
{
    private readonly IFileSystem _fileSystem;
    private readonly TeamFileParser _parser;
    private readonly JsonUnitRepository _unitRepository;
    private readonly TeamValidator _validator;
    
    public TeamRepository(
        IFileSystem fileSystem,
        JsonUnitRepository unitRepository)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _unitRepository = unitRepository ?? throw new ArgumentNullException(nameof(unitRepository));
        _parser = new TeamFileParser(fileSystem);
        _validator = new TeamValidator();
    }
    
    
    public StringCollection GetAvailableTeamFiles(string teamsFolder)
    {
        var files = _fileSystem.GetFiles(teamsFolder, "*.txt");
        return new StringCollection(files);
    }

    public (Team player1, Team player2) LoadTeams(string teamFilePath)
    
    {
        var (player1Data, player2Data) = _parser.ParseTeamFile(teamFilePath);

        ValidateTeamData(player1Data, "Player 1");
        ValidateTeamData(player2Data, "Player 2");

        var player1Team = BuildTeam("J1", player1Data);
        var player2Team = BuildTeam("J2", player2Data);

        return (player1Team, player2Team);
    }
    
    private void ValidateTeamData(TeamData teamData, string playerLabel)
    {
        if (!_validator.IsValid(teamData))
        {
            var error = _validator.GetFirstErrorMessage(teamData);
            throw new InvalidTeamException($"{playerLabel}: {error}");
        }
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