using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneScript : MonoBehaviour {

    public Transform cross;

    private float width = 2f;
    private float padding = 0.1f;

	// Use this for initialization
    void Start () {

        CreateBoard(3);

	}
	
    void CreateBoard (int dim) {
        GameObject board = GameObject.CreatePrimitive(PrimitiveType.Cube);
        board.transform.parent = this.transform;
        board.name = "Board";
        board.transform.position = new Vector3(0, 0, 0);
        board.transform.localScale = new Vector3(dim*width+(dim+1)*padding, 0.4f, dim*width+(dim+1)*padding);

        Material boardMaterial = new Material(Shader.Find("Standard"));
        boardMaterial.color = Color.white;
        board.GetComponent<Renderer>().material = boardMaterial;

        float offset = (dim - 1) / 2 * (width + padding);

        for (int i = 0; i < dim; i++) {
            for (int j = 0; j < dim; j++) {
                GameObject cell = CreateCell();
                cell.transform.parent = board.transform;
                cell.transform.position = new Vector3((width + padding) * i - offset, 0.25f, (width + padding) * j - offset);
            }
        }
        cross = Instantiate(cross);
        cross.transform.parent = this.transform;
        cross.transform.position = new Vector3(0, 0.35f, 0);
        cross.transform.localScale = new Vector3(width/3, 0.1f, width/3);
    }

    GameObject CreateCell () {

        GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cell.name = "Cell";
        cell.transform.localScale = new Vector3(width, 0.1f, width);
        Material cellMaterial = new Material(Shader.Find("Standard"));
        cellMaterial.color = Color.gray;
        cell.GetComponent<Renderer>().material = cellMaterial;
        return cell;

    }

	// Update is called once per frame
	void Update () {
		
	}
}
