using System.Collections;
using System.Collections.Generic;
using UnityEditor.Scripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ControllerMouse : MonoBehaviour
{
    [SerializeField] private bool dontDestroy = false;

    [SerializeField] private float controllerMouseSpeed = 3f;
    private Vector2 mousePosition;
    private Vector2 mouseDirection;

    private static ControllerMouse instance;

    private void Awake()
    {
        if (dontDestroy)
        {
            DontDestroyOnLoad(this);
        }

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    private void FixedUpdate()
    {
        if (isMovingControllerMouse)
        {
            mousePosition += mouseDirection * controllerMouseSpeed;
            Mouse.current.WarpCursorPosition(mousePosition);
        }
    }

    // to prevent activation by stick drift i do this manually instead of with context.started
    private bool startedSimulateMouseThisFrame = false;
    private bool isMovingControllerMouse = false;
    private bool endedSimulateMouseThisFrame = false;

    public void OnSimulateMouse(InputAction.CallbackContext context)
    {
        var vec = context.ReadValue<Vector2>();

        if (vec.magnitude > 0f && !isMovingControllerMouse)
        {
            isMovingControllerMouse = true;
            startedSimulateMouseThisFrame = true;
        }
        else if (vec.magnitude <= 0f)
        {
            if (isMovingControllerMouse)
            {
                endedSimulateMouseThisFrame = true;
            }
            isMovingControllerMouse = false;
        }
        // made it to here, it almost works
        asjdioasjdiojasiodjoaisjd

        if(startedSimulateMouseThisFrame)
        {
            //mousePosition = (Vector2)Input.mousePosition;
            //isMovingControllerMouse = true;
            Debug.LogWarning("started");
        }

        // always do while moving
        if (vec.magnitude > 0f)
        {
            mouseDirection = vec;
            Debug.Log(vec);
        }

        if (endedSimulateMouseThisFrame)
        {
            //isMovingControllerMouse = false;
            Debug.LogError("canceled");
        }

        endedSimulateMouseThisFrame = false;
        startedSimulateMouseThisFrame = false;
    }

    public void OnLeftClick(InputAction.CallbackContext context)
    {
        //Mouse.current.IsPressed = true;
        //GameObject go = ExecuteEvents.GetEventHandler<IPointerClickHandler>();
    }



}
