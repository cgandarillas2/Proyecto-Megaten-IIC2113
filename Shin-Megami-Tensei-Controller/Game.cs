using Shin_Megami_Tensei_Model.Repositories;
using Shin_Megami_Tensei_Model.Utils;
using Shin_Megami_Tensei_View;

namespace Shin_Megami_Tensei;

public class Game
{
    private const string DataFolder = "data";
    private const string SamuraiFile = "samurai.json";
    private const string MonstersFile = "monsters.json";

    private readonly View _view;
    private readonly string _teamsFolder;
    private readonly GameController _gameController;

    public Game(View view, string teamsFolder)
    {
        _view = view;
        _teamsFolder = teamsFolder;
        _gameController = InitializeGameController();
    }

    public void Play()
    {
        _gameController.Run();
    }

    private GameController InitializeGameController()
    {
        var fileSystem = CreateFileSystem();
        var jsonSerializer = CreateJsonSerializer();
        var unitRepository = CreateUnitRepository(fileSystem, jsonSerializer);
        var teamRepository = CreateTeamRepository(fileSystem, unitRepository);
        var teamController = CreateTeamController(teamRepository);
        
        return CreateGameController(teamController);
    }

    private IFileSystem CreateFileSystem()
    {
        return new FileSystemWrapper();
    }

    private IJsonSerializer CreateJsonSerializer()
    {
        return new NewtonsoftJsonSerializer();
    }

    private JsonUnitRepository CreateUnitRepository(
        IFileSystem fileSystem,
        IJsonSerializer jsonSerializer)
    {
        var repository = new JsonUnitRepository(fileSystem, jsonSerializer);
        LoadUnitData(repository);
        return repository;
    }

    private void LoadUnitData(JsonUnitRepository repository)
    {
        var samuraiPath = BuildDataPath(SamuraiFile);
        var monstersPath = BuildDataPath(MonstersFile);
        repository.LoadData(samuraiPath, monstersPath);
    }

    private TeamRepository CreateTeamRepository(
        IFileSystem fileSystem,
        JsonUnitRepository unitRepository)
    {
        return new TeamRepository(fileSystem, unitRepository);
    }

    private TeamController CreateTeamController(TeamRepository teamRepository)
    {
        return new TeamController(teamRepository, _view);
    }

    private GameController CreateGameController(TeamController teamController)
    {
        return new GameController(teamController, _view, _teamsFolder);
    }

    private string BuildDataPath(string fileName)
    {
        return $"{DataFolder}/{fileName}";
    }

}
