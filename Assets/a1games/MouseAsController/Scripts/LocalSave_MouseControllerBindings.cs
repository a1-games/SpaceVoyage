
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// This class is meant for you to be able to edit yourself to fit into your own system.
// You can also use as is, playerprefs is a fine solution for keybinds.
public class LocalSave_MouseControllerBindings : MonoBehaviour
{

    [SerializeField] private InputActionAsset _actions;

    private Coroutine saveAllRoutine = null;

    private void OnEnable()
    {
        // Load saved custom keybind
        LoadAndSetKeyOverride();
    }

    public void LoadAndSetKeyOverride()
    {
        string keybindsAsJson = GetAllBinds();
        if (keybindsAsJson != null)
            _actions.LoadBindingOverridesFromJson(keybindsAsJson);
    }
    public void QueueSaveAll()
    {
        if (saveAllRoutine != null)
            StopCoroutine(saveAllRoutine);

        saveAllRoutine = StartCoroutine(WaitThenSaveAll());
    }
    // I don't like waiting until the game closes to save the keybinds, and this prevents spamming
    private IEnumerator WaitThenSaveAll()
    {
        yield return new WaitForSecondsRealtime(2f);
        //Debug.LogWarning("Waited, then saved all bindings to playerprefs");
        //var rebinds = _actions.ToJson();
        //PlayerPrefs.SetString("MOUSECONTROLLER_KEYBINDS", rebinds);
        PlayerPrefs.SetString("ALL_CONTROLLERMOUSE_KEYBINDS", _actions.SaveBindingOverridesAsJson());
        PlayerPrefs.Save();
    }
    public string GetAllBinds()
    {
        return PlayerPrefs.GetString("ALL_CONTROLLERMOUSE_KEYBINDS");
    }

}
