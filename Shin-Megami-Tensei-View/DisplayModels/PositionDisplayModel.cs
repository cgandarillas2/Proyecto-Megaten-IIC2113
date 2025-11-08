namespace Shin_Megami_Tensei_View.DisplayModels;

/// <summary>
/// Display model for summon position selection, following MVC pattern.
/// Contains only presentation data, no business logic.
/// </summary>
public class PositionDisplayModel
{
    public int Position { get; set; }
    public string DisplayText { get; set; }
    public bool IsEmpty { get; set; }

    public PositionDisplayModel(int position, string displayText, bool isEmpty)
    {
        Position = position;
        DisplayText = displayText;
        IsEmpty = isEmpty;
    }
}
