using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundCamera : MonoBehaviour {
    public Transform camTransform;
    public Transform target;
    public float speedX = 1.0f;
    public float speedY = 1.0f;

    public RectTransform detectionZoneMask;
    public RectTransform miniCameraMask;

    //public InputManager.InputMethod inputMethod = InputManager.InputMethod.Auto;

    bool draging = false;

    /// <summary>
    /// 鼠标按下的之前位置
    /// </summary>
    Vector3 previousPosition;
    /// <summary>
    /// 鼠标按下之后的滑动距离
    /// </summary>
    Vector3 offset;

    Vector3 axisToRoutateAround;

    enum axis{
        x,
        y,
        z,
        normalInXZ,
        empty
    }

    private axis routatingAround = axis.empty;

    //private delegate bool GetInputDown(int idx);
    //GetInputDown getInputDown;
    //private delegate bool GetInput(int idx);
    //GetInput getInput;
    //private delegate bool GetInputUp(int idx);
    //GetInputUp getInputUp;


    //private delegate Vector3 GetInputPosition();
    //GetInputPosition getInputPosition = new GetInputPosition(InputManager.GetInputPosition);

    private void Awake()
    {
        //if(inputMethod == InputManager.InputMethod.Auto){
        //    inputMethod = Input.touchSupported ? InputManager.InputMethod.Touch : InputManager.InputMethod.Mouse;
        //}
        //switch (inputMethod)
        //{
        //    case InputManager.InputMethod.Mouse:
        //        Input.simulateMouseWithTouches = false;
        //        getInputDown = new GetInputDown(Input.GetMouseButtonDown);
        //        getInputUp = new GetInputUp(Input.GetMouseButtonUp);
        //        getInput = new GetInput(Input.GetMouseButton);
        //        getInputPosition = new GetInputPosition(() => Input.mousePosition);
        //        break;
        //    case InputManager.InputMethod.Touch:
        //        Input.simulateMouseWithTouches = false;
        //        ///<summary>
        //        /// 踩了一个坑，在C#里，匿名函数的返回值是一个布尔运算式的时候，其中的 <c>&&</c> 不会触发短路逻辑，需要用 <c>(condition1)?(condition2):false</c> 代替
        //        /// 实际上是因为 A && B == C 的时候，尽管 B 可能不是 bool，但是考虑到隐式转换，仍然按照 == 的高优先级计算
        //        /// </summary>
        //        //getInputDown = new GetInputDown((int idx) => Input.touchCount == 1 ? Input.GetTouch(idx).phase == TouchPhase.Began : false);
        //        getInputDown = new GetInputDown((int idx) => (Input.touchCount == 1) && (Input.GetTouch(idx).phase == TouchPhase.Began));
        //        getInputUp = new GetInputUp((int idx) => Input.touchCount == 1 ? Input.GetTouch(idx).phase == TouchPhase.Ended || Input.GetTouch(idx).phase == TouchPhase.Canceled : false);
        //        getInput = new GetInput((int idx) => Input.touchCount == 1 ? Input.GetTouch(idx).phase == TouchPhase.Stationary || Input.GetTouch(idx).phase == TouchPhase.Moved : false);
        //        getInputPosition = new GetInputPosition(() => Input.touchCount == 1 ? new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y) : Vector3.zero);
        //        break;
        //    case InputManager.InputMethod.TouchSimulatingMouse:
        //        Input.simulateMouseWithTouches = true;
        //        getInputDown = new GetInputDown(Input.GetMouseButtonDown);
        //        getInputUp = new GetInputUp(Input.GetMouseButtonUp);
        //        getInput = new GetInput(Input.GetMouseButton);
        //        getInputPosition = new GetInputPosition(() => Input.mousePosition);
        //        break;
        //}
    }


    void Update()
    {
        offset = (InputManager.GetInputPosition() - previousPosition);
        if (InputManager.GetInputDown(0))    // Input.GetMouseButtonDown(0) 当0键被按下一次
        {
            if(detectionZoneMask.rect.Contains(InputManager.GetInputPosition()))
            {
                draging = true;
            }
            else
            {
                draging = false;
                Debug.Log("No draging" + InputManager.GetInputPosition().ToString() + "in " + detectionZoneMask.rect.size.ToString());
            }
            if(miniCameraMask.rect.Contains(InputManager.GetInputPosition())){
                draging = false;
                Debug.Log("Cancle Input draging" + InputManager.GetInputPosition().ToString() + "in " + miniCameraMask.rect.size.ToString());
            }

            if (draging)
                Debug.Log("Input draging"+ InputManager.GetInputPosition().ToString()+ "in "+detectionZoneMask.rect.size.ToString());
        }
        else if (InputManager.GetInputHold(0) && draging)       // Input.GetMouseButton(0) 当0键被按住持续侦测(包含down和up各一次)
        {
            if (routatingAround == axis.empty && offset.magnitude > 0.001f){
                routatingAround = Mathf.Abs(offset.x) >= Mathf.Abs(offset.y) ? axis.y : axis.normalInXZ;
                axisToRoutateAround = (routatingAround != axis.empty && Mathf.Abs(offset.x) >= Mathf.Abs(offset.y))? Vector3.up : Vector3.Cross(camTransform.position - target.position, Vector3.down);
                if(Mathf.Abs(offset.x) >= Mathf.Abs(offset.y)){
                    Debug.Log("Detect x>=y "+offset.ToString());
                }else{
                    Debug.Log("Detect x<y " + offset.ToString());
                }
            }

            if (routatingAround == axis.y)
            {
                // Routate around the y axis
                camTransform.RotateAround(target.position, axisToRoutateAround, Time.deltaTime * offset.x * speedX);
            }
            else if (routatingAround == axis.normalInXZ){
                float cosAngleBetweenCamRayAndXZPlane = 0f;
                cosAngleBetweenCamRayAndXZPlane = Vector3.Dot(Vector3.up, camTransform.position - target.position)/(camTransform.position - target.position).magnitude;
                if ((cosAngleBetweenCamRayAndXZPlane >= 0.9f && offset.y < 0 || (cosAngleBetweenCamRayAndXZPlane <=-0.9f && offset.y > 0)))
                    offset.y = 0;
                camTransform.RotateAround(target.position, axisToRoutateAround, Time.deltaTime * offset.y * speedY);
            }
        }
        else if (InputManager.GetInputUp(0) && draging)  //Input.GetMouseButtonUp(0) 当0键放开一次
        {
            routatingAround = axis.empty;
            axisToRoutateAround = Vector3.zero;
            draging = false;
        }
        previousPosition = Input.mousePosition;
    }
}
