using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeleporterTutorialManager : MonoBehaviour
{
    private static TeleporterTutorialManager instance;
    public static TeleporterTutorialManager AskFor { get => instance; }
    //------------------

    public int tutorialStep = 0;
    public bool cameraButtonHasBeenClicked;

    public MouseControl mouseControl;

    //public TMP_Text tutorialMessage;

    public StartPosition[] startPositions;

    public GameObject[] messages;
    public GameObject[] machineGuns;
    public GameObject[] teleporters;
    public GameObject TP_SHOWCASE;
    public GameObject showcaseShip_1;
    public GameObject showcaseShip_2;
    public GameObject crystal;
    public GameObject portal;
    public GameObject forcefield;
    public GameObject spaceship;
    public GameObject ui_CamToggle;
    public GameObject ui_StartMoving;
    public GameObject ui_TimeControl;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        TutorialCycle();
        
    }

    public void TutorialCycle()
    {

        switch (tutorialStep)
        {
            case 0: 
                {
                    if (true)
                    {
                        RefreshMessages(tutorialStep);

                        DisableAll();
                        foreach (var tp in teleporters)
                        {
                            tp.SetActive(true);
                        }

                        tutorialStep++;
                    }
                    break;
                }
            case 1: //showcase 1
                {
                    if (true)
                    {
                        RefreshMessages(tutorialStep);

                        DisableAll();
                        TP_SHOWCASE.SetActive(true);

                        tutorialStep++;
                    }
                    break;
                }
            case 2: //showcase 2
                {
                    if (true)
                    {
                        RefreshMessages(tutorialStep);

                        showcaseShip_1.transform.Rotate(Vector3.up, 180f);
                        showcaseShip_2.transform.Rotate(Vector3.up, 180f);

                        tutorialStep++;
                    }
                    break;
                }
            case 3: //enable everything
                {
                    if (true)
                    {
                        RefreshMessages(tutorialStep);

                        DisableAll();
                        spaceship.SetActive(true);
                        crystal.SetActive(true);
                        portal.SetActive(true);
                        forcefield.SetActive(true);
                        ui_CamToggle.SetActive(true);
                        ui_StartMoving.SetActive(true);
                        ui_TimeControl.SetActive(true);
                        foreach (var gun in machineGuns)
                        {
                            gun.SetActive(true);
                        }
                        foreach (var tp in teleporters)
                        {
                            tp.SetActive(true);
                        }
                        ResetStartPositions();

                        tutorialStep++;
                    }
                    break;
                }
        }
    }
    private void RefreshMessages(int tutStep)
    {
        for (int i = 0; i < messages.Length; i++)
        {
            if (i != tutStep) messages[i].SetActive(false);
            else messages[i].SetActive(true);
        }
    }
    private void ResetStartPositions()
    {
        foreach (StartPosition sp in startPositions)
        {
            if (sp.StartPos != Vector3.zero) sp.ResetPosRotScale();
        }
    }
    private void DisableAll()
    {
        TP_SHOWCASE.SetActive(false);
        crystal.SetActive(false);
        portal.SetActive(false);
        forcefield.SetActive(false);
        spaceship.SetActive(false);
        ui_CamToggle.SetActive(false);
        ui_StartMoving.SetActive(false);
        ui_TimeControl.SetActive(false);
        
        foreach (var tp in teleporters)
        {
            tp.SetActive(false);
        }

        foreach (var gun in machineGuns)
        {
            gun.SetActive(false);
        }
    }
    public void SetTimeToNormal()
    {
        Time.timeScale = IngameController.AskFor.CurrentTimeScale;
    }
}