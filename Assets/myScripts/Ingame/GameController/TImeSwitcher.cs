using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TImeSwitcher : MonoBehaviour
{
    //public Image uiFillElement;
    public TMPro.TMP_Text securityCamTimeStamp;
    [SerializeField] private Slider timeChangeSlider;
    private PlayerMovement playerMove;
    private Coroutine camTimeRoutine;
    private DateTime myDateTime;

    private void Awake()
    {
        Time.timeScale = 1f;
        //uiFillElement.fillAmount = 0.25f;
        myDateTime = DateTime.Now;
    }
    private void OnEnable()
    {
        StartSecurityCamTimer();
    }
    private void OnDisable()
    {
        StopCoroutine(camTimeRoutine);
    }
    private void Start()
    {
        playerMove = IngameController.AskFor.PlayerObject.GetComponent<PlayerMovement>();
        if (GameSettings.AskFor.GetAlwaysMaxGameSpeed())
        {
            if (SceneLoader.AskFor.isDemo) return;
            var max = timeChangeSlider.maxValue;
            timeChangeSlider.value = max;
            SwitchTimeSlider();
        }
    }
    public void SwitchTimeSlider()
    {
        var value = timeChangeSlider.value;
        Time.timeScale = value;
        IngameController.AskFor.CurrentTimeScale = value;
        //playerMove.MoveSpeedModifier = value;
    }

    public void StartSecurityCamTimer()
    {
        if (camTimeRoutine != null) return;
        camTimeRoutine = StartCoroutine(SecurityCamTimer());
    }

    private IEnumerator SecurityCamTimer()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1f);
            myDateTime.AddSeconds(1f);
            securityCamTimeStamp.text = myDateTime.ToString();
        }

    }
}
