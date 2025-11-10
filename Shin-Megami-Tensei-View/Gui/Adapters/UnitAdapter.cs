using Shin_Megami_Tensei_GUI;
using Shin_Megami_Tensei_Model.Units;

namespace Shin_Megami_Tensei_View.Gui.Adapters;

/// <summary>
/// Adapter que convierte una Unit del modelo a IUnit para la GUI.
/// </summary>
public class UnitAdapter : IUnit
{
    private readonly Unit _unit;

    public UnitAdapter(Unit unit)
    {
        _unit = unit ?? throw new ArgumentNullException(nameof(unit));
    }

    public string Name => _unit.Name;
    public int HP => _unit.CurrentStats.CurrentHP;
    public int MP => _unit.CurrentStats.CurrentMP;
    public int MaxHP => _unit.CurrentStats.MaxHP;
    public int MaxMP => _unit.CurrentStats.MaxMP;
}
