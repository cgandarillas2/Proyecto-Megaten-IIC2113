namespace Shin_Megami_Tensei_View.DisplayModels;

/// <summary>
/// Display model for action menu, following MVC pattern.
/// Removes type checking from views.
/// </summary>
public class ActionMenuModel
{
    public string ActorName { get; set; }
    public List<string> Actions { get; set; }

    public ActionMenuModel(string actorName, List<string> actions)
    {
        ActorName = actorName;
        Actions = actions;
    }
}
