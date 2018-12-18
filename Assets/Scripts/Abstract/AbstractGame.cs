using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

public enum GameValue
{
    Lose,
    Win,
    Tie,
    Undecided
}

public abstract class AbstractGameState { }

public abstract class AbstractGameMove { }

public abstract class AbstractGame<GameStateT, GameMoveT>
    where GameStateT : AbstractGameState
    where GameMoveT : AbstractGameMove
{

    public string GameName;

    public virtual GameStateT State
    {
        get;
        protected set;
    }

    public virtual bool InitGameState(GameStateT state)
    {
        State = state;
        return true;
    }

    public abstract List<GameMoveT> GenerateMoves();
    public abstract List<GameMoveT> GenerateMoves(GameStateT state);

    public abstract GameStateT DoMove(GameMoveT move);
    public abstract GameStateT DoMoveOnState(GameStateT state, GameMoveT move);

    public abstract GameValue PrimitiveValue();
    public abstract GameValue PrimitiveValue(GameStateT state);

    public virtual string SerializeState(GameStateT state)
    {
        return JsonConvert.SerializeObject(state);
    }

    public virtual GameStateT DeserialzeState(string stateString)
    {
        return JsonConvert.DeserializeObject<GameStateT>(stateString);
    }

    public abstract List<string> SerializeStateList(GameStateT state);
}
