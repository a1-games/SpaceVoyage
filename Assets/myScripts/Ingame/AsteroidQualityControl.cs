using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidQualityControl : MonoBehaviour
{
    [SerializeField] private int disableUnderThisLevel = 2;
    private void Start()
    {
        if (GameSettings.AskFor.GetQualityLevel() < disableUnderThisLevel)
            gameObject.SetActive(false);
    }
}
