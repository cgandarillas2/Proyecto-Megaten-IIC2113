using Shin_Megami_Tensei_Model.Combat;
using Shin_Megami_Tensei_Model.Factories;
using Shin_Megami_Tensei_Model.Repositories.Dtos;
using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Utils;

namespace Shin_Megami_Tensei_Model.Repositories;

public class JsonSkillRepository
{
    private readonly IFileSystem _fileSystem;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly SkillFactory _skillFactory;
        private Dictionary<string, ISkill> _skills;

        public JsonSkillRepository(
            IFileSystem fileSystem,
            IJsonSerializer jsonSerializer,
            DamageCalculator damageCalculator)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            _skillFactory = new SkillFactory(damageCalculator);
        }

        public void LoadData(string skillsPath)
        {
            var json = _fileSystem.ReadAllText(skillsPath);
            var skillDtos = _jsonSerializer.Deserialize<List<SkillDto>>(json);
            _skills = BuildSkillsDictionary(skillDtos);
        }

        public ISkill GetSkill(string name)
        {
            if (!_skills.ContainsKey(name))
            {
                throw new ArgumentException($"Skill not found: {name}");
            }
            return _skills[name];
        }

        public bool SkillExists(string name)
        {
            return _skills?.ContainsKey(name) ?? false;
        }

        public List<ISkill> GetSkillsByNames(List<string> names)
        {
            return names.Select(name => GetSkill(name)).ToList();
        }

        private Dictionary<string, ISkill> BuildSkillsDictionary(List<SkillDto> skillDtos)
        {
            var dictionary = new Dictionary<string, ISkill>();

            foreach (var dto in skillDtos)
            {
                try
                {
                    var skill = _skillFactory.CreateSkill(dto);
                    dictionary[dto.Name] = skill;
                }
                catch (NotImplementedException)
                {
                    // Skill no implementada aún (Heal, Support), skip
                    continue;
                }
            }

            return dictionary;
        }
}