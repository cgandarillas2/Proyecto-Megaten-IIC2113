using Shin_Megami_Tensei.Services;
using Shin_Megami_Tensei_Model.Action;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Units;
using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei_View.ConsoleLib;

namespace Shin_Megami_Tensei;

public class CombatController
{
    private readonly BoardView _boardView;
    private readonly CombatView _combatView;
    private readonly ActionSelector _actionSelector;
    private readonly ActionExecutor _actionExecutor;

    public CombatController(View view)
    {
        if (view == null)
        {
            throw new ArgumentNullException(nameof(view));
        }

        _boardView = new BoardView(view);
        _combatView = new CombatView(view);

        var skillController = new SkillController(view);
        var targetSelector = new TargetSelector(view);
        var positionSelector = new PositionSelector(view);
        var summonCoordinator = new SummonCoordinator();

        _actionSelector = new ActionSelector(view, skillController);
        _actionExecutor = new ActionExecutor(view, skillController, targetSelector, positionSelector, summonCoordinator);
    }

    public void InitialRoundHeaderMessage(GameState gameState)
    {
        var player = gameState.CurrentPlayer;
        var samurai = player.ActiveBoard.GetSamurai();
        _combatView.ShowRoundHeader(samurai.Name, player.PlayerName);
    }

    public ActionExecutionResult ExecuteRound(GameState gameState)
    {
        DisplayGameState(gameState);
        var actingUnit = gameState.GetCurrentActingUnit();
        return ExecuteTurnForUnit(actingUnit, gameState);
    }

    private void DisplayGameState(GameState gameState)
    {
        _boardView.ShowBoardState(gameState.Player1, gameState.Player2);
        _boardView.ShowTurnState(
            gameState.CurrentTurnState.FullTurns,
            gameState.CurrentTurnState.BlinkingTurns
        );
        _boardView.ShowActionOrder(gameState.ActionQueue.GetOrderedUnits());
    }

    private ActionExecutionResult ExecuteTurnForUnit(Unit actor, GameState gameState)
    {
        var action = _actionSelector.SelectAction(actor, gameState);

        if (action == null)
        {
            return ActionExecutionResult.Cancelled();
        }

        var executionResult = action switch
        {
            SurrenderAction => _actionExecutor.ExecuteSurrender(actor, gameState),
            SummonAction => _actionExecutor.ExecuteSummon(actor, gameState),
            PassTurnAction => _actionExecutor.ExecutePassTurn(actor, gameState),
            UseSkillAction skillAction => _actionExecutor.ExecuteSkill(skillAction, actor, gameState),
            _ => _actionExecutor.ExecuteCombatAction(action, actor, gameState)
        };

        if (executionResult.WasCancelled())
        {
            return ExecuteTurnForUnit(actor, gameState);
        }

        return ActionExecutionResult.Completed();
    }
}
