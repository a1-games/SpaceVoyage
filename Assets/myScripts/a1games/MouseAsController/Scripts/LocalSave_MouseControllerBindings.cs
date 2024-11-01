
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;


public enum MouseSimulationKeybind
{
    LeftClickPrimary,
    LeftClickSecondary,
    CursorMovePrimary,
    CursorMoveSecondary,
}

// This class is meant for you to be able to edit yourself to fit into your own system.
// You can also use as is, playerprefs is a fine solution for keybinds.

public class LocalSave_MouseControllerBindings : MonoBehaviour
{



    //private static void AddOverrideToDictionary(Guid actionId, string path, int bindingIndex)
    //{
    //    string key = string.Format("{0} : {1}", actionId.ToString(), bindingIndex);

    //    if (OverridesDictionary.ContainsKey(key))
    //    {
    //        OverridesDictionary[key] = path;
    //    }
    //    else
    //    {
    //        OverridesDictionary.Add(key, path);
    //    }
    //}

    //private static void SaveControlOverrides()
    //{
    //    FileStream file = new FileStream(Application.persistentDataPath + "/controlsOverrides.dat", FileMode.OpenOrCreate);
    //    BinaryFormatter bf = new BinaryFormatter();
    //    bf.Serialize(file, OverridesDictionary);
    //    file.Close();
    //}

    //private static void LoadControlOverrides()
    //{
    //    if (!File.Exists(Application.persistentDataPath + "/controlsOverrides.dat"))
    //    {
    //        return;
    //    }

    //    FileStream file = new FileStream(Application.persistentDataPath + "/controlsOverrides.dat", FileMode.OpenOrCreate);
    //    BinaryFormatter bf = new BinaryFormatter();
    //    OverridesDictionary = bf.Deserialize(file) as Dictionary<string, string>;
    //    file.Close();

    //    foreach (var item in OverridesDictionary)
    //    {
    //        string[] split = item.Key.Split(new string[] { " : " }, StringSplitOptions.None);
    //        Guid id = Guid.Parse(split[0]);
    //        int index = int.Parse(split[1]);
    //        instance._actions.FindAction(id).ApplyBindingOverride(index, item.Value);
    //    }
    //}



    private static LocalSave_MouseControllerBindings instance;

    [SerializeField] private InputActionAsset _actions;

    private Coroutine saveAllRoutine = null;

    private void Awake()
    {
        instance = this;
    }

    //private void OnEnable()
    //{
    //    var rebinds = PlayerPrefs.GetString("MOUSECONTROLLER_KEYBINDS");
    //    if (!string.IsNullOrEmpty(rebinds))
    //        _actions.LoadFromJson(rebinds);
    //}

    private static InputBinding FindKeyBind(string keybindID)
    {
        for (int i = 0; i < instance._actions.actionMaps.Count; i++)
        {
            var acMap = instance._actions.actionMaps[i];
            for (int j = 0; j < acMap.actions.Count; j++)
            {
                for (int k = 0; k < acMap.actions[j].bindings.Count; k++)
                {
                    if (keybindID.Equals(acMap.actions[j].bindings[k].id.ToString()))
                    {
                        print(acMap.actions[j].bindings[k].name + ": " + acMap.actions[j].bindings[k].id);
                        return acMap.actions[j].bindings[k];
                    }
                }
            }
        }
        return default;
    }

    public static void LoadAndSetKeyOverride(MouseSimulationKeybind key)
    {
        var controlPath = GetKeybindPath(key);
        var keybindID = GetKeybindBindingID(key);
        var keybindIndex = GetKeybindBindingIndex(key);

        if (string.IsNullOrEmpty(controlPath) || string.IsNullOrEmpty(keybindID)) return;


        var action = FindKeyBind(keybindI);

        if (action == null) return;
        instance._actions.FindAction(keybindID).ApplyBindingOverride(keybindIndex, controlPath);
    }

    public static void SaveAll()
    {
        instance.QueueSaveAll();
    }

    private void QueueSaveAll()
    {
        if (saveAllRoutine != null)
            StopCoroutine(saveAllRoutine);

        saveAllRoutine = StartCoroutine(WaitThenSaveAll());
    }

    // I don't like waiting until the game closes to save the keybinds
    private IEnumerator WaitThenSaveAll()
    {
        yield return new WaitForSecondsRealtime(2f);
        //Debug.LogWarning("Waited, then saved all bindings to playerprefs");
        //var rebinds = _actions.ToJson();
        //PlayerPrefs.SetString("MOUSECONTROLLER_KEYBINDS", rebinds);
        PlayerPrefs.Save();
    }


    public static void SaveKey(MouseSimulationKeybind key, string inputBindingID, string controlPath, int bindingIndex)
    {
        if (instance == null) throw new NullReferenceException("This script isn't in your game scene");
        if (string.IsNullOrEmpty(inputBindingID) || string.IsNullOrEmpty(controlPath))
            return;

        // buttonSouth
        // 3b77a20c-3eb7-49b7-a34f-e7c0561c0023

        // rightTrigger
        // 2aea57f4-483e-4559-af23-a5c94b8f4133

        //Debug.Log("Tried to save " + key + " with path: " + controlPath + " and inputBindingID: " + inputBindingID);



        PlayerPrefs.SetString(key.ToString() + "_path", controlPath);
        PlayerPrefs.SetString(key.ToString() + "_bindingID", inputBindingID);
        PlayerPrefs.SetInt(key.ToString() + "_bindingIndex", bindingIndex);
        instance.QueueSaveAll();


        //instance._actions.FindAction(inputBindingID).ApplyBindingOverride(bindingIndex, controlPath);

    }



    public static string GetKeybindPath(MouseSimulationKeybind key)
    {
        return PlayerPrefs.GetString(key.ToString() + "_path");
    }
    public static string GetKeybindBindingID(MouseSimulationKeybind key)
    {
        return PlayerPrefs.GetString(key.ToString() + "_bindingID");
    }
    public static int GetKeybindBindingIndex(MouseSimulationKeybind key)
    {
        return PlayerPrefs.GetInt(key.ToString() + "_bindingIndex");
    }


}
