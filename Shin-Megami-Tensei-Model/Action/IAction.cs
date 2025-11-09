using Shin_Megami_Tensei_Model.Units;
using Shin_Megami_Tensei_Model.Game;

namespace Shin_Megami_Tensei_Model.Action;

public interface IAction
{ 
    ActionResult Execute(Unit actor, Unit target, GameState gameState);
    bool CanExecute(Unit actor, GameState gameState);
}