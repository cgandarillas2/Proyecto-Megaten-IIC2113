using Shin_Megami_Tensei_Model.Utils;
using Shin_Megami_Tensei_Model.Repositories.Dtos;
using Shin_Megami_Tensei_Model.Repositories.Parsers;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_Model.Repositories;

public class JsonUnitRepository
{
    private readonly IFileSystem _fileSystem;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly AffinityParser _affinityParser;
    private readonly StatsBuilder _statsBuilder;
    private readonly JsonSkillRepository _skillRepository;

    private Dictionary<string, UnitDto> _samuraiData;
    private Dictionary<string, UnitDto> _monsterData;

    public JsonUnitRepository(
            IFileSystem fileSystem,
            IJsonSerializer jsonSerializer,
            JsonSkillRepository skillRepository)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            _skillRepository = skillRepository ?? throw new ArgumentNullException(nameof(skillRepository));
            _affinityParser = new AffinityParser();
            _statsBuilder = new StatsBuilder();
        }

        public void LoadData(string samuraiPath, string monsterPath)
        {
            _samuraiData = LoadUnitsFromFile(samuraiPath);
            _monsterData = LoadUnitsFromFile(monsterPath);
        }

        public Samurai CreateSamurai(string name, IEnumerable<string> skillNames)
        {
            var dto = FindSamuraiDto(name);
            var stats = _statsBuilder.BuildFromDto(dto.Stats);
            var affinities = _affinityParser.ParseAffinitySet(dto.Affinity);
            var skills = _skillRepository.GetSkillsByNames(skillNames ?? Enumerable.Empty<string>());

            return new Samurai(name, stats, affinities, skills);
        }

        public Monster CreateMonster(string name)
        {
            var dto = FindMonsterDto(name);
            var stats = _statsBuilder.BuildFromDto(dto.Stats);
            var affinities = _affinityParser.ParseAffinitySet(dto.Affinity);
            var skillNames = dto.Skills ?? Array.Empty<string>();
            var skills = _skillRepository.GetSkillsByNames(skillNames);

            return new Monster(name, stats, affinities, skills);
        }

        public bool SamuraiExists(string name)
        {
            return _samuraiData?.ContainsKey(name) ?? false;
        }

        public bool MonsterExists(string name)
        {
            return _monsterData?.ContainsKey(name) ?? false;
        }

        private Dictionary<string, UnitDto> LoadUnitsFromFile(string filePath)
        {
            var json = _fileSystem.ReadAllText(filePath);
            var units = _jsonSerializer.Deserialize<UnitDto[]>(json);
            return units.ToDictionary(u => u.Name);
        }

        private UnitDto FindSamuraiDto(string name)
        {
            if (!_samuraiData.ContainsKey(name))
            {
                throw new ArgumentException($"Samurai not found: {name}");
            }
            return _samuraiData[name];
        }

        private UnitDto FindMonsterDto(string name)
        {
            if (!_monsterData.ContainsKey(name))
            {
                throw new ArgumentException($"Monster not found: {name}");
            }
            return _monsterData[name];
        }
}