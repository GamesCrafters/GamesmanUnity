using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSolver<GameStateT, GameMoveT>
    where GameStateT : AbstractGameState
    where GameMoveT : AbstractGameMove
{

    public abstract void InitSolver(AbstractGame<GameStateT, GameMoveT> game);

    public abstract GameValue GetStatusValue(GameStateT gameState);

    public abstract GameMoveT GetMove(GameStateT gameState);

}
