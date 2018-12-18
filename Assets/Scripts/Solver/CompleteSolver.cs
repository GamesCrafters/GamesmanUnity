using System.Collections;
using System.Collections.Generic;
using System;

public class CompleteSolver<GameStateT, GameMoveT> : AbstractSolver<GameStateT, GameMoveT>
    where GameStateT : AbstractGameState
    where GameMoveT : AbstractGameMove
{
    private AbstractGame<GameStateT, GameMoveT> Game;
    private IDictionary<string, GameValue> Memory = new Dictionary<string, GameValue>();

    public CompleteSolver(AbstractGame<GameStateT, GameMoveT> game)
    {
        Game = game;
        Solve(Game.State);
    }

    public override GameValue GetStatusValue(GameStateT gameState)
    { 
        if (!Memory.ContainsKey(Game.SerializeState(gameState)))
        {
            return GameValue.Undecided;
        }
        return Memory[Game.SerializeState(gameState)];
    }


    private GameValue Solve(GameStateT state)
    {
        bool winFlag = false;
        bool tieFlag = false;
        var serialized = Game.SerializeState(state);
        if (Memory.ContainsKey(serialized))
        {
            return Memory[serialized];
        }
        var primitive = Game.PrimitiveValue(state);
        if (primitive != GameValue.Undecided)
        {
            SetMemoryForAllSymmetry(primitive, Game.SerializeStateList(state));
            return primitive;
        }
        var moveList = Game.GenerateMoves(state);

        foreach (var move in moveList)
        {
            var newState = Game.DoMoveOnState(state, move);
            var result = Solve(newState);
            if (result == GameValue.Lose)
            {
                winFlag = true;
                //break;
            }
            else if (result == GameValue.Tie)
            {
                tieFlag = true;
            }
        }
        if (!winFlag)
        {
            if (tieFlag) 
            {
                SetMemoryForAllSymmetry(GameValue.Tie, Game.SerializeStateList(state));
                return GameValue.Tie;
            }
            else
            {
                SetMemoryForAllSymmetry(GameValue.Lose, Game.SerializeStateList(state));
                return GameValue.Lose;
            }
        }
        SetMemoryForAllSymmetry(GameValue.Win, Game.SerializeStateList(state));
        return GameValue.Win;
    }

    private void SetMemoryForAllSymmetry(GameValue gameValue, List<string> stringList)
    {
        foreach (string s in stringList)
        {
            Memory[s] = gameValue;
        }
        Console.WriteLine(Memory.Count);
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
