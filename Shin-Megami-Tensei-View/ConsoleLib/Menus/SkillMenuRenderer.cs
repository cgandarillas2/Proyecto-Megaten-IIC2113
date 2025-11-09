using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Stats;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_View.ConsoleLib;

public class SkillMenuRenderer : IMenuRenderer<ISkill>
{
    private readonly View _view;

    public SkillMenuRenderer(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    public void Render(IEnumerable<ISkill> skills, object context = null)
    {
        var actor = context as Unit;

        _view.WriteSeparation();
        _view.WriteLine($"Seleccione una habilidad para que {actor?.Name ?? "la unidad"} use");

        var skillList = skills.ToList();
        var activeSkills = new List<ISkill>();
        for (int i = 0; i < skillList.Count; i++)
        {
            if (skillList[i].Element != Element.Passive)
            {
                activeSkills.Add(skillList[i]);
            }
        }

        for (int i = 0; i < activeSkills.Count; i++)
        {
            var skill = activeSkills[i];
            _view.WriteLine($"{i + 1}-{skill.Name} MP:{skill.Cost}");
        }

        _view.WriteLine($"{activeSkills.Count + 1}-Cancelar");
    }
}