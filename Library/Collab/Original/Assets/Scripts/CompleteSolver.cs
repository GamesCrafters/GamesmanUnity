using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteSolver: AbstractSolver
{
    private AbstractGame Game;
    private IDictionary<string, GameValue> Memory = new Dictionary<string, GameValue>();


    public override void InitSolver(AbstractGame game)
    {
        Game = game;
    }

    public override GameValue GetStatusValue(AbstractGameState gameState)
    {
        return Memory[Game.SerializeState(gameState)];
    }

    public override AbstractGameMove GetMove(AbstractGameState gameState)
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

    private GameValue Solve(AbstractGameState state)
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

}
