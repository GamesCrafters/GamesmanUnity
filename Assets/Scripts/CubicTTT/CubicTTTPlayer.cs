using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubicTTTPlayer : AbstractPlayer<TTTGameState, TTTGameMove, CubicTTT>
{

    #region Fields

    [Header("Game Setting")]
    public int xSize = 3;
    public int ySize = 3;
    public int zSize = 1;

    public int winNumber = 3;

    [SerializeField]
    private SolverType solverType = SolverType.CompleteSolver;
    [SerializeField]
    private PlayerType p1 = PlayerType.P1;
    [SerializeField]
    private PlayerType p2 = PlayerType.COM;

    [SerializeField]
    private Piece p1Piece = Piece.o;
    [SerializeField]
    private Piece p2Piece = Piece.x;

    [Header("Block Prefabs")]
    public GameObject block;
    public Material winMaterial;
    public Material loseMaterial;
    public Material tieMaterial;
    public Material undecidedMaterial;
    public Color selectedMaskColor;
    public Color defaultMaskColor;

    [Header("UI Elements")]
    public Text promptText;

    public Button playButton;

    public InputField xInputField;
    public InputField yInputField;
    public InputField zInputField;

    public RectTransform loggerContent;
    public Font loggerFont;

    //=== Private

    private AbstractSolver<TTTGameState, TTTGameMove> Solver;

    private List<GameObject> blocks = new List<GameObject>();
    private List<GameObject> cubes = new List<GameObject>();
    private List<GameObject> spheres = new List<GameObject>();
    private List<GameObject> indicators = new List<GameObject>();

    private int logNum = 1;

    private PlayerType currentPlayer;

    private int turnCount = 1;
    #endregion


    private string PlayerTypeToStr(PlayerType p)
    {
        string playerS = "";
        switch (p)
        {
            case PlayerType.P1:
                playerS = "Player 1";
                break;
            case PlayerType.P2:
                playerS = "Player 2";
                break;
            case PlayerType.COM:
                playerS = "Computer";
                break;
        }
        return playerS;
    }

    private string SolverTypeToStr(SolverType s)
    {
        string solverS = "";
        switch (s)
        {
            case SolverType.CompleteSolver:
                solverS = "Local Complete Solver";
                break;
            case SolverType.RandomSolver:
                solverS = "Random Solver";
                break;
            case SolverType.RemoteSolver331:
                solverS = "Remote Solver";
                break;
        }
        return solverS;
    }

    public void IntiFromInfo(CubicTTTInitInfo info)
    {
        xSize = info.xSize;
        ySize = info.ySize;
        zSize = info.zSize;
        winNumber = info.winNumber;
        solverType = info.solverType;
        p1 = info.p1;
        p2 = info.p2;
        p1Piece = info.p1Piece;
        p2Piece = info.p2Piece;
    }

    public override void InitGame()
    {
        Prompt(string.Format("Initializing Cubic TTT Game {0}*{1}*{2}, {3} in a row to win.", xSize, ySize, zSize, winNumber));
        InitCubicTTTGame();
        Prompt("Initializing " + SolverTypeToStr(solverType) + "...");
        InitSolver();

        // Display
        blocks = new List<GameObject>();
        cubes = new List<GameObject>();
        spheres = new List<GameObject>();
        indicators = new List<GameObject>();
        Prompt("Initializing Game Display...");
        for (int i = 0; i < Game.XSize; i++)
        {
            for (int j = 0; j < Game.YSize; j++)
            {
                for (int k = 0; k < Game.ZSize; k++)
                {
                    var offset = new Vector3(i - Game.XSize / 2, j - Game.YSize / 2, k - Game.ZSize / 2);
                    GameObject tempBlock = Instantiate(block, this.transform.position + offset, this.transform.localRotation, this.transform);
                    blocks.Add(tempBlock);
                    cubes.Add(tempBlock.transform.Find("Cube").gameObject);
                    spheres.Add(tempBlock.transform.Find("Sphere").gameObject);
                    indicators.Add(tempBlock.transform.Find("Indicator").gameObject);
                }
            }
        }
        UpdateStateDisplay(Game.State);

        PromptTutorial();

        string playerS1 = PlayerTypeToStr(p1);
        string playerS2 = PlayerTypeToStr(p2);
        Prompt("Game start!\n" + playerS1 + " plays with Sphere.\n" + playerS2 + " plays with Cube.");
        turnCount = 1;
        currentPlayer = p1;
        Prompt("Turn " + turnCount.ToString() + " : " + playerS1 + "'s turn.");
        playButton.enabled = true;
    }

    private void InitSolver()
    {
        switch (solverType)
        {
            case SolverType.CompleteSolver:
                Solver = new CompleteSolver<TTTGameState, TTTGameMove>(Game);
                break;
            case SolverType.RandomSolver:
                Solver = new RandomSolver<TTTGameState, TTTGameMove>(Game);
                break;
            case SolverType.RemoteSolver331:
                Solver = new OnlineSolver331<TTTGameState, TTTGameMove>(Game);
                break;
        }

    }

    private void PromptTutorial()
    {
        Prompt("Tutorial:  Input the coorinate in the text fileds.  \n " +
               "X(Red): 0 to " + (xSize - 1).ToString() + "; Y(Green): 0 to " + (ySize - 1).ToString() + "; Z(Blue): 0 to " + (zSize - 1).ToString() + ".\n" +
               "Click the button to put your piece on the coordinate.");
    }

    private void Prompt(string s, bool log = true)
    {
        promptText.text = s;
        if (log)
        {
            Log(s);
        }
    }

    private void Log(string s)
    {
        var log = new GameObject("Log" + logNum.ToString(), typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        log.layer = LayerMask.NameToLayer("UI");
        log.transform.parent = loggerContent;
        var toAdd = loggerContent.GetComponent<GridLayoutGroup>().cellSize.y;
        loggerContent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, toAdd);
        var t = log.GetComponent<Text>();
        t.text = logNum.ToString() + ": " + s;
        t.font = loggerFont;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        logNum += 1;

    }

    private void InitCubicTTTGame()
    {
        Game = new CubicTTT(xSize, ySize, zSize, winNumber, p1Piece);
    }

    public override TTTGameState SelectFromAI()
    {
        return Game.DoMove(Solver.GetMove(Game.State));
    }

    private bool CheckLegalInput(out int x, out int y, out int z, bool log = false)
    {
        bool ok = true;
        if (!int.TryParse(xInputField.text, out x))
        {
            x = -1;
            if (log)
                Prompt("X input is not numeric!", false);
            ok = false;
        }
        if (!int.TryParse(yInputField.text, out y))
        {
            y = -1;
            if (log)
                Prompt("Y input is not numeric!", false);
            ok = false;
        }
        if (!int.TryParse(zInputField.text, out z))
        {
            z = -1;
            if (log)
                Prompt("Z input is not numeric!", false);
            ok = false;
        }

        return ok;
    }

    private bool CheckLegalPosition(int x, int y, int z, bool log = false)
    {
        bool ok = true;
        bool xOK = true, yOK = true, zOK = true;

        if (x < 0 || x > xSize)
        {
            xOK = false;
        }
        if (y < 0 || y > ySize)
        {
            yOK = false;
        }
        if (z < 0 || z > zSize)
        {
            zOK = false;
        }
        ok = xOK && yOK && zOK;

        if (!ok && log)
        {
            string wrongPos = (!(xOK) ? "x " : "") + (!(yOK) ? "y " : "") + (!(zOK) ? "z " : "");

            Prompt(string.Format("({0},{1},{2}) has {3}positions out of range.  Please retry.", x, y, z, wrongPos));
        }
        return ok;
    }

    private bool CheckAccessibleMove(int x, int y, int z, TTTGameMove move = null, TTTGameState state = null, bool log = false)
    {
        bool ok = true;
        state = state ?? Game.State;
        move = move ?? new TTTGameMove(x, y, z, (state.NextPiece == p1Piece ? p1Piece : p2Piece));
        var moves = Game.GenerateMoves();

        foreach (var m in moves)
        {
            ok |= m == move;
        }

        if (!ok && log)
        {
            Prompt(string.Format("({0},{1},{2}) is inaccessible move.  Please retry.", x, y, z), false);
        }
        return ok;
    }

    public override TTTGameState SelectFromUser()
    {
        #region Legacy
        //int x, y, z;
        //bool ok = true;
        //if (!int.TryParse(xInputField.text, out x))
        //{
        //    Prompt("X input is not numeric!", false);
        //    ok = false;
        //}
        //if (!int.TryParse(yInputField.text, out y))
        //{
        //    Prompt("Y input is not numeric!", false);
        //    ok = false;
        //}
        //if (!int.TryParse(zInputField.text, out z))
        //{
        //    Prompt("Z input is not numeric!", false);
        //    ok = false;
        //}
        //if (!ok)
        //{
        //    return null;
        //}
        //var move = new TTTGameMove(x, y, z, (Game.State.NextPiece == p1Piece?p1Piece:p2Piece));
        //var moves = Game.GenerateMoves();


        //foreach (var m in moves)
        //{
        //    if(m == move)
        //    {
        //        ok = true;
        //    }
        //}

        //if (!ok)
        //{
        //    Prompt(string.Format("({0},{1},{2}) is invalid position input.  Please retry.", x, y, z), false);
        //    return null;
        //}
        #endregion
        int x, y, z;
        bool ok = true;
        ok = CheckLegalInput(out x, out y, out z, true);
        if (!ok)
            return null;
        ok = CheckLegalPosition(x, y, z, true);
        if (!ok)
            return null;
        var move = new TTTGameMove(x, y, z, (Game.State.NextPiece == p1Piece ? p1Piece : p2Piece));
        ok = CheckAccessibleMove(x, y, z, move, Game.State, true);

        if (ok)
        {
            Prompt(string.Format("Player {3} plays on ({0},{1},{2})", x, y, z, currentPlayer == PlayerType.P1 ? 1 : 2));
            //xInputField.text = "";
            //yInputField.text = "";
            //zInputField.text = "";
            return Game.DoMove(move);
        }
        else
        {
            return null;
        }
    }

    public override void ShowEnd(PlayerType player, GameValue value)
    {
        playButton.enabled = false;
        string playerS = "";
        switch (player)
        {
            case PlayerType.P1:
                playerS = "Player 1";
                break;
            case PlayerType.P2:
                playerS = "Player 2";
                break;
            case PlayerType.COM:
                playerS = "Computer";
                break;
        }
        string valueS = "";
        switch (value)
        {
            case GameValue.Win:
                valueS = "Lose";
                Prompt(playerS + " " + valueS + "!");
                break;
            case GameValue.Lose:
                valueS = "Win";
                Prompt(playerS + " " + valueS + "!");
                break;
            case GameValue.Tie:
                valueS = "Tie";
                Prompt("With the move by " + playerS + " the game result is " + valueS + "!");
                break;
            case GameValue.Undecided:
                valueS = "Undecided";
                Prompt("The game is not end!");
                break;
        }
    }

    public override void UpdateStateDisplay(TTTGameState gameState)
    {
        for (int i = 0; i < Game.XSize; i++)
        {
            for (int j = 0; j < Game.YSize; j++)
            {
                for (int k = 0; k < Game.ZSize; k++)
                {
                    var idx = k + j * Game.ZSize + i * Game.YSize * Game.ZSize;
                    switch (gameState.Board[i][j][k])
                    {
                        case Piece.e:
                            cubes[idx].GetComponent<MeshRenderer>().enabled = false;
                            spheres[idx].GetComponent<MeshRenderer>().enabled = false;
                            var m = indicators[idx].GetComponent<MeshRenderer>();
                            m.enabled = true;
                            // When the nextPiece is palyed here, what value for the nextPiece player' nextPiece playrt is;
                            GameValue value = Solver.GetStatusValue(Game.DoMoveOnState(gameState, new TTTGameMove(i, j, k, gameState.NextPiece)));
                            switch (value)
                            {
                                case GameValue.Win:
                                    // value is for next's next, metarial is showing next's expectation
                                    m.material = loseMaterial;
                                    break;
                                case GameValue.Lose:
                                    // value is for next's next, metarial is showing next's expectation
                                    m.material = winMaterial;
                                    break;
                                case GameValue.Tie:
                                    m.material = tieMaterial;
                                    break;
                                case GameValue.Undecided:
                                    m.material = undecidedMaterial;
                                    break;
                            }
                            break;
                        case Piece.x:
                            cubes[idx].GetComponent<MeshRenderer>().enabled = true;
                            spheres[idx].GetComponent<MeshRenderer>().enabled = false;
                            indicators[idx].GetComponent<MeshRenderer>().enabled = false;
                            break;
                        case Piece.o:
                            cubes[idx].GetComponent<MeshRenderer>().enabled = false;
                            spheres[idx].GetComponent<MeshRenderer>().enabled = true;
                            indicators[idx].GetComponent<MeshRenderer>().enabled = false;
                            break;
                    }
                }
            }
        }
    }


    public void OnClickPlayOn()
    {
        if (currentPlayer != PlayerType.COM)
        {
            var nextState = SelectFromUser();
            if (nextState == null)
            {
                return;
            }
            UpdateStateDisplay(nextState);
            if (CheckEnd(Game.State, p1))
            {
                return;
            }

            currentPlayer = currentPlayer == p1 ? p2 : p1;
            turnCount += 1;
        }
        if (currentPlayer == PlayerType.COM)
        {
            Prompt("Computer's turn.");
            var moveC = Solver.GetMove(Game.State);
            var nextStateC = Game.DoMove(moveC);
            Prompt("Computer playes on " + string.Format("({0}, {1}, {2})", moveC.Coordinate[0], moveC.Coordinate[1], moveC.Coordinate[2]));
            UpdateStateDisplay(nextStateC);
            if (CheckEnd(Game.State, p2))
            {
                return;
            }
            currentPlayer = currentPlayer == p1 ? p2 : p1;
            turnCount += 1;
        }
        string playerS = PlayerTypeToStr(currentPlayer);
        Prompt("Turn " + turnCount.ToString() + " : " + playerS + "'s turn.");
    }

    private void Start()
    {
        InitGame();
    }

    void Update()
    {
        if (InputManager.GetInputDown(0))
        {
            var Ray = Camera.main.ScreenPointToRay(InputManager.GetInputPosition());
            RaycastHit hit;
            if (Physics.Raycast(Ray, out hit, 500f, LayerMask.GetMask("Indicator")))
            {
                Debug.Log(hit.collider.transform.position.ToString() + " indicator hit!");
                var cord = GetCordinateFromIndicator(hit.transform.gameObject);
                int selectedX = -1, selectedY = -1, selectedZ = -1;
                System.Int32.TryParse(xInputField.text, out selectedX);
                System.Int32.TryParse(yInputField.text, out selectedY);
                System.Int32.TryParse(zInputField.text, out selectedZ);
                if (cord == new Vector3Int(selectedX, selectedY, selectedZ))
                {
                    OnClickPlayOn();
                }
                else
                {
                    ChangeColorSelected(cord);
                    xInputField.text = cord.x.ToString();
                    yInputField.text = cord.y.ToString();
                    zInputField.text = cord.z.ToString();
                }
            }
        }
    }

    void ChangeColorSelected(int x = -1, int y = -1, int z = -1)
    {
        for (int i = 0; i < Game.XSize; i++)
        {
            for (int j = 0; j < Game.YSize; j++)
            {
                for (int k = 0; k < Game.ZSize; k++)
                {
                    var maskMeshRenderer = blocks[i * Game.YSize * Game.ZSize + j * Game.ZSize + k].transform.GetChild(0).GetComponent<MeshRenderer>();
                    if ((i == x || x == -1) && (j == y || y == -1) && (k == z || z == -1))
                        maskMeshRenderer.material.color = selectedMaskColor;
                    else
                        maskMeshRenderer.material.color = defaultMaskColor;
                }
            }
        }
    }

    void ChangeColorSelected(Vector3Int cordinate)
    {
        ChangeColorSelected(cordinate.x, cordinate.y, cordinate.z);
    }

    int GetIdxFromIndicator(GameObject indicator)
    {
        var b = indicator.transform.parent.gameObject;
        int idx = blocks.IndexOf(b);
        return idx;
    }

    Vector3Int GetCordinateFromIndicator(GameObject indicator)
    {
        var b = indicator.transform.parent.gameObject;
        int idx = blocks.IndexOf(b);
        int x = idx / (Game.YSize * Game.ZSize);
        int y = (idx - (Game.YSize * Game.ZSize) * x) / Game.ZSize;
        int z = idx - (Game.YSize * Game.ZSize) * x - Game.ZSize * y;
        return new Vector3Int(x, y, z);
    }

    public void OnEndEdit()
    {
        int x, y, z;
        CheckLegalInput(out x, out y, out z);
        x = x > Game.XSize - 1 ? -1 : x;
        y = y > Game.YSize - 1 ? -1 : y;
        z = z > Game.ZSize - 1 ? -1 : z;
        ChangeColorSelected(x, y, z);
    }
}