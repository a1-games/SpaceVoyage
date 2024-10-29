using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputBinding;

public class MouseControllerRebindKey : MonoBehaviour
{

    [Tooltip("Reference to action that is to be rebound from the UI.")]
    [SerializeField]
    private InputActionReference m_Action;

    [SerializeField]
    private string m_BindingId;

    //[SerializeField]
    //private InputBinding.DisplayStringOptions m_DisplayStringOptions;


    [Tooltip("Text label that will receive the current, formatted binding string.")]
    [SerializeField]
    private TMP_Text m_BindingText;

    [Tooltip("Optional UI that will be shown while a rebind is in progress.")]
    [SerializeField]
    private GameObject m_RebindOverlay;

    [Tooltip("Optional  TMP_Text label that will be updated with prompt for user input.")]
    [SerializeField]
    private TMP_Text m_RebindText;

    [Tooltip("Event that is triggered when the way the binding is display should be updated. This allows displaying "
        + "bindings in custom ways, e.g. using images instead of text.")]
    [SerializeField]
    private UpdateBindingUIEvent m_UpdateBindingUIEvent;

    public UpdateBindingUIEvent updateBindingUIEvent { get => m_UpdateBindingUIEvent; }

    [Tooltip("Event that is triggered when an interactive rebind is being initiated. This can be used, for example, "
        + "to implement custom UI behavior while a rebind is in progress. It can also be used to further "
        + "customize the rebind.")]
    [SerializeField]
    private InteractiveRebindEvent m_RebindStartEvent;

    [Tooltip("Event that is triggered when an interactive rebind is complete or has been aborted.")]
    [SerializeField]
    private InteractiveRebindEvent m_RebindStopEvent;

    [Tooltip("Event that returns a string with information as to why the rebind was aborted")]
    [SerializeField]
    private UnityEvent<string> m_OnAbortedMessage;

    private InputActionRebindingExtensions.RebindingOperation m_RebindOperation;

    private static List<MouseControllerRebindKey> s_RebindActionUIs;

    [Serializable]
    public class UpdateBindingUIEvent : UnityEvent<MouseControllerRebindKey, string, string, string> { }

    [Serializable]
    public class InteractiveRebindEvent : UnityEvent<MouseControllerRebindKey, InputActionRebindingExtensions.RebindingOperation> { }

    // We want the label for the action name to update in edit mode, too, so
    // we kick that off from here.
#if UNITY_EDITOR
    protected void OnValidate()
    {
        UpdateBindingDisplay();
    }

#endif


    /// <summary>
    /// Reference to the action that is to be rebound.
    /// </summary>
    public InputActionReference actionReference
    {
        get => m_Action;
        set
        {
            m_Action = value;
            UpdateBindingDisplay();
        }
    }

    /// <summary>
    /// ID (in string form) of the binding that is to be rebound on the action.
    /// </summary>
    /// <seealso cref="InputBinding.id"/>
    public string bindingId
    {
        get => m_BindingId;
        set
        {
            m_BindingId = value;
            UpdateBindingDisplay();
        }
    }

    public TMP_Text bindingText { get => m_BindingText; }

    /// <summary>
    /// Trigger a refresh of the currently displayed binding.
    /// </summary>
    public void UpdateBindingDisplay()
    {
        var displayString = string.Empty;
        var deviceLayoutName = default(string);
        var controlPath = default(string);

        // Get display string from action.
        var action = m_Action?.action;
        if (action != null)
        {
            var bindingIndex = action.bindings.IndexOf(x => x.id.ToString() == m_BindingId);
            if (bindingIndex != -1)
                displayString = action.GetBindingDisplayString(bindingIndex, out deviceLayoutName, out controlPath, /*displayStringOptions*/DisplayStringOptions.DontUseShortDisplayNames);
        }

        // Set on label
        if (m_BindingText != null)
            m_BindingText.text = displayString;

        // Give listeners a chance to configure UI in response.
        m_UpdateBindingUIEvent?.Invoke(this, displayString, deviceLayoutName, controlPath);
    }




    /// <summary>
    /// Return the action and binding index for the binding that is targeted by the component
    /// according to
    /// </summary>
    /// <param name="action"></param>
    /// <param name="bindingIndex"></param>
    /// <returns></returns>
    public bool ResolveActionAndBinding(out InputAction action, out int bindingIndex)
    {
        bindingIndex = -1;

        action = m_Action?.action;
        if (action == null)
            return false;

        if (string.IsNullOrEmpty(m_BindingId))
            return false;

        // Look up binding index.
        var bindingId = new Guid(m_BindingId);
        bindingIndex = action.bindings.IndexOf(x => x.id == bindingId);
        if (bindingIndex == -1)
        {
            Debug.LogError($"Cannot find binding with ID '{bindingId}' on '{action}'", this);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Remove currently applied binding overrides.
    /// </summary>
    public void ResetToDefault()
    {
        if (!ResolveActionAndBinding(out var action, out var bindingIndex))
            return;

        if (action.bindings[bindingIndex].isComposite)
        {
            // It's a composite. Remove overrides from part bindings.
            for (var i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; ++i)
                action.RemoveBindingOverride(i);
        }
        else
        {
            action.RemoveBindingOverride(bindingIndex);
        }
        UpdateBindingDisplay();
    }


    /// <summary>
    /// Initiate an interactive rebind that lets the player actuate a control to choose a new binding
    /// for the action.
    /// </summary>
    public void StartInteractiveRebind()
    {
        if (!ResolveActionAndBinding(out var action, out var bindingIndex))
            return;
        // Disable cursor simulation when waiting on keybind
        EnableCursorSimulation(false, action);

        // If the binding is a composite, we need to rebind each part in turn.
        if (action.bindings[bindingIndex].isComposite)
        {
            var firstPartIndex = bindingIndex + 1;
            if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
                PerformInteractiveRebind(action, firstPartIndex, allCompositeParts: true);
        }
        else
        {
            PerformInteractiveRebind(action, bindingIndex);
        }
    }

    private void EnableCursorSimulation(bool doEnable, InputAction action)
    {
        RebindMouseController.IsRebinding = !doEnable;
        //Debug.LogError("IsRebinding=" + RebindMouseController.IsRebinding);
        // Allows rebinding the active action
        if (doEnable)
            action.Enable();
        else
            action.Disable();
        //Debug.LogWarning(" action is enabled: " + action.enabled);
    }

    private void PerformInteractiveRebind(InputAction action, int bindingIndex, bool allCompositeParts = false)
    {
        // checking for the same keypress twice in a composite
        string lastKeysName = "";
        if (allCompositeParts)
            lastKeysName = action.bindings[bindingIndex - 1].name;
        


        m_RebindOperation?.Cancel(); // Will null out m_RebindOperation.
        //Debug.LogWarning(" action is enabled: " + action.enabled);

        void CleanUp()
        {
            m_RebindOperation?.Dispose();
            m_RebindOperation = null;
        }

        // Configure the rebind.
        m_RebindOperation = action.PerformInteractiveRebinding(bindingIndex)
                                  .WithControlsExcluding("<Pointer>/delta")
                                  .WithControlsExcluding("<Pointer>/position")
                                  .WithControlsExcluding("<Touchscreen>/touch*/position")
                                  .WithControlsExcluding("<Touchscreen>/touch*/delta")
                                  .WithControlsExcluding("<Mouse>/clickCount")
                                  .OnPotentialMatch((rebindOperation) =>
                                  {
                                      if (rebindOperation == null || !allCompositeParts) return;
                                      foreach (var canditate in rebindOperation.candidates)
                                      {
                                          // If we are making a composite and use the same key twice
                                          if (canditate.name.Equals(lastKeysName))
                                          {
                                              m_OnAbortedMessage.Invoke("Tried to use the same key twice in a composite.");
                                              rebindOperation.Cancel();
                                          }
                                      }
                                  });
                                  //.WithMatchingEventsBeingSuppressed();

        if (allCompositeParts)
            m_RebindOperation = m_RebindOperation.OnMatchWaitForAnother(0.35f);

        m_RebindOperation = m_RebindOperation.OnCancel(operation =>
                {
                    m_RebindStopEvent?.Invoke(this, operation);
                    m_RebindOverlay?.SetActive(false);
                    UpdateBindingDisplay();
                    CleanUp();
                    EnableCursorSimulation(true, action);
                });

        m_RebindOperation = m_RebindOperation.OnComplete(
                operation =>
                {
                    m_RebindOverlay?.SetActive(false);
                    m_RebindStopEvent?.Invoke(this, operation);
                    UpdateBindingDisplay();
                    CleanUp();

                    // If there's more composite parts we should bind, initiate a rebind
                    // for the next part.
                    if (allCompositeParts)
                    {
                        var nextBindingIndex = bindingIndex + 1;
                        if (nextBindingIndex < action.bindings.Count && action.bindings[nextBindingIndex].isPartOfComposite)
                            PerformInteractiveRebind(action, nextBindingIndex, true);
                        else // once we have gone through all keys in the composite
                            EnableCursorSimulation(true, action);
                    }
                    else // if we are not a composite
                    {
                        EnableCursorSimulation(true, action);
                    }
                });

        // If it's a part binding, show the name of the part in the UI.
        var partName = default(string);
        if (action.bindings[bindingIndex].isPartOfComposite)
            partName = $"Binding '{action.bindings[bindingIndex].name}'. ";

        // Bring up rebind overlay, if we have one.
        m_RebindOverlay?.SetActive(true);
        if (m_RebindText != null)
            m_RebindText.text = $"{partName}Waiting for input...";
        m_BindingText.text = "< Waiting... >";


        // Give listeners a chance to act on the rebind starting.
        m_RebindStartEvent?.Invoke(this, m_RebindOperation);

        m_RebindOperation.Start();
    }

    protected void OnEnable()
    {
        if (s_RebindActionUIs == null)
            s_RebindActionUIs = new List<MouseControllerRebindKey>();
        s_RebindActionUIs.Add(this);
        if (s_RebindActionUIs.Count == 1)
            InputSystem.onActionChange += OnActionChange;
    }

    protected void OnDisable()
    {
        m_RebindOperation?.Dispose();
        m_RebindOperation = null;

        s_RebindActionUIs.Remove(this);
        if (s_RebindActionUIs.Count == 0)
        {
            s_RebindActionUIs = null;
            InputSystem.onActionChange -= OnActionChange;
        }
    }

    // When the action system re-resolves bindings, we want to update our UI in response. While this will
    // also trigger from changes we made ourselves, it ensures that we react to changes made elsewhere. If
    // the user changes keyboard layout, for example, we will get a BoundControlsChanged notification and
    // will update our UI to reflect the current keyboard layout.
    private static void OnActionChange(object obj, InputActionChange change)
    {
        if (change != InputActionChange.BoundControlsChanged)
            return;

        var action = obj as InputAction;
        var actionMap = action?.actionMap ?? obj as InputActionMap;
        var actionAsset = actionMap?.asset ?? obj as InputActionAsset;

        for (var i = 0; i < s_RebindActionUIs.Count; ++i)
        {
            var component = s_RebindActionUIs[i];
            var referencedAction = component.actionReference?.action;
            if (referencedAction == null)
                continue;

            if (referencedAction == action ||
                referencedAction.actionMap == actionMap ||
                referencedAction.actionMap?.asset == actionAsset)
                component.UpdateBindingDisplay();
        }
    }


}
