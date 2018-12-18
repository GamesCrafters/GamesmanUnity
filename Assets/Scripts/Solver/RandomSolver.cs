using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSolver<GameStateT, GameMoveT> : AbstractSolver<GameStateT, GameMoveT>
    where GameStateT : AbstractGameState
    where GameMoveT : AbstractGameMove
{
    private AbstractGame<GameStateT, GameMoveT> Game;

    public RandomSolver(AbstractGame<GameStateT, GameMoveT> game)
    {
        Game = game;
    }

    public override GameValue GetStatusValue(GameStateT gameState)
    {
        return GameValue.Undecided;
    }

    public override GameMoveT GetMove(GameStateT gameState)
    {
        var moveList = Game.GenerateMoves(gameState);
        return moveList[37 % moveList.Count];
    }

}
