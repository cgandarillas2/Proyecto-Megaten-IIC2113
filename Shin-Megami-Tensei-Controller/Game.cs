using Shin_Megami_Tensei_Model.Combat;
using Shin_Megami_Tensei_Model.Repositories;
using Shin_Megami_Tensei_Model.Utils;
using Shin_Megami_Tensei_View;

namespace Shin_Megami_Tensei;

public class Game
{
    private const string DataFolder = "Data";
    private const string SamuraiFile = "samurai.json";
    private const string MonstersFile = "monsters.json";
    private const string SkillsFile = "skills.json";

    private readonly View _view;
    private readonly string _teamsFolder;
    private readonly GameController _gameController;

    public Game(View view, string teamsFolder)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _teamsFolder = teamsFolder ?? throw new ArgumentNullException(nameof(teamsFolder));
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
            var damageCalculator = CreateDamageCalculator();
            var skillRepository = CreateSkillRepository(fileSystem, jsonSerializer, damageCalculator);
            var unitRepository = CreateUnitRepository(fileSystem, jsonSerializer, skillRepository);
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

        private DamageCalculator CreateDamageCalculator()
        {
            return new DamageCalculator();
        }

        private JsonSkillRepository CreateSkillRepository(
            IFileSystem fileSystem,
            IJsonSerializer jsonSerializer,
            DamageCalculator damageCalculator)
        {
            var repository = new JsonSkillRepository(fileSystem, jsonSerializer, damageCalculator);
            var skillsPath = BuildDataPath(SkillsFile);
            repository.LoadData(skillsPath);
            return repository;
        }

        private JsonUnitRepository CreateUnitRepository(
            IFileSystem fileSystem,
            IJsonSerializer jsonSerializer,
            JsonSkillRepository skillRepository)
        {
            var repository = new JsonUnitRepository(fileSystem, jsonSerializer, skillRepository);
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