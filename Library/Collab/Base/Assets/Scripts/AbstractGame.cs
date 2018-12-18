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

public abstract class GameState { }

public abstract class GameMove { }

public abstract class AbstractGame : MonoBehaviour {

    public string GameName;

    public virtual GameState State {
        get;
        protected set;
    }

    public bool InitGameState(GameState state)
    {
        State = state;
        return true;
    }

    public abstract IList<GameMove> GenerateMoves();
    public abstract IList<GameMove> GenerateMoves(GameState state);

    public abstract GameState DoMove(GameMove move);
    public abstract GameState DoMoveOnState(GameState state, GameMove move);

    public abstract GameValue PrimitiveValue();
    public abstract GameValue PrimitiveValue(GameState state);

    public string SerializeState(GameState state){
        return JsonConvert.SerializeObject(state);
    }

    public GameState DeserialzeState(string stateString){
        return JsonConvert.DeserializeObject(stateString) as GameState;
    }



}
