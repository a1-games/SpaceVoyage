using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MouseSimulationKeybind
{
    LeftClickPrimary,
    LeftClickSecondary,
    CursorMovePrimary,
    CursorMoveSecondary,
}

// This class is meant for you to be able to edit yourself to fit into your own system.
// You can also use as is, playerprefs is a fine solution for keybinds.

public class SaveMouseControllerBindingsLocally : MonoBehaviour
{

    public void SaveKey(MouseSimulationKeybind key, string keybindPath)
    {
        PlayerPrefs.SetString(key.ToString(), keybindPath);
    }


    public string GetKeybindPath(MouseSimulationKeybind key)
    {
        return PlayerPrefs.GetString(key.ToString());
    }


}
