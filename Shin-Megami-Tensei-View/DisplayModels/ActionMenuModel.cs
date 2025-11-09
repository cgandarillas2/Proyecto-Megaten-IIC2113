using Shin_Megami_Tensei_Model.Collections;

namespace Shin_Megami_Tensei_View.DisplayModels;

/// <summary>
/// Display model for action menu, following MVC pattern.
/// Removes type checking from views.
/// </summary>
public class ActionMenuModel
{
    public string ActorName { get; set; }
    public StringCollection Actions { get; set; }

    public ActionMenuModel(string actorName, StringCollection actions)
    {
        ActorName = actorName;
        Actions = actions;
    }
}
