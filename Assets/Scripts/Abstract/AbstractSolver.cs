using System.Collections;
using System.Collections.Generic;

public enum SolverType
{
    CompleteSolver,
    RandomSolver,
    RemoteSolver331,
}

public abstract class AbstractSolver<GameStateT, GameMoveT>
    where GameStateT : AbstractGameState
    where GameMoveT : AbstractGameMove
{

    public abstract GameValue GetStatusValue(GameStateT gameState);

    public abstract GameMoveT GetMove(GameStateT gameState);

    public virtual GameValue GetExpectedValueIfMoveOn(GameStateT gameState, GameMoveT gameMove)
    {
        throw new System.NotImplementedException();
    }
}
