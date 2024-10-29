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
            //_script.AddClickBinding("<Gamepad>/rightShoulder");
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
    public static bool IsRebinding { get; set; }




    
    


}
