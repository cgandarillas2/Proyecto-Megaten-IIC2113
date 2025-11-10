namespace Shin_Megami_Tensei_View.DisplayModels;


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
