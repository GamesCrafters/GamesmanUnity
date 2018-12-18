using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class OnlineSolver331<GameStateT, GameMoveT> : AbstractSolver<GameStateT, GameMoveT>
    where GameStateT : AbstractGameState
    where GameMoveT : AbstractGameMove
{

    private class Response
    {
        public string move;
        public string board;
        public string value;
        public int remoteness;
    }


    private AbstractGame<GameStateT, GameMoveT> Game;
    private IDictionary<string, GameValue> Memory = new Dictionary<string, GameValue>();

    public OnlineSolver331(AbstractGame<GameStateT, GameMoveT> game)
    {
        Game = game;
        VisitURLAndSave("+++++++++");
    }

    private string GET(string url)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        try
        {
            WebResponse response = request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }
        catch (WebException ex)
        {
            return GET(url);
        }
    }

    private string SerailizeForUrl(GameStateT gameState)
    {
        TTTGameState tTTGameState = gameState as TTTGameState;
        string result = "";
        var board = tTTGameState.Board;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i][j][0] == Piece.e) result += "+";
                else if (board[i][j][0] == Piece.x) result += "o";
                else if (board[i][j][0] == Piece.o) result += "x";
            }
        }
        return result;
    }

    private GameValue GetGameValueByResponse(string res)
    {
        if (res == "tie") return GameValue.Tie;
        else if (res == "win") return GameValue.Win;
        else if (res == "lose") return GameValue.Lose;
        else return GameValue.Undecided;
    }

    private void VisitURLAndSave(string board)
    {
        var data = GET("http://nyc.cs.berkeley.edu:8081/ttt/getNextMoveValues?board=" + board);
        JObject d = JObject.Parse(data);
        var responseList = d["response"].ToObject<List<Response>>();
        foreach (var response in responseList)
        {
            Memory[response.board.Replace(" ", "+")] = GetGameValueByResponse(response.value);
        }
    }

    public override GameValue GetStatusValue(GameStateT gameState)
    {
        VisitURLAndSave(SerailizeForUrl(gameState));
        return Memory[SerailizeForUrl(gameState)];
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
