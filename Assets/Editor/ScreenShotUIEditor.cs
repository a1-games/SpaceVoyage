using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Screenshot))]
public class ScreenShotUIEditor : Editor
{
    [SerializeField] public SerializedProperty onScreenshotTakenEvent;

    private void OnEnable()
    {
        onScreenshotTakenEvent = serializedObject.FindProperty("onScreenshotTaken");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var script = (Screenshot)target;

        // title
        EditorGUILayout.LabelField("Screenshot Settings:", EditorStyles.boldLabel);
        // indent everything under this title
        EditorGUI.indentLevel++;

        // default screenshot png name
        script.defaultScreenshotName = EditorGUILayout.TextField("Default Screenshot Name", script.defaultScreenshotName);

        // custom resolution options
        script.useCustomResolution = EditorGUILayout.ToggleLeft("Custom Resolution:", script.useCustomResolution);

        if (script.useCustomResolution)
        {
            EditorGUI.indentLevel++;
            script.resX = EditorGUILayout.IntField("Width:", script.resX, GUILayout.ExpandWidth(false));
            script.resY = EditorGUILayout.IntField("Height:", script.resY, GUILayout.ExpandWidth(false));
            EditorGUI.indentLevel--;
        }
        else
        {
            //EditorGUILayout.EndHorizontal();
            EditorGUILayout.HelpBox("Screenshots will use the screen resolution by default.", MessageType.Info);
        }

        // make space before the next subject
        EditorGUILayout.Separator();

        // remove indentation before new title
        EditorGUI.indentLevel--;
        // title
        EditorGUILayout.LabelField("Input System:", EditorStyles.boldLabel);
        // indent everything under this title
        EditorGUI.indentLevel++;

        // a little space between
        EditorGUILayout.Separator();

        // input system stuff
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Use old input system?");
        script.useOldInputSystem = EditorGUILayout.Toggle(script.useOldInputSystem);
        EditorGUILayout.EndHorizontal();

        // a little space between
        EditorGUILayout.Separator();
        if (script.useOldInputSystem)
        {
            script.screenshotKeybind = (KeyCode)EditorGUILayout.EnumPopup("Key", script.screenshotKeybind);
        }
        else
        {
            EditorGUILayout.HelpBox("To use the new input system, set it to use Unity Events and drag the object this script is attached to into the event box for your configured screenshot key.\nIn the dropdown, select the Screenshot script. \"ScreenShotKeybind()\" should be visible at the top under dynamic functions. Select this and you should be set.", MessageType.None);
        }

        // make space before the next subject
        EditorGUILayout.Separator();
        // indent everything under this title
        EditorGUI.indentLevel--;

        // hide devtools? + title
        script.hideDeveloperTools = EditorGUILayout.ToggleLeft("Developer Tools:", script.hideDeveloperTools, EditorStyles.boldLabel);

        // return if true so we dont draw the rest
        if (!script.hideDeveloperTools) return;

        // a little space between
        EditorGUILayout.Separator();

        EditorGUILayout.HelpBox("The script uses the \"Assets/Resources/\" folder to load the screenshots. I would not advise changing its location.", MessageType.Warning);

        // a little space between
        EditorGUILayout.Separator();

        if (GUILayout.Button("Take Screenshot (Only in playmode)"))
        {
            script.TakeScreenshot();
        }

        // a little space between
        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("On Screenshot Taken Event:", EditorStyles.boldLabel);
        
        EditorGUILayout.HelpBox("Returns a Texture2D of the screenshot that was taken. You could for example use this to show a pop up of the taken screenshot.\nDrag your own script into the event here and your function will trigger every time the user takes a screenshot.", MessageType.Info);
        
        // a little space between
        EditorGUILayout.Separator();

        EditorGUILayout.PropertyField(onScreenshotTakenEvent, new GUIContent("OnScreenshotTakenEvent"));

        // a little space between
        EditorGUILayout.Separator();
        EditorGUI.indentLevel--;

        EditorGUILayout.LabelField("Access all screenshots:", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Reference the Screenshot script and access the function called \"GetAllScreenshots\". It returns a List<Texture2D> with all screenshots. Use \"GetAllScreenshotsAsArray\" for the same list as array.", MessageType.Info);


        serializedObject.ApplyModifiedProperties();
    }
}
