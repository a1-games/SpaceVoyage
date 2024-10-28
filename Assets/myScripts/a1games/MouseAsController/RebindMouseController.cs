using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

#if UNITY_EDITOR
[CustomEditor(typeof(RebindMouseController))]
public class CustomInspectorRebindMouseController : Editor
{


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RebindMouseController _script = (RebindMouseController)target;


        GUILayout.Space(20);

        if (GUILayout.Button(" ADD ITEM TESTING "))
        {
            _script.AddClickBinding("<Gamepad>/rightShoulder");
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(_script);
            serializedObject.ApplyModifiedProperties();
        }
    }

}
#endif
public class RebindMouseController : MonoBehaviour
{

    //[SerializeField] private InputActionAsset inputAction_SO;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputActionMap inputActionMap;





    private int playerInputEventCountOnAwake = -1;
    
    
    internal void FakeAwake(UnityAction<InputAction.CallbackContext> onCursorMovement, UnityAction<InputAction.CallbackContext> onClickSimulation)
    {
        if (playerInput == null)
        {
            throw new System.Exception("'playerInput' must be assigned in the inspector!");
        }

        inputActionMap = new InputActionMap("Mouse Simulation With Controller");
        playerInputEventCountOnAwake = playerInput.actionEvents.Count;

        // Moving the cursor
        inputActionMap.AddAction("Cursor Movement", InputActionType.Value, "<Gamepad>/leftStick", null, null, null, "Vector2");
        inputActionMap["Cursor Movement"].AddBinding("<Gamepad>/rightStick");

        // Simulating mouse click
        inputActionMap.AddAction("Mouse Click Simulation", InputActionType.PassThrough, "<Gamepad>/buttonSouth", null, null, null, "Button");
        inputActionMap["Mouse Click Simulation"].AddBinding("<Gamepad>/buttonEast");
        inputActionMap["Mouse Click Simulation"].AddBinding("<Gamepad>/rightTrigger");
        //inputActionMap["Mouse Click Simulation"].AddBinding("<Gamepad>/rightShoulder");

        playerInput.actions.AddActionMap(inputActionMap);
        playerInput.notificationBehavior = PlayerNotifications.InvokeUnityEvents;

        // It takes one frame, maybe more, for the actionmap to be fully added
        StartCoroutine(DoWhenControlsWereSuccesfullyAdded(onCursorMovement, onClickSimulation));
    }

    private IEnumerator DoWhenControlsWereSuccesfullyAdded(UnityAction<InputAction.CallbackContext> onCursorMovement, UnityAction<InputAction.CallbackContext> onClickSimulation)
    {
        // Wait for the add to complete
        while (playerInput.actionEvents.Count == playerInputEventCountOnAwake)
        {
            yield return null;
        }

        // We go backwards because we add onto the top. No reason to go through the rest.
        for (int i = playerInput.actionEvents.Count - 1; i >= playerInputEventCountOnAwake; i--)
        {
            var _event = playerInput.actionEvents[i];
            print("i:" + i + " | acName: " + _event.actionName);

            if (_event.actionName.Contains("Movement"))
            {
                print("made it into movement");
                _event.AddListener(onCursorMovement);
            }
            if (_event.actionName.Contains("Click"))
            {
                print("made it into click");
                _event.AddListener(onClickSimulation);
            }
        }
    }



    private void OnDisable()
    {
        playerInput.actions.RemoveActionMap(inputActionMap);
        //playerInput.SwitchCurrentActionMap(playerInput.actions.FindAction(0));
    }

    public void AddCursorMoveBinding(string bindingPath)
    {
        inputActionMap["Cursor Movement"].AddBinding(bindingPath);
        //EditorUtility.SetDirty(inputAction_SO);
    }

    public void AddClickBinding(string bindingPath)
    {
        inputActionMap["Mouse Click Simulation"].AddBinding(bindingPath);
        //EditorUtility.SetDirty(inputAction_SO);
    }


}
