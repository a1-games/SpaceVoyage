using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FreeTrialLevelName : MonoBehaviour
{
    [SerializeField] private TMP_Text[] titlesToReplace;
    [SerializeField] private TMP_Text stagenameToShow;

    private void Awake()
    {
        AddNameToText();
    }
    private void AddNameToText()
    {
        for (int i = 0; i < titlesToReplace.Length; i++)
        {
            titlesToReplace[i].text = stagenameToShow.text;
        }
    }
}
