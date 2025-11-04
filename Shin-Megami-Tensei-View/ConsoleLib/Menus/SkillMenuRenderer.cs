using Shin_Megami_Tensei_Model.Skills;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_View.ConsoleLib;

public class SkillMenuRenderer : IMenuRenderer<ISkill>
{
    private readonly View _view;

    public SkillMenuRenderer(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    public void Render(List<ISkill> skills, object context = null)
    {
        var actor = context as Unit;
        
        _view.WriteSeparation();
        _view.WriteLine($"Seleccione una habilidad para que {actor?.Name ?? "la unidad"} use");

        for (int i = 0; i < skills.Count; i++)
        {
            var skill = skills[i];
            _view.WriteLine($"{i + 1}-{skill.Name} MP:{skill.Cost}");
        }

        _view.WriteLine($"{skills.Count + 1}-Cancelar");
    }
}