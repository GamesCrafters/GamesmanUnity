using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubicTTTInitInfo : MonoBehaviour{
    [Header("Game Setting")]
    public int xSize = 3;
    public int ySize = 3;
    public int zSize = 1;
    public int winNumber = 3;
    public SolverType solverType = SolverType.CompleteSolver;
    public PlayerType p1 = PlayerType.P1;
    public PlayerType p2 = PlayerType.COM;
    public Piece p1Piece = Piece.o;
    public Piece p2Piece = Piece.x;

    [Header("UI Input")]
    public Dropdown solverDropdown;
    public Slider ModeSlider;
    public InputField xIn;
    public InputField yIn;
    public InputField zIn;
    public InputField wIn;

    public Text warnText;

    [Header("Reference")]
    public Canvas setCan;
    public CubicTTTPlayer player;
    public GameObject toInit;
    public Transform anchor;

    public bool AssignFromUI(){
        int x, y, z, w;
        if(!CheckLegalInput(out x, out y, out z, true)){
            return false;
        }
        if(!CheckLegalPosition(x,y,z, true)){
            return false;
        }
        if (!int.TryParse(wIn.text, out w))
        {
            Prompt("W input is not numeric!");
            return false;
        }
        if(w<=0){
            Prompt("Non Positive win number!");
        }
        xSize = x;
        ySize = y;
        zSize = z;
        winNumber = w;
        //===
        switch (solverDropdown.value)
        {
            case 0:
                solverType = SolverType.RandomSolver;
                break;
            case 1:
                solverType = SolverType.CompleteSolver;
                break;
            case 2:
                solverType = SolverType.RemoteSolver331;
                break;
            default:
                solverType = SolverType.RandomSolver;
                break;
        }
        //==
        if(ModeSlider.value > 0){
            p1 = PlayerType.P1;
            p2 = PlayerType.COM;
            p1Piece = Piece.o;
            p2Piece = Piece.x;
        }else{
            p1 = PlayerType.P1;
            p2 = PlayerType.P2;
            p1Piece = Piece.o;
            p2Piece = Piece.x;
        }

        return true;
    }

    void Prompt(string p){
        warnText.text = p;
    }

    private bool CheckLegalInput(out int x, out int y, out int z, bool log = false)
    {
        bool ok = true;
        if (!int.TryParse(xIn.text, out x))
        {
            if (log)
                Prompt("X input is not numeric!");
            ok = false;
        }
        if (!int.TryParse(yIn.text, out y))
        {
            if (log)
                Prompt("Y input is not numeric!");
            ok = false;
        }
        if (!int.TryParse(zIn.text, out z))
        {
            if (log)
                Prompt("Z input is not numeric!");
            ok = false;
        }
        return ok;
    }


    private bool CheckLegalPosition(int x, int y, int z, bool log = false)
    {
        bool ok = true;
        bool xOK = true, yOK = true, zOK = true;

        if (x < 0)
        {
            xOK = false;
        }
        if (y < 0)
        {
            yOK = false;
        }
        if (z < 0)
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

    public void StartGame()
    {
        if(AssignFromUI()){
            for (int i=0; i < player.transform.childCount;i++){
                Destroy(player.transform.GetChild(i).gameObject);
            }
            player.IntiFromInfo(this);
            player.InitGame();
            Debug.Log("Init a new game");
            SwitchSettingPanle();
        }
    }

    public void SwitchSettingPanle(){
        setCan.GetComponent<Canvas>().enabled = !setCan.GetComponent<Canvas>().enabled;
    }
}
