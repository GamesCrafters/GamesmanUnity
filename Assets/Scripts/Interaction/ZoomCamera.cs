using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomCamera : MonoBehaviour {
    public Transform cam;
    public Transform target;
    public float speed;

    public RectTransform detectZone;
    public RectTransform miniCamMask;

    float lastDis;
    bool zooming;

    private void Start()
    {
        if (Input.touchSupported == false)
            gameObject.GetComponent<ZoomCamera>().enabled = false;
    }

    private void Update()
    {
        if(Input.touchCount==2 &&!zooming){
            if(detectZone.rect.Contains(Input.GetTouch(0).position) && detectZone.rect.Contains(Input.GetTouch(1).position))
            {
                zooming = true;
                lastDis = (Input.GetTouch(0).position - Input.GetTouch(1).position).magnitude;
            }
        }

        if(Input.touchCount != 2){
            zooming = false;
        }

        if (Input.touchCount ==2 && zooming){
            if (!(detectZone.rect.Contains(Input.GetTouch(0).position) && detectZone.rect.Contains(Input.GetTouch(1).position)))
            {
                zooming = false;
            }
        }

        if(zooming){
            float currentDis = (Input.GetTouch(0).position - Input.GetTouch(1).position).magnitude;
            float offset = currentDis - lastDis;
            //Debug.Log("Distance: "+ currentDis.ToString()+" "+ lastDis.ToString()+ " Offset:"+offset.ToString());
            cam.position += (cam.position - target.position).normalized * -0.01f * offset * speed * Time.deltaTime;
            lastDis = currentDis;
        }
    }
}
