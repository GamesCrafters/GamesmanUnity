using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneSpawner : MonoBehaviour {

    public bool readyToSpwan = false;
    public LayerMask boardLayer;
    public LayerMask spawnerLayer;
    public KeyCode spwanKey;


	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (!readyToSpwan && Input.GetKeyDown(spwanKey))
        {
            readyToSpwan = true;
        }
        else if (readyToSpwan && !Input.GetKeyDown(spwanKey) && Input.GetButton("Fire1"))
        {
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit floorHit;

            if (Physics.Raycast(camRay, out floorHit, 100f, boardLayer))
            {
                GameObject child = Instantiate(this.gameObject, floorHit.point + Vector3.up, transform.rotation);
                child.GetComponent<CloneSpawner>().enabled = false;
            }
            readyToSpwan = false;
        }
    }
}

