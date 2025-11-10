using Shin_Megami_Tensei_Model.Collections;
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
            var skillDtos = _jsonSerializer.Deserialize<SkillDto[]>(json);
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

        public SkillsCollection GetSkillsByNames(IEnumerable<string> names)
        {
            var skills = new List<ISkill>();
            foreach (var name in names)
            {
                skills.Add(GetSkill(name));
            }
            return new SkillsCollection(skills);
        }

        private Dictionary<string, ISkill> BuildSkillsDictionary(SkillDto[] skillDtos)
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
                    // Skill not implemented
                }
            }

            return dictionary;
        }
}