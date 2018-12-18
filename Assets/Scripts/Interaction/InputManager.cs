using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(fileName = "InputManager", menuName = "Managers/InputManager", order = 1)]
public class InputManager : MonoBehaviour {

    public InputMethod inputMethod = InputMethod.Auto;

    public enum InputMethod
    {
        Mouse,
        Touch,
        TouchSimulatingMouse,
        Auto
    }

    enum axis
    {
        x,
        y,
        z,
        normalInXZ,
        empty
    }
    
    public delegate bool GetInputDownDelegate(int idx);
    public static GetInputDownDelegate GetInputDown;
    public delegate bool GetInputHoldDelegate(int idx);
    public static GetInputHoldDelegate GetInputHold;
    public delegate bool GetInputUpDelegate(int idx);
    public static GetInputUpDelegate GetInputUp;
    public delegate Vector3 GetInputPositionDelegate();
    public static GetInputPositionDelegate GetInputPosition;

    private void Awake()
    {
        if (inputMethod == InputMethod.Auto)
        {
            inputMethod = Input.touchSupported ? InputMethod.Touch : InputMethod.Mouse;
        }
        switch (inputMethod)
        {
            case InputMethod.Mouse:
                Input.simulateMouseWithTouches = false;
                GetInputDown = new GetInputDownDelegate(Input.GetMouseButtonDown);
                GetInputUp = new GetInputUpDelegate(Input.GetMouseButtonUp);
                GetInputHold = new GetInputHoldDelegate(Input.GetMouseButton);
                GetInputPosition = new GetInputPositionDelegate(() => Input.mousePosition);
                break;
            case InputMethod.Touch:
                Input.simulateMouseWithTouches = false;
                ///<summary>
                /// 踩了一个坑，在C#里，匿名函数的返回值是一个布尔运算式的时候，其中的 <c>&&</c> 不会触发短路逻辑，需要用 <c>(condition1)?(condition2):false</c> 代替
                /// 实际上是因为 A && B == C 的时候，尽管 B 可能不是 bool，但是考虑到隐式转换，仍然按照 == 的高优先级计算
                /// </summary>
                //getInputDown = new GetInputDown((int idx) => Input.touchCount == 1 ? Input.GetTouch(idx).phase == TouchPhase.Began : false);
                GetInputDown = new GetInputDownDelegate((int idx) => (Input.touchCount == 1) && (Input.GetTouch(idx).phase == TouchPhase.Began));
                GetInputUp = new GetInputUpDelegate((int idx) => Input.touchCount == 1 ? Input.GetTouch(idx).phase == TouchPhase.Ended || Input.GetTouch(idx).phase == TouchPhase.Canceled : false);
                GetInputHold = new GetInputHoldDelegate((int idx) => Input.touchCount == 1 ? Input.GetTouch(idx).phase == TouchPhase.Stationary || Input.GetTouch(idx).phase == TouchPhase.Moved : false);
                GetInputPosition = new GetInputPositionDelegate(() => Input.touchCount == 1 ? new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y) : Vector3.zero);
                break;
            case InputMethod.TouchSimulatingMouse:
                Input.simulateMouseWithTouches = true;
                GetInputDown = new GetInputDownDelegate(Input.GetMouseButtonDown);
                GetInputUp = new GetInputUpDelegate(Input.GetMouseButtonUp);
                GetInputHold = new GetInputHoldDelegate(Input.GetMouseButton);
                GetInputPosition = new GetInputPositionDelegate(() => Input.mousePosition);
                break;
        }
    }
}
