namespace Shin_Megami_Tensei.Services;

/// <summary>
/// Represents the result of executing an action in the ActionExecutor.
/// Follows Command-Query Separation principle.
/// </summary>
public class ActionExecutionResult
{
    private readonly ActionExecutionStatus _status;

    private ActionExecutionResult(ActionExecutionStatus status)
    {
        _status = status;
    }

    public static ActionExecutionResult Completed()
    {
        return new ActionExecutionResult(ActionExecutionStatus.Completed);
    }

    public static ActionExecutionResult Cancelled()
    {
        return new ActionExecutionResult(ActionExecutionStatus.Cancelled);
    }

    public bool WasCompleted() => _status == ActionExecutionStatus.Completed;

    public bool WasCancelled() => _status == ActionExecutionStatus.Cancelled;

    private enum ActionExecutionStatus
    {
        Completed,
        Cancelled
    }
}
