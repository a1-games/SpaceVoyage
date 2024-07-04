using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelUI : MonoBehaviour
{
    public UIState uiState;

    public void RefreshUI(UIState currentState)
    {
        if (currentState != uiState) this.gameObject.SetActive(false);
        else this.gameObject.SetActive(true);
    }
}
