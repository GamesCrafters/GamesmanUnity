using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteSolver<GameStateT, GameMoveT> : AbstractSolver<GameStateT, GameMoveT> 
    where GameStateT : AbstractGameState
    where GameMoveT : AbstractGameMove
{
    private AbstractGame<GameStateT, GameMoveT> Game;
    private IDictionary<string, GameValue> Memory = new Dictionary<string, GameValue>();


    public override GameValue GetStatusValue(GameStateT gameState)
    {
        return Memory[Game.SerializeState(gameState)];
    }


    private GameValue Solve(GameStateT state)
    {
        bool winFlag = false;
        var serialized = Game.SerializeState(state);
        if (Memory.ContainsKey(serialized))
        {
            return Memory[serialized];
        }
        var primitive = Game.PrimitiveValue(state);
        if (primitive != GameValue.Undecided)
        {
            Memory[serialized] = primitive;
            return primitive;
        }
        foreach (var move in Game.GenerateMoves(state))
        {
            var newState = Game.DoMoveOnState(state, move);
            if (Solve(newState) == GameValue.Lose)
            {
                Memory[serialized] = GameValue.Win;
                winFlag = true;
            }
        }
        if (!winFlag)
        {
            Memory[serialized] = GameValue.Lose;
        }
        return winFlag ? GameValue.Win : GameValue.Lose;
    }

    public override void InitSolver(AbstractGame<GameStateT, GameMoveT> game)
    {
        Game = game;
    }

    public override GameMoveT GetMove(GameStateT gameState)
    {
        var moveList = Game.GenerateMoves(gameState);
        foreach (var move in moveList)
        {
            var newState = Game.DoMoveOnState(gameState, move);
            if (Memory[Game.SerializeState(newState)] == GameValue.Win)
            {
                return move;
            }
        }
        return moveList[0];
    }
}
