using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractSolver : MonoBehaviour 
{

    public abstract void InitSolver(AbstractGame game);

    public abstract GameValue GetStatusValue(AbstractGameState gameState);

    public abstract AbstractGameMove GetMove(AbstractGameState gameState);

}
