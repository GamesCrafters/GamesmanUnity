using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public enum GameValue
{
    Win,
    Lose,
    Tie,
    Undecided
}

public abstract class AbstractGameState { }

public abstract class AbstractGameMove { }

public abstract class AbstractGame : MonoBehaviour {

    public string GameName;

    public abstract AbstractGameState State {
        get;
        protected set;
    }

    public bool InitGameState(AbstractGameState state)
    {
        State = state;
        return true;
    }

    public abstract IList<AbstractGameMove> GenerateMoves();
    public abstract IList<AbstractGameMove> GenerateMoves(AbstractGameState state);

    public abstract AbstractGameState DoMove(AbstractGameMove move);
    public abstract AbstractGameState DoMoveOnState(AbstractGameState state, AbstractGameMove move);

    public abstract GameValue PrimitiveValue();
    public abstract GameValue PrimitiveValue(AbstractGameState state);

    public string SerializeState(AbstractGameState state){
        return JsonConvert.SerializeObject(state);
    }

    public AbstractGameState DeserialzeState(string stateString){
        return JsonConvert.DeserializeObject(stateString) as AbstractGameState;
    }



}
