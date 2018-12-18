using System.Collections;
using System.Collections.Generic;

public enum Piece
{
    e,
    x,
    o
}

public class TTTGameState : AbstractGameState
{
    public List<List<List<Piece>>> Board;
    public Piece NextPiece;

    public TTTGameState(int xSize, int ySize, int zSize, Piece currentPiece)
    {
        Board = new List<List<List<Piece>>>(xSize);
        for (int i = 0; i < xSize; i++)
        {
            var temp1 = new List<List<Piece>>(ySize);
            for (int j = 0; j < ySize; j++)
            {
                var temp2 = new List<Piece>(zSize);
                for (int k = 0; k < zSize; k++)
                {
                    temp2.Add(Piece.e);
                }
                temp1.Add(temp2);
            }
            Board.Add(temp1);
        }
        NextPiece = currentPiece;
    }
}


public class TTTGameMove : AbstractGameMove
{
    public List<int> Coordinate;
    // length is 3
    public Piece Piece;

    public TTTGameMove(int x, int y, int z, Piece piece)
    {
        Coordinate = new List<int>(3);
        Coordinate.Add(x);
        Coordinate.Add(y);
        Coordinate.Add(z);

        Piece = piece;
    }
}

public class CubicTTT : AbstractGame<TTTGameState, TTTGameMove>
{
    public int XSize;
    public int YSize;
    public int ZSize;
    public int WinNum;

    public int Size
    {
        get
        {
            if (XSize == YSize && XSize == ZSize & XSize == WinNum)
            {
                return XSize;
            }
            else
                return 0;
        }
    }

    public CubicTTT(int xSize, int ySize, int zSize, int winNum, Piece initPiece)
    {
        XSize = xSize;
        YSize = ySize;
        ZSize = zSize;
        WinNum = winNum;
        State = new TTTGameState(xSize, ySize, zSize, initPiece);
    }

    public CubicTTT(int size, Piece initPiece)
    {
        XSize = size;
        YSize = size;
        ZSize = size;
        WinNum = size;
        State = new TTTGameState(size, size, size, initPiece);
    }

    public override TTTGameState DoMove(TTTGameMove move)
    {
        var stateCopy = DoMoveOnState(State, move);
        State = stateCopy;
        return State;
    }

    private TTTGameState GetStateCopy(TTTGameState state)
    {
        var newState = new TTTGameState(XSize, YSize, ZSize, state.NextPiece);
        for (int x = 0; x < XSize; x++)
        {
            for (int y = 0; y < YSize; y++)
            {
                for (int z = 0; z < ZSize; z++)
                {
                    newState.Board[x][y][z] = state.Board[x][y][z];
                }
            }
        }
        return newState;
    }

    public override TTTGameState DoMoveOnState(TTTGameState state, TTTGameMove move)
    {
        var stateCopy = GetStateCopy(state);
        var coord = move.Coordinate;

        if (stateCopy.Board[coord[0]][coord[1]][coord[2]] != Piece.e)
        {
            throw new System.Exception("The position is not empty.");
        }
        stateCopy.NextPiece = stateCopy.NextPiece == Piece.x ? Piece.o : Piece.x;
        stateCopy.Board[coord[0]][coord[1]][coord[2]] = move.Piece;

        return stateCopy;
    }

    public override List<TTTGameMove> GenerateMoves()
    {
        return GenerateMoves(State);
    }

    public override List<TTTGameMove> GenerateMoves(TTTGameState state)
    {
        var moveList = new List<TTTGameMove>();
        for (int x = 0; x < XSize; x++)
        {
            for (int y = 0; y < YSize; y++)
            {
                for (int z = 0; z < ZSize; z++)
                {
                    if (state.Board[x][y][z] == Piece.e) moveList.Add(new TTTGameMove(x, y, z, state.NextPiece));

                    //if (state.Board[x][y][z] == Piece.e)
                    //{
                    //    moveList.Add(new TTTGameMove(x, y, z, state.NextPiece));
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
        for (int x = 0; x < XSize; x++)
        {
            for (int y = 0; y < YSize; y++)
            {
                for (int z = 0; z < ZSize; z++)
                {
                    var chess = state.Board[x][y][z];
                    if (chess == Piece.e) continue;
                    //if (chess == Piece.e) goto nextPosition;
                    if (x + WinNum - 1 < XSize)
                    {
                        for (int i = 1; i < WinNum; i++)
                        {
                            if (state.Board[x + i][y][z] != chess) goto nextDirection1;
                        }
                        return chess == state.NextPiece ? GameValue.Win : GameValue.Lose;
                    }
                nextDirection1:;
                    if (y + WinNum - 1 < YSize)
                    {
                        for (int i = 1; i < WinNum; i++)
                        {
                            if (state.Board[x][y + i][z] != chess) goto nextDirection2;
                        }
                        return chess == state.NextPiece ? GameValue.Win : GameValue.Lose;
                    }
                nextDirection2:;
                    if (z + WinNum - 1 < ZSize)
                    {
                        for (int i = 1; i < WinNum; i++)
                        {
                            if (state.Board[x][y][z + i] != chess) goto nextDirection3;
                        }
                        return chess == state.NextPiece ? GameValue.Win : GameValue.Lose;
                    }
                nextDirection3:;
                    if (x + WinNum - 1 < XSize && y + WinNum - 1 < YSize)
                    {
                        for (int i = 1; i < WinNum; i++)
                        {
                            if (state.Board[x + i][y + i][z] != chess) goto nextDirection4;
                        }
                        return chess == state.NextPiece ? GameValue.Win : GameValue.Lose;
                    }
                nextDirection4:;
                    if (x + WinNum - 1 < XSize && y - WinNum + 1 >= 0)
                    {
                        for (int i = 1; i < WinNum; i++)
                        {
                            if (state.Board[x + i][y - i][z] != chess) goto nextDirection5;
                        }
                        return chess == state.NextPiece ? GameValue.Win : GameValue.Lose;
                    }
                nextDirection5:;
                    if (x + WinNum - 1 < XSize && z + WinNum - 1 < ZSize)
                    {
                        for (int i = 1; i < WinNum; i++)
                        {
                            if (state.Board[x + i][y][z + i] != chess) goto nextDirection6;
                        }
                        return chess == state.NextPiece ? GameValue.Win : GameValue.Lose;
                    }
                nextDirection6:;
                    if (x + WinNum - 1 < XSize && z - WinNum + 1 >= 0)
                    {
                        for (int i = 1; i < WinNum; i++)
                        {
                            if (state.Board[x + i][y][z - i] != chess) goto nextDirection7;
                        }
                        return chess == state.NextPiece ? GameValue.Win : GameValue.Lose;
                    }
                nextDirection7:;
                    if (y + WinNum - 1 < YSize && z + WinNum - 1 < ZSize)
                    {
                        for (int i = 1; i < WinNum; i++)
                        {
                            if (state.Board[x][y + i][z + i] != chess) goto nextDirection8;
                        }
                        return chess == state.NextPiece ? GameValue.Win : GameValue.Lose;
                    }
                nextDirection8:;
                    if (y + WinNum - 1 < YSize && z - WinNum + 1 >= 0)
                    {
                        for (int i = 1; i < WinNum; i++)
                        {
                            if (state.Board[x][y + i][z - i] != chess) goto nextDirection9;
                        }
                        return chess == state.NextPiece ? GameValue.Win : GameValue.Lose;
                    }
                nextDirection9:;
                    if (x + WinNum - 1 < XSize && y + WinNum - 1 < YSize && z + WinNum - 1 < ZSize)
                    {
                        for (int i = 1; i < WinNum; i++)
                        {
                            if (state.Board[x + i][y + i][z + i] != chess) goto nextDirection10;
                        }
                        return chess == state.NextPiece ? GameValue.Win : GameValue.Lose;
                    }
                nextDirection10:;
                    if (x + WinNum - 1 < XSize && y + WinNum - 1 < YSize && z - WinNum + 1 >= 0)
                    {
                        for (int i = 1; i < WinNum; i++)
                        {
                            if (state.Board[x + i][y + i][z - i] != chess) goto nextDirection11;
                        }
                        return chess == state.NextPiece ? GameValue.Win : GameValue.Lose;
                    }
                nextDirection11:;
                    if (x + WinNum - 1 < XSize && y - WinNum + 1 >= 0 && z + WinNum - 1 < ZSize)
                    {
                        for (int i = 1; i < WinNum; i++)
                        {
                            if (state.Board[x + i][y - i][z + i] != chess) goto nextDirection12;
                        }
                        return chess == state.NextPiece ? GameValue.Win : GameValue.Lose;
                    }
                nextDirection12:;
                    if (x + WinNum - 1 < XSize && y - WinNum + 1 >= 0 && z - WinNum + 1 >= 0)
                    {
                        for (int i = 1; i < WinNum; i++)
                        {
                            if (state.Board[x + i][y - i][z - i] != chess) goto nextDirection13;
                        }
                        return chess == state.NextPiece ? GameValue.Win : GameValue.Lose;
                    }
                nextDirection13:;

                }
                //nextPosition:;
            }
        }
        var moveList = GenerateMoves(state);
        if (moveList.Count == 0) return GameValue.Tie;
        return GameValue.Undecided;
    }


    public override string SerializeState(TTTGameState state)
    {

        List<string> pieceList = new List<string>();
        pieceList.Add(state.NextPiece.ToString());
        for (int x = 0; x < XSize; x++)
        {
            for (int y = 0; y < YSize; y++)
            {
                for (int z = 0; z < ZSize; z++)
                {
                    pieceList.Add(state.Board[x][y][z].ToString());
                }
            }
        }
        return string.Join("", pieceList);
    }

    public override TTTGameState DeserialzeState(string stateString)
    {
        var pieceList = new List<Piece>();
        foreach (char s in stateString)
        {
            Piece piece;
            if (s == Piece.o.ToString()[0])
            {
                piece = Piece.o;
            }
            else if (s == Piece.x.ToString()[0])
            {
                piece = Piece.x;
            }
            else
            {
                piece = Piece.e;
            }
            pieceList.Add(piece);
        }
        TTTGameState state = new TTTGameState(XSize, YSize, ZSize, pieceList[0]);
        int i = 1;
        for (int x = 0; x < XSize; x++)
        {
            for (int y = 0; y < YSize; y++)
            {
                for (int z = 0; z < ZSize; z++)
                {
                    state.Board[x][y][z] = pieceList[i];
                    i++;
                }
            }
        }
        return state;
    }

    public override List<string> SerializeStateList(TTTGameState state)
    {
        List<string> stateList = new List<string>();
        List<List<string>> pieceList = new List<List<string>>();
        int stateCompressNum;
        int stateCase;
        if (XSize == YSize && XSize == ZSize)
        {
            stateCase = 0;
            stateCompressNum = 48;
        }
        else if (XSize == YSize)
        {
            stateCase = 1;
            stateCompressNum = 16;
        }
        else if (XSize == ZSize)
        {
            stateCase = 2;
            stateCompressNum = 16;
        }
        else if (YSize == ZSize)
        {
            stateCase = 3;
            stateCompressNum = 16;
        }
        else
        {
            stateCase = 4;
            stateCompressNum = 8;
        }
        for (int i = 0; i < stateCompressNum; i++)
        {
            pieceList.Add(new List<string>());
            pieceList[i].Add(state.NextPiece.ToString());
        }
        for (int x = 0; x < XSize; x++)
        {
            for (int y = 0; y < YSize; y++)
            {
                for (int z = 0; z < ZSize; z++)
                {
                    switch (stateCase)
                    {
                        case 0:
                            pieceList[0].Add(state.Board[x][y][z].ToString());
                            pieceList[1].Add(state.Board[XSize - 1 - x][y][z].ToString());
                            pieceList[2].Add(state.Board[x][YSize - 1 - y][z].ToString());
                            pieceList[3].Add(state.Board[x][y][ZSize - 1 - z].ToString());
                            pieceList[4].Add(state.Board[XSize - 1 - x][YSize - 1 - y][z].ToString());
                            pieceList[5].Add(state.Board[XSize - 1 - x][y][ZSize - 1 - z].ToString());
                            pieceList[6].Add(state.Board[x][YSize - 1 - y][ZSize - 1 - z].ToString());
                            pieceList[7].Add(state.Board[XSize - 1 - x][YSize - 1 - y][ZSize - 1 - z].ToString());
                            pieceList[8].Add(state.Board[x][z][y].ToString());
                            pieceList[9].Add(state.Board[XSize - 1 - x][z][y].ToString());
                            pieceList[10].Add(state.Board[x][ZSize - 1 - z][y].ToString());
                            pieceList[11].Add(state.Board[x][z][YSize - 1 - y].ToString());
                            pieceList[12].Add(state.Board[XSize - 1 - x][ZSize - 1 - z][y].ToString());
                            pieceList[13].Add(state.Board[XSize - 1 - x][z][YSize - 1 - y].ToString());
                            pieceList[14].Add(state.Board[x][ZSize - 1 - z][YSize - 1 - y].ToString());
                            pieceList[15].Add(state.Board[XSize - 1 - x][ZSize - 1 - z][YSize - 1 - y].ToString());
                            pieceList[16].Add(state.Board[y][x][z].ToString());
                            pieceList[17].Add(state.Board[YSize - 1 - y][x][z].ToString());
                            pieceList[18].Add(state.Board[y][XSize - 1 - x][z].ToString());
                            pieceList[19].Add(state.Board[y][x][ZSize - 1 - z].ToString());
                            pieceList[20].Add(state.Board[YSize - 1 - y][XSize - 1 - x][z].ToString());
                            pieceList[21].Add(state.Board[YSize - 1 - y][x][ZSize - 1 - z].ToString());
                            pieceList[22].Add(state.Board[y][XSize - 1 - x][ZSize - 1 - z].ToString());
                            pieceList[23].Add(state.Board[YSize - 1 - y][XSize - 1 - x][ZSize - 1 - z].ToString());
                            pieceList[24].Add(state.Board[y][z][x].ToString());
                            pieceList[25].Add(state.Board[YSize - 1 - y][z][x].ToString());
                            pieceList[26].Add(state.Board[y][ZSize - 1 - z][x].ToString());
                            pieceList[27].Add(state.Board[y][z][XSize - 1 - x].ToString());
                            pieceList[28].Add(state.Board[YSize - 1 - y][ZSize - 1 - z][x].ToString());
                            pieceList[29].Add(state.Board[YSize - 1 - y][z][XSize - 1 - x].ToString());
                            pieceList[30].Add(state.Board[y][ZSize - 1 - z][XSize - 1 - x].ToString());
                            pieceList[31].Add(state.Board[YSize - 1 - y][ZSize - 1 - z][XSize - 1 - x].ToString());
                            pieceList[32].Add(state.Board[z][x][y].ToString());
                            pieceList[33].Add(state.Board[ZSize - 1 - z][x][y].ToString());
                            pieceList[34].Add(state.Board[z][XSize - 1 - x][y].ToString());
                            pieceList[35].Add(state.Board[z][x][YSize - 1 - y].ToString());
                            pieceList[36].Add(state.Board[ZSize - 1 - z][XSize - 1 - x][y].ToString());
                            pieceList[37].Add(state.Board[ZSize - 1 - z][x][YSize - 1 - y].ToString());
                            pieceList[38].Add(state.Board[z][XSize - 1 - x][YSize - 1 - y].ToString());
                            pieceList[39].Add(state.Board[ZSize - 1 - z][XSize - 1 - x][YSize - 1 - y].ToString());
                            pieceList[40].Add(state.Board[z][y][x].ToString());
                            pieceList[41].Add(state.Board[ZSize - 1 - z][y][x].ToString());
                            pieceList[42].Add(state.Board[z][YSize - 1 - y][x].ToString());
                            pieceList[43].Add(state.Board[z][y][XSize - 1 - x].ToString());
                            pieceList[44].Add(state.Board[ZSize - 1 - z][YSize - 1 - y][x].ToString());
                            pieceList[45].Add(state.Board[ZSize - 1 - z][y][XSize - 1 - x].ToString());
                            pieceList[46].Add(state.Board[z][YSize - 1 - y][XSize - 1 - x].ToString());
                            pieceList[47].Add(state.Board[ZSize - 1 - z][YSize - 1 - y][XSize - 1 - x].ToString());
                            break;
                        case 1:
                            pieceList[0].Add(state.Board[x][y][z].ToString());
                            pieceList[1].Add(state.Board[XSize - 1 - x][y][z].ToString());
                            pieceList[2].Add(state.Board[x][YSize - 1 - y][z].ToString());
                            pieceList[3].Add(state.Board[XSize - 1 - x][YSize - 1 - y][z].ToString());
                            pieceList[4].Add(state.Board[x][y][ZSize - 1 - z].ToString());
                            pieceList[5].Add(state.Board[XSize - 1 - x][y][ZSize - 1 - z].ToString());
                            pieceList[6].Add(state.Board[x][YSize - 1 - y][ZSize - 1 - z].ToString());
                            pieceList[7].Add(state.Board[XSize - 1 - x][YSize - 1 - y][ZSize - 1 - z].ToString());
                            pieceList[8].Add(state.Board[y][x][z].ToString());
                            pieceList[9].Add(state.Board[y][XSize - 1 - x][z].ToString());
                            pieceList[10].Add(state.Board[YSize - 1 - y][x][z].ToString());
                            pieceList[11].Add(state.Board[YSize - 1 - y][XSize - 1 - x][z].ToString());
                            pieceList[12].Add(state.Board[y][x][ZSize - 1 - z].ToString());
                            pieceList[13].Add(state.Board[y][XSize - 1 - x][ZSize - 1 - z].ToString());
                            pieceList[14].Add(state.Board[YSize - 1 - y][x][ZSize - 1 - z].ToString());
                            pieceList[15].Add(state.Board[YSize - 1 - y][XSize - 1 - x][ZSize - 1 - z].ToString());
                            break;
                        case 2:
                            pieceList[0].Add(state.Board[x][y][z].ToString());
                            pieceList[1].Add(state.Board[XSize - 1 - x][y][z].ToString());
                            pieceList[2].Add(state.Board[x][YSize - 1 - y][z].ToString());
                            pieceList[3].Add(state.Board[XSize - 1 - x][YSize - 1 - y][z].ToString());
                            pieceList[4].Add(state.Board[x][y][ZSize - 1 - z].ToString());
                            pieceList[5].Add(state.Board[XSize - 1 - x][y][ZSize - 1 - z].ToString());
                            pieceList[6].Add(state.Board[x][YSize - 1 - y][ZSize - 1 - z].ToString());
                            pieceList[7].Add(state.Board[XSize - 1 - x][YSize - 1 - y][ZSize - 1 - z].ToString());
                            pieceList[8].Add(state.Board[z][y][x].ToString());
                            pieceList[9].Add(state.Board[z][y][XSize - 1 - x].ToString());
                            pieceList[10].Add(state.Board[z][YSize - 1 - y][x].ToString());
                            pieceList[11].Add(state.Board[z][YSize - 1 - y][XSize - 1 - x].ToString());
                            pieceList[12].Add(state.Board[ZSize - 1 - z][y][x].ToString());
                            pieceList[13].Add(state.Board[ZSize - 1 - z][y][XSize - 1 - x].ToString());
                            pieceList[14].Add(state.Board[ZSize - 1 - z][YSize - 1 - y][x].ToString());
                            pieceList[15].Add(state.Board[ZSize - 1 - z][YSize - 1 - y][XSize - 1 - x].ToString());
                            break;
                        case 3:
                            pieceList[0].Add(state.Board[x][y][z].ToString());
                            pieceList[1].Add(state.Board[XSize - 1 - x][y][z].ToString());
                            pieceList[2].Add(state.Board[x][YSize - 1 - y][z].ToString());
                            pieceList[3].Add(state.Board[XSize - 1 - x][YSize - 1 - y][z].ToString());
                            pieceList[4].Add(state.Board[x][y][ZSize - 1 - z].ToString());
                            pieceList[5].Add(state.Board[XSize - 1 - x][y][ZSize - 1 - z].ToString());
                            pieceList[6].Add(state.Board[x][YSize - 1 - y][ZSize - 1 - z].ToString());
                            pieceList[7].Add(state.Board[XSize - 1 - x][YSize - 1 - y][ZSize - 1 - z].ToString());
                            pieceList[8].Add(state.Board[x][z][y].ToString());
                            pieceList[9].Add(state.Board[XSize - 1 - x][z][y].ToString());
                            pieceList[10].Add(state.Board[x][z][YSize - 1 - y].ToString());
                            pieceList[11].Add(state.Board[XSize - 1 - x][z][YSize - 1 - y].ToString());
                            pieceList[12].Add(state.Board[x][ZSize - 1 - z][y].ToString());
                            pieceList[13].Add(state.Board[XSize - 1 - x][ZSize - 1 - z][y].ToString());
                            pieceList[14].Add(state.Board[x][ZSize - 1 - z][YSize - 1 - y].ToString());
                            pieceList[15].Add(state.Board[XSize - 1 - x][ZSize - 1 - z][YSize - 1 - y].ToString());
                            break;
                        case 4:
                            pieceList[0].Add(state.Board[x][y][z].ToString());
                            pieceList[1].Add(state.Board[XSize - 1 - x][y][z].ToString());
                            pieceList[2].Add(state.Board[x][YSize - 1 - y][z].ToString());
                            pieceList[3].Add(state.Board[XSize - 1 - x][YSize - 1 - y][z].ToString());
                            pieceList[4].Add(state.Board[x][y][ZSize - 1 - z].ToString());
                            pieceList[5].Add(state.Board[XSize - 1 - x][y][ZSize - 1 - z].ToString());
                            pieceList[6].Add(state.Board[x][YSize - 1 - y][ZSize - 1 - z].ToString());
                            pieceList[7].Add(state.Board[XSize - 1 - x][YSize - 1 - y][ZSize - 1 - z].ToString());
                            break;
                    }
                }
            }
        }
        for (int i = 0; i < stateCompressNum; i++)
        {
            stateList.Add(string.Join("", pieceList[i]));
        }
        return stateList;

    }

    //public override List<string> SerializeStateList(TTTGameState state)
    //{
    //    List<string> stateList = new List<string>();
    //    List<List<string>> pieceList = new List<List<string>>();
    //    int stateCompressNum;
    //    if (XSize == YSize) stateCompressNum = 8;
    //    else stateCompressNum = 4;
    //    for (int i = 0; i < stateCompressNum; i++)
    //    {
    //        pieceList.Add(new List<string>());
    //        pieceList[i].Add(state.NextPiece.ToString());
    //    }
    //    for (int x = 0; x < XSize; x++)
    //    {
    //        for (int y = 0; y < YSize; y++)
    //        {
    //            for (int z = 0; z < ZSize; z++)
    //            {
    //                pieceList[0].Add(state.Board[x][y][z].ToString());
    //                pieceList[1].Add(state.Board[XSize - 1 - x][y][z].ToString());
    //                pieceList[2].Add(state.Board[x][YSize - 1 - y][z].ToString());
    //                pieceList[3].Add(state.Board[XSize - 1 - x][YSize - 1 - y][z].ToString());

    //                if (stateCompressNum == 8)
    //                {
    //                    pieceList[4].Add(state.Board[y][x][z].ToString());
    //                    pieceList[5].Add(state.Board[YSize - 1 - y][x][z].ToString());
    //                    pieceList[6].Add(state.Board[y][XSize - 1 - x][z].ToString());
    //                    pieceList[7].Add(state.Board[YSize - 1 - y][XSize - 1 - x][z].ToString());
    //                }
    //            }
    //        }
    //    }
    //    for (int i = 0; i < stateCompressNum; i++)
    //    {
    //        stateList.Add(string.Join("", pieceList[i]));
    //    }
    //    return stateList;
    //}




    //public override List<string> SerializeStateList(TTTGameState state)
    //{
    //    List<string> stateList = new List<string>();
    //    List<List<string>> pieceList = new List<List<string>>();
    //    var opponPiece = Piece.o == state.NextPiece ? Piece.x : Piece.o;
    //    for (int i = 0; i < 96; i++)
    //    {
    //        pieceList.Add(new List<string>());
    //        if (i<48)
    //        {
    //            pieceList[i].Add(state.NextPiece.ToString());
    //        }
    //        else
    //        {

    //            pieceList[i].Add(opponPiece.ToString());
    //        }
    //    }
    //    var size = state.Board.Count;
    //    for (int x = 0; x < Size; x++)
    //    {
    //        for (int y = 0; y < Size; y++)
    //        {
    //            for (int z = 0; z < Size; z++)
    //            {
    //                pieceList[0].Add(state.Board[x][y][z].ToString());
    //                pieceList[1].Add(state.Board[size - 1 - x][y][z].ToString());
    //                pieceList[2].Add(state.Board[x][size - 1 - y][z].ToString());
    //                pieceList[3].Add(state.Board[x][y][size - 1 - z].ToString());
    //                pieceList[4].Add(state.Board[size - 1 - x][size - 1 - y][z].ToString());
    //                pieceList[5].Add(state.Board[size - 1 - x][y][size - 1 - z].ToString());
    //                pieceList[6].Add(state.Board[x][size - 1 - y][size - 1 - z].ToString());
    //                pieceList[7].Add(state.Board[size - 1 - x][size - 1 - y][size - 1 - z].ToString());
    //                pieceList[8].Add(state.Board[x][z][y].ToString());
    //                pieceList[9].Add(state.Board[size - 1 - x][z][y].ToString());
    //                pieceList[10].Add(state.Board[x][size - 1 - z][y].ToString());
    //                pieceList[11].Add(state.Board[x][z][size - 1 - y].ToString());
    //                pieceList[12].Add(state.Board[size - 1 - x][size - 1 - z][y].ToString());
    //                pieceList[13].Add(state.Board[size - 1 - x][z][size - 1 - y].ToString());
    //                pieceList[14].Add(state.Board[x][size - 1 - z][size - 1 - y].ToString());
    //                pieceList[15].Add(state.Board[size - 1 - x][size - 1 - z][size - 1 - y].ToString());
    //                pieceList[16].Add(state.Board[y][x][z].ToString());
    //                pieceList[17].Add(state.Board[size - 1 - y][x][z].ToString());
    //                pieceList[18].Add(state.Board[y][size - 1 - x][z].ToString());
    //                pieceList[19].Add(state.Board[y][x][size - 1 - z].ToString());
    //                pieceList[20].Add(state.Board[size - 1 - y][size - 1 - x][z].ToString());
    //                pieceList[21].Add(state.Board[size - 1 - y][x][size - 1 - z].ToString());
    //                pieceList[22].Add(state.Board[y][size - 1 - x][size - 1 - z].ToString());
    //                pieceList[23].Add(state.Board[size - 1 - y][size - 1 - x][size - 1 - z].ToString());
    //                pieceList[24].Add(state.Board[y][z][x].ToString());
    //                pieceList[25].Add(state.Board[size - 1 - y][z][x].ToString());
    //                pieceList[26].Add(state.Board[y][size - 1 - z][x].ToString());
    //                pieceList[27].Add(state.Board[y][z][size - 1 - x].ToString());
    //                pieceList[28].Add(state.Board[size - 1 - y][size - 1 - z][x].ToString());
    //                pieceList[29].Add(state.Board[size - 1 - y][z][size - 1 - x].ToString());
    //                pieceList[30].Add(state.Board[y][size - 1 - z][size - 1 - x].ToString());
    //                pieceList[31].Add(state.Board[size - 1 - y][size - 1 - z][size - 1 - x].ToString());
    //                pieceList[32].Add(state.Board[z][x][y].ToString());
    //                pieceList[33].Add(state.Board[size - 1 - z][x][y].ToString());
    //                pieceList[34].Add(state.Board[z][size - 1 - x][y].ToString());
    //                pieceList[35].Add(state.Board[z][x][size - 1 - y].ToString());
    //                pieceList[36].Add(state.Board[size - 1 - z][size - 1 - x][y].ToString());
    //                pieceList[37].Add(state.Board[size - 1 - z][x][size - 1 - y].ToString());
    //                pieceList[38].Add(state.Board[z][size - 1 - x][size - 1 - y].ToString());
    //                pieceList[39].Add(state.Board[size - 1 - z][size - 1 - x][size - 1 - y].ToString());
    //                pieceList[40].Add(state.Board[z][y][x].ToString());
    //                pieceList[41].Add(state.Board[size - 1 - z][y][x].ToString());
    //                pieceList[42].Add(state.Board[z][size - 1 - y][x].ToString());
    //                pieceList[43].Add(state.Board[z][y][size - 1 - x].ToString());
    //                pieceList[44].Add(state.Board[size - 1 - z][size - 1 - y][x].ToString());
    //                pieceList[45].Add(state.Board[size - 1 - z][y][size - 1 - x].ToString());
    //                pieceList[46].Add(state.Board[z][size - 1 - y][size - 1 - x].ToString());
    //                pieceList[47].Add(state.Board[size - 1 - z][size - 1 - y][size - 1 - x].ToString());

    //                pieceList[48].Add(OpponentPiece(state.Board[x][y][z]).ToString());
    //                pieceList[49].Add(OpponentPiece(state.Board[size - 1 - x][y][z]).ToString());
    //                pieceList[50].Add(OpponentPiece(state.Board[x][size - 1 - y][z]).ToString());
    //                pieceList[51].Add(OpponentPiece(state.Board[x][y][size - 1 - z]).ToString());
    //                pieceList[52].Add(OpponentPiece(state.Board[size - 1 - x][size - 1 - y][z]).ToString());
    //                pieceList[53].Add(OpponentPiece(state.Board[size - 1 - x][y][size - 1 - z]).ToString());
    //                pieceList[54].Add(OpponentPiece(state.Board[x][size - 1 - y][size - 1 - z]).ToString());
    //                pieceList[55].Add(OpponentPiece(state.Board[size - 1 - x][size - 1 - y][size - 1 - z]).ToString());
    //                pieceList[56].Add(OpponentPiece(state.Board[x][z][y]).ToString());
    //                pieceList[57].Add(OpponentPiece(state.Board[size - 1 - x][z][y]).ToString());
    //                pieceList[58].Add(OpponentPiece(state.Board[x][size - 1 - z][y]).ToString());
    //                pieceList[59].Add(OpponentPiece(state.Board[x][z][size - 1 - y]).ToString());
    //                pieceList[60].Add(OpponentPiece(state.Board[size - 1 - x][size - 1 - z][y]).ToString());
    //                pieceList[61].Add(OpponentPiece(state.Board[size - 1 - x][z][size - 1 - y]).ToString());
    //                pieceList[62].Add(OpponentPiece(state.Board[x][size - 1 - z][size - 1 - y]).ToString());
    //                pieceList[63].Add(OpponentPiece(state.Board[size - 1 - x][size - 1 - z][size - 1 - y]).ToString());
    //                pieceList[64].Add(OpponentPiece(state.Board[y][x][z]).ToString());
    //                pieceList[65].Add(OpponentPiece(state.Board[size - 1 - y][x][z]).ToString());
    //                pieceList[66].Add(OpponentPiece(state.Board[y][size - 1 - x][z]).ToString());
    //                pieceList[67].Add(OpponentPiece(state.Board[y][x][size - 1 - z]).ToString());
    //                pieceList[68].Add(OpponentPiece(state.Board[size - 1 - y][size - 1 - x][z]).ToString());
    //                pieceList[69].Add(OpponentPiece(state.Board[size - 1 - y][x][size - 1 - z]).ToString());
    //                pieceList[70].Add(OpponentPiece(state.Board[y][size - 1 - x][size - 1 - z]).ToString());
    //                pieceList[71].Add(OpponentPiece(state.Board[size - 1 - y][size - 1 - x][size - 1 - z]).ToString());
    //                pieceList[72].Add(OpponentPiece(state.Board[y][z][x]).ToString());
    //                pieceList[73].Add(OpponentPiece(state.Board[size - 1 - y][z][x]).ToString());
    //                pieceList[74].Add(OpponentPiece(state.Board[y][size - 1 - z][x]).ToString());
    //                pieceList[75].Add(OpponentPiece(state.Board[y][z][size - 1 - x]).ToString());
    //                pieceList[76].Add(OpponentPiece(state.Board[size - 1 - y][size - 1 - z][x]).ToString());
    //                pieceList[77].Add(OpponentPiece(state.Board[size - 1 - y][z][size - 1 - x]).ToString());
    //                pieceList[78].Add(OpponentPiece(state.Board[y][size - 1 - z][size - 1 - x]).ToString());
    //                pieceList[79].Add(OpponentPiece(state.Board[size - 1 - y][size - 1 - z][size - 1 - x]).ToString());
    //                pieceList[80].Add(OpponentPiece(state.Board[z][x][y]).ToString());
    //                pieceList[81].Add(OpponentPiece(state.Board[size - 1 - z][x][y]).ToString());
    //                pieceList[82].Add(OpponentPiece(state.Board[z][size - 1 - x][y]).ToString());
    //                pieceList[83].Add(OpponentPiece(state.Board[z][x][size - 1 - y]).ToString());
    //                pieceList[84].Add(OpponentPiece(state.Board[size - 1 - z][size - 1 - x][y]).ToString());
    //                pieceList[85].Add(OpponentPiece(state.Board[size - 1 - z][x][size - 1 - y]).ToString());
    //                pieceList[86].Add(OpponentPiece(state.Board[z][size - 1 - x][size - 1 - y]).ToString());
    //                pieceList[87].Add(OpponentPiece(state.Board[size - 1 - z][size - 1 - x][size - 1 - y]).ToString());
    //                pieceList[88].Add(OpponentPiece(state.Board[z][y][x]).ToString());
    //                pieceList[89].Add(OpponentPiece(state.Board[size - 1 - z][y][x]).ToString());
    //                pieceList[90].Add(OpponentPiece(state.Board[z][size - 1 - y][x]).ToString());
    //                pieceList[91].Add(OpponentPiece(state.Board[z][y][size - 1 - x]).ToString());
    //                pieceList[92].Add(OpponentPiece(state.Board[size - 1 - z][size - 1 - y][x]).ToString());
    //                pieceList[93].Add(OpponentPiece(state.Board[size - 1 - z][y][size - 1 - x]).ToString());
    //                pieceList[94].Add(OpponentPiece(state.Board[z][size - 1 - y][size - 1 - x]).ToString());
    //                pieceList[95].Add(OpponentPiece(state.Board[size - 1 - z][size - 1 - y][size - 1 - x]).ToString());
    //            }
    //        }
    //    }
    //    for (int i = 0; i < 96; i++)
    //    {
    //        stateList.Add(string.Join(" ", pieceList[i]));
    //    }
    //    return stateList;
    //}

    private Piece OpponentPiece(Piece piece)
    {
        if (piece == Piece.o)
        {
            return Piece.x;
        }
        else if (piece == Piece.x)
        {
            return Piece.o;
        }
        return Piece.e;
    }

}
