using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[CustomEditor(typeof(ControllerMouse))]
[CanEditMultipleObjects]
public class CustomInspectorControllerMouse : Editor
{

    private bool showevents;

    private GUIStyle labelBackgroundStyle;

    SerializedProperty MoveCursor_Begin;
    SerializedProperty MoveCursor_While;
    SerializedProperty MoveCursor_End;

    SerializedProperty Click_Down;
    SerializedProperty Click_Up;

    void OnEnable()
    {
        labelBackgroundStyle = new GUIStyle();
        labelBackgroundStyle.normal = new GUIStyleState();
        labelBackgroundStyle.normal.textColor = Color.white;
        var texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, new Color(1f, 1f, 1f, 0.1f));
        texture.Apply();
        labelBackgroundStyle.normal.background = texture;

        // Fetch the objects from the GameObject script to display in the inspector
        MoveCursor_Begin = serializedObject.FindProperty("OnStartedCursorMove_WithController");
        MoveCursor_While = serializedObject.FindProperty("OnCursorMove_WithController");
        MoveCursor_End = serializedObject.FindProperty("OnEndedCursorMove_WithController");

        Click_Down = serializedObject.FindProperty("OnLeftClickDown_WithController");
        Click_Up = serializedObject.FindProperty("OnLeftClickUp_WithController");
    }

    private void BarLabel(string labelText)
    {
        var defaultColor = GUI.contentColor;
        GUILayout.BeginHorizontal(labelBackgroundStyle);
        GUI.contentColor = Color.white;
        GUILayout.Label(labelText, EditorStyles.boldLabel);
        GUI.contentColor = defaultColor;
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
    }

    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();

        ControllerMouse _controllerMouseScript = (ControllerMouse)target;

        GUILayout.Space(5);
        _controllerMouseScript._dontDestroy = EditorGUILayout.ToggleLeft(new GUIContent("Never destroy this object", "Sets DontDestroy(this) and destroys all other instances of this component, should there be more than one in the scene."), _controllerMouseScript._dontDestroy, EditorStyles.boldLabel);
        GUILayout.Space(5);
        _controllerMouseScript._debugComments = EditorGUILayout.ToggleLeft(new GUIContent("Write debug to console", "Enable this to debug the component. Will not output in a build, no matter the value."), _controllerMouseScript._debugComments, EditorStyles.boldLabel);
        GUILayout.Space(5);
        _controllerMouseScript._cursorBehaviour = (ControllerMouse.ControllerMouseSpeedBehaviour)GUILayout.Toolbar((int)_controllerMouseScript._cursorBehaviour, new string[] { ((ControllerMouse.ControllerMouseSpeedBehaviour)0).ToString(), ((ControllerMouse.ControllerMouseSpeedBehaviour)1).ToString() });
        GUILayout.Space(5);
        _controllerMouseScript.ControllerMouseSpeed = EditorGUILayout.FloatField(new GUIContent("Controller Mouse Speed"), _controllerMouseScript.ControllerMouseSpeed);
        GUILayout.Space(5);
        _controllerMouseScript.StickDriftThreshhold = EditorGUILayout.FloatField(new GUIContent("Stick Drift Threshhold"), _controllerMouseScript.StickDriftThreshhold);
        GUILayout.Space(5);
        _controllerMouseScript.TimeBeforeStopMovingStick = EditorGUILayout.FloatField(new GUIContent("Seconds to wait before stopping movement on release"), _controllerMouseScript.TimeBeforeStopMovingStick);


        GUILayout.Space(5);
        showevents = EditorGUILayout.Foldout(showevents, "Mouse Simulation Events");
        if (showevents)
        {
            GUILayout.Space(10);
            BarLabel("Cursor Movement");
            EditorGUILayout.PropertyField(MoveCursor_Begin, new GUIContent("On Started Moving Cursor With Controller"));
            EditorGUILayout.PropertyField(MoveCursor_While, new GUIContent("While Moving Cursor With Controller"));
            EditorGUILayout.PropertyField(MoveCursor_End, new GUIContent("On Ended Moving Cursor With Controller"));

            GUILayout.Space(10);
            BarLabel("Mouse Click");
            EditorGUILayout.PropertyField(Click_Down, new GUIContent("On LeftMouseDown With Controller"));
            EditorGUILayout.PropertyField(Click_Up, new GUIContent("On LeftMouseUp With Controller"));

        }
        GUILayout.Space(5);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(_controllerMouseScript);
            serializedObject.ApplyModifiedProperties();
        }
    }

}
#endif


// I haven't implemented MacOS mouse yet. This is the reference I plan to use:
// https://discussions.unity.com/t/how-can-i-control-the-mouse-move-and-send-mouse-click-signals-via-the-keyboard/85297/3
// If you beat me to it, please send your script to me @ customerhelp.a1a3@gmail.com ! I will add it as an update.
// Also, feel free to suggest improvements to the code !

[System.Serializable]
public class ControllerMouse : MonoBehaviour
{
    public static bool IsRebinding { get; set; }
    // The reason this is set to false is so that you can use the script
    // in level scenes when testing, and not have it cause errors
    // when you load the scene from your menu where you also have the script.
    [SerializeField] public bool _dontDestroy = false;

    // This is just for debugging if you can't get it to work.
    // Since it should work, this should theoretically never have to be used.
    [SerializeField] public bool _debugComments = false;
    private bool DebugComments
    {
        get
        {
            // It's your choice if you want console output in the editor
#if UNITY_EDITOR
            return _debugComments;
#endif
            // When we aren't using the editor (fx. in a build) it will never output to console
            return false;
        }
    }

    // All of this is just to make the inspector more readable. It could literally just be a bool.
    public enum ControllerMouseSpeedBehaviour { SensitiveSpeed, ConstantSpeed, }
    [SerializeField] public ControllerMouseSpeedBehaviour _cursorBehaviour;
    private bool ConstantMouseSpeed { get => _cursorBehaviour == ControllerMouseSpeedBehaviour.ConstantSpeed; }
    [field: SerializeField] public float ControllerMouseSpeed { get; set; } = 8f;

    // How many seconds after releasing the stick should we stop moving?
    // (Stick issues can trigger release on random frames if this isn't above 0)
    [field: SerializeField] public float TimeBeforeStopMovingStick { get; set; } = 0.05f;

    // How large must the magnitude of the stick be before we count it as an intentional move?
    [field: SerializeField] public float StickDriftThreshhold { get; set; } = 0.05f;

    // This is only used in one calculation. I'm not sure if it saves memory to reuse it or if it could just be replace with var ? - Better safe than sorry
    private Vector2 mousePosition;
    // Controller stick direction
    private Vector2 mouseDirection;


    // ---------------------------------------------------------------------------------------
    // These should be properties but Custom Editor can't display properties for some reason
    // ---------------------------------------------------------------------------------------

    // Cursor Move With Controller
    [field: SerializeField] public UnityEvent<Vector2> OnStartedCursorMove_WithController = new UnityEvent<Vector2>();
    [field: SerializeField] public UnityEvent<Vector2> OnCursorMove_WithController = new UnityEvent<Vector2>();
    [field: SerializeField] public UnityEvent OnEndedCursorMove_WithController = new UnityEvent();

    // Cursor Click With Controller
    [field: SerializeField] public UnityEvent OnLeftClickDown_WithController = new UnityEvent();
    [field: SerializeField] public UnityEvent OnLeftClickUp_WithController = new UnityEvent();


    // Singleton that isn't really a singleton.. hmm...
    private static ControllerMouse instance;
    private void Awake()
    {
        // Keep this instance in all scenes
        if (_dontDestroy)
            DontDestroyOnLoad(this);
        // Remove other instances if we are keeping one instance
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        //_rebindMouseController = this.GetComponent<RebindMouseController>();
        //_rebindMouseController.FakeAwake(OnSimulateMouse, OnLeftClick);
    }


    private void Update()
    {
        if (IsRebinding) return;
        if (stoppedMovingStick)
        {
            stoppedMovingStickTimer += Time.deltaTime;
            if (stoppedMovingStickTimer >= TimeBeforeStopMovingStick)
            {
                stoppedMovingStick = false;
                stoppedMovingStickTimer = 0f;
                // behaviour
                isMovingControllerMouse = false;
                OnEndedCursorMove_WithController.Invoke();
                if (DebugComments)
                    Debug.LogError("Stopped moving cursor by controller.");
            }
        }

        if (isMovingControllerMouse)
        {
            mousePosition = (Vector2)Input.mousePosition + mouseDirection * ControllerMouseSpeed * Time.deltaTime * 100f;
            Mouse.current.WarpCursorPosition(mousePosition);
        }
    }

    // to prevent activation by stick drift i do this manually instead of with context.started
    // if you have stick drift it's always receiving a number
    private bool isMovingControllerMouse = false;
    // broken controllers set passthrough to 0 for weird frames, this counteracts that issue
    private bool stoppedMovingStick = false;
    private float stoppedMovingStickTimer = 0f;

    public void OnSimulateMouse(InputAction.CallbackContext context)
    {
        if (IsRebinding) return;
        var vec = context.ReadValue<Vector2>();
        if (ConstantMouseSpeed)
            vec = vec.normalized;

        if (vec.magnitude > StickDriftThreshhold)
        {
            if (!isMovingControllerMouse)
            {
                OnStartedCursorMove_WithController.Invoke(vec);
                if (DebugComments)
                    Debug.LogWarning("Started moving cursor by controller | " + vec);
            }
            isMovingControllerMouse = true;
            stoppedMovingStick = false;
            // This line could be in started moving but i'm not taking any chances
            stoppedMovingStickTimer = 0f;
            mouseDirection = vec;
            OnCursorMove_WithController.Invoke(vec);
            //if (DebugComments)
            //    Debug.Log("Is moving cursor by controller | " + vec);
        }
        else
        {
            if (isMovingControllerMouse)
            {
                stoppedMovingStick = true;
                //Debug.LogWarning("released stick: " + vec.magnitude);
            }
        }
    }

    // context.performed isn't working for this either. I have no clue.
    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (IsRebinding) return;

        // 1 is down, 0 is up
        var clickID = context.ReadValue<Single>();
        if (clickID == 1)
        {
            OnLeftClickDown_WithController.Invoke();
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
            if (DebugComments)
                Debug.Log("Clicked MouseLeftDown as controller CLICK phase: " + context.phase);
        }
        if (clickID == 0)
        {
            OnLeftClickUp_WithController.Invoke();
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
            if (DebugComments)
                Debug.Log("Clicked MouseLeftUp as controller CLICK phase: " + context.phase);
        }
    }


}
