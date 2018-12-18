using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType {
    P1,
    P2,
    COM
}

public abstract class AbstractPlayer<GameStateT, GameMoveT, GameT> 
    : MonoBehaviour 
    where GameStateT : AbstractGameState
    where GameMoveT : AbstractGameMove
    where GameT : AbstractGame<GameStateT, GameMoveT>
{
    protected GameT Game;

    public abstract void InitGame();

    public abstract void UpdateStateDisplay(GameStateT gameState);


    public abstract GameStateT SelectFromUser();
    public abstract GameStateT SelectFromAI();

    public virtual bool CheckEnd(GameStateT gameState, PlayerType player) {
        GameValue value = Game.PrimitiveValue(gameState);
        if (value != GameValue.Undecided)
        {
            ShowEnd(player, value);
            return true;
        }
        else
            return false;
    }
    public abstract void ShowEnd(PlayerType player, GameValue value);
}
