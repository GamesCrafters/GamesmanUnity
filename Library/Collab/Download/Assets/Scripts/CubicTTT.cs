using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Piece
{
    empty,
    x,
    o
}

public class TTTGameState : AbstractGameState
{
    public List<List<List<Piece>>> Board;
    public Piece CurrentPiece;

    public TTTGameState(int size, Piece currentChess)
    {
        Board = new List<List<List<Piece>>>(size);
        for (int i = 0; i < size; i++)
        {
            var temp1 = new List<List<Piece>>(size);
            for (int j = 0; j < size; j++)
            {
                var temp2 = new List<Piece>(size);
                for (int k = 0; k < size; k++)
                {
                    temp2.Add(Piece.empty);
                }
                temp1.Add(temp2);
            }
            Board.Add(temp1);
        }
        CurrentPiece = currentChess;
    }
}


public class TTTGameMove : AbstractGameMove
{
    public List<int> Coordinate;
    // length is 3
    public Piece Chess;

    public TTTGameMove(int x, int y, int z, Piece chess)
    {
        Coordinate = new List<int>(3);
        Coordinate.Add(x);
        Coordinate.Add(y);
        Coordinate.Add(z);

        Chess = chess;
    }
}

public class CubicTTT : AbstractGame<TTTGameState, TTTGameMove>
{
    public int Size;

    public CubicTTT(int size, Piece initPiece)
    {
        Size = size;
        State = new TTTGameState(size, initPiece);
    }

    public override TTTGameState DoMove(TTTGameMove move)
    {
        var stateCopy = DoMoveOnState(State, move);
        State = stateCopy;
        return State;
    }

    public override TTTGameState DoMoveOnState(TTTGameState state, TTTGameMove move)
    {
        var stateString = this.SerializeState(state);
        var stateCopy = this.DeserialzeState(stateString);
        var tttState = stateCopy as TTTGameState;
        var tttMove = move as TTTGameMove;
        var coord = tttMove.Coordinate;

        if (tttState.Board[coord[0]][coord[1]][coord[2]] != Piece.empty)
        {
            throw new System.Exception("The position is not empty.");
        }
        tttState.CurrentPiece = tttState.CurrentPiece == Piece.x ? Piece.o : Piece.x;
        tttState.Board[coord[0]][coord[1]][coord[2]] = tttMove.Chess;

        return tttState;
    }

    public override List<TTTGameMove> GenerateMoves()
    {
        return GenerateMoves(State);
    }

    public override List<TTTGameMove> GenerateMoves(TTTGameState state)
    {
        var tttState = state as TTTGameState;
        var moveList = new List<TTTGameMove>();
        for (int x = 0; x < Size; x++)
        {
            for (int y = 0; y < Size; y++)
            {
                for (int z = 0; z < Size; z++)
                {
                    if (tttState.Board[x][y][z] == Piece.empty) moveList.Add(new TTTGameMove(x, y, z, tttState.CurrentPiece));

                    //if (tttState.Board[x][y][z] == Piece.empty)
                    //{
                    //    moveList.Add(new TTTGameMove(x, y, z, tttState.CurrentChess));
                    //    break;
                    //}
                }
            }
        }
        return moveList;
    }



    public override GameValue PrimitiveValue()
    {
        return PrimitiveValue(State);
    }

    public override GameValue PrimitiveValue(TTTGameState state)
    {
        var tttState = state as TTTGameState;
        for (int x = 0; x < Size; x++)
        {
            for (int y = 0; y < Size; y++)
            {
                for (int z = 0; z < Size; z++)
                {
                    var chess = tttState.Board[x][y][z];
                    if (chess == Piece.empty) goto nextPosition;
                    if (x == 0)
                    {
                        for (int i = 0; i < Size; i++)
                        {
                            if (tttState.Board[x + i][y][z] != chess) goto nextDirection1;
                        }
                        return chess == tttState.CurrentPiece ? GameValue.Win : GameValue.Lose;
                    }
                nextDirection1:;
                    if (y == 0)
                    {
                        for (int i = 0; i < Size; i++)
                        {
                            if (tttState.Board[x][y + i][z] != chess) goto nextDirection2;
                        }
                        return chess == tttState.CurrentPiece ? GameValue.Win : GameValue.Lose;
                    }
                nextDirection2:;
                    if (z == 0)
                    {
                        for (int i = 0; i < Size; i++)
                        {
                            if (tttState.Board[x][y][z + i] != chess) goto nextDirection3;
                        }
                        return chess == tttState.CurrentPiece ? GameValue.Win : GameValue.Lose;
                    }
                nextDirection3:;
                    if (x == 0 && y == 0)
                    {
                        for (int i = 0; i < Size; i++)
                        {
                            if (tttState.Board[x + i][y + i][z] != chess) goto nextDirection4;
                        }
                        return chess == tttState.CurrentPiece ? GameValue.Win : GameValue.Lose;
                    }
                nextDirection4:;
                    if (x == 0 && z == 0)
                    {
                        for (int i = 0; i < Size; i++)
                        {
                            if (tttState.Board[x + i][y][z + i] != chess) goto nextDirection5;
                        }
                        return chess == tttState.CurrentPiece ? GameValue.Win : GameValue.Lose;
                    }
                nextDirection5:;
                    if (z == 0 && y == 0)
                    {
                        for (int i = 0; i < Size; i++)
                        {
                            if (tttState.Board[x][y + i][z + i] != chess) goto nextDirection6;
                        }
                        return chess == tttState.CurrentPiece ? GameValue.Win : GameValue.Lose;
                    }
                nextDirection6:;
                    if (x == 0 && y == Size - 1)
                    {
                        for (int i = 0; i < Size; i++)
                        {
                            if (tttState.Board[x + i][y - i][z] != chess) goto nextDirection7;
                        }
                        return chess == tttState.CurrentPiece ? GameValue.Win : GameValue.Lose;
                    }
                nextDirection7:;
                    if (x == 0 && z == Size - 1)
                    {
                        for (int i = 0; i < Size; i++)
                        {
                            if (tttState.Board[x + i][y][z - i] != chess) goto nextDirection8;
                        }
                        return chess == tttState.CurrentPiece ? GameValue.Win : GameValue.Lose;
                    }
                nextDirection8:;
                    if (z == 0 && y == Size - 1)
                    {
                        for (int i = 0; i < Size; i++)
                        {
                            if (tttState.Board[x][y - i][z + i] != chess) goto nextDirection9;
                        }
                        return chess == tttState.CurrentPiece ? GameValue.Win : GameValue.Lose;
                    }
                nextDirection9:;
                    if (x == 0 && y == 0 && z == 0)
                    {
                        for (int i = 0; i < Size; i++)
                        {
                            if (tttState.Board[x + i][y + i][z + i] != chess) goto nextDirection10;
                        }
                        return chess == tttState.CurrentPiece ? GameValue.Win : GameValue.Lose;
                    }
                nextDirection10:;
                    if (x == 0 && y == 0 && z == Size - 1)
                    {
                        for (int i = 0; i < Size; i++)
                        {
                            if (tttState.Board[x + i][y + i][z - i] != chess) goto nextDirection11;
                        }
                        return chess == tttState.CurrentPiece ? GameValue.Win : GameValue.Lose;
                    }
                nextDirection11:;
                    if (x == 0 && y == Size - 1 && z == 0)
                    {
                        for (int i = 0; i < Size; i++)
                        {
                            if (tttState.Board[x + i][y - i][z + i] != chess) goto nextDirection12;
                        }
                        return chess == tttState.CurrentPiece ? GameValue.Win : GameValue.Lose;
                    }
                nextDirection12:;
                    if (x == 0 && y == Size - 1 && z == Size - 1)
                    {
                        for (int i = 0; i < Size; i++)
                        {
                            if (tttState.Board[x + i][y - i][z - i] != chess) goto nextDirection13;
                        }
                        return chess == tttState.CurrentPiece ? GameValue.Win : GameValue.Lose;
                    }
                nextDirection13:;
                }
            nextPosition:;
            }
        }
        return GameValue.Undecided;
    }
}
