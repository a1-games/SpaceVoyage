using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TutorialManager : MonoBehaviour
{
    private static TutorialManager instance;
    public static TutorialManager AskFor { get => instance; }
    //------------------

    public int tutorialStep = 0;
    private PlayerMovement playerMove;
    private bool playerHasRotatedOnce;
    private bool greenButtonHasMoved;
    private bool DemoThingHasRotated;
    public bool cameraButtonHasBeenClicked;

    public AudioSource tutStepSound;

    public MouseControl mouseControl;

    public TMP_Text tutorialMessage;

    public StartPosition[] startPositions;

    public GameObject[] messages;
    public GameObject greenButton;
    public GameObject greenButtonGhost;
    public GameObject rotationDemo;
    public GameObject crystal;
    public GameObject portal;
    public GameObject forcefield;
    public GameObject spaceship;
    public GameObject ui_CamToggle;
    public GameObject ui_StartMoving;
    public GameObject ui_TimeControl;


    private void Awake()
    {
        tutorialStep = 0;
        instance = this;
        playerMove = spaceship.GetComponent<PlayerMovement>();
    }
    private void Start()
    {
        TutorialCycle(true);
    }
    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && mouseControl.lastMoved_ID == greenButton.transform.GetInstanceID() && tutorialStep == 2 && !greenButtonHasMoved)
        {
            greenButtonHasMoved = true;
            WaitSecondsThenCycle(.8f);
        }
        if (mouseControl.lastRotated_ID == rotationDemo.transform.GetInstanceID() && tutorialStep == 3 && !DemoThingHasRotated)
        {
            DemoThingHasRotated = true;
            WaitSecondsThenCycle(.8f);
        }
        if (!playerHasRotatedOnce && playerMove.IsRotating && tutorialStep == 8)
        {
            playerHasRotatedOnce = true;
            WaitSecondsThenCycle(.4f);
        }

        if (IngameController.AskFor.GatheredCrystals > 0 && tutorialStep == 9 ||
            IngameController.AskFor.GatheredCrystals > 0 && tutorialStep == 14)
            TutorialCycle(true);
    }
    private void WaitSecondsThenCycle(float seconds)
    {
        StartCoroutine(ExecuteAfterSeconds(seconds, () => TutorialCycle()));
    }
    private IEnumerator ExecuteAfterSeconds(float seconds, Action command)
    {
        yield return new WaitForSeconds(seconds);
        command();
    }
    public void TutorialCycleFromPauseButton()
    {
        if (tutorialStep != 8) return;
        TutorialCycle(true);
    }
    public void TutorialCycle(bool muteSound = false)
    {
        if (!muteSound) tutStepSound.Play();
        switch (tutorialStep)
        {
            case 0: // introduction, welcoming
                {
                    if (true)
                    {
                        RefreshMessages(tutorialStep);

                        DisableAll();
                        tutorialStep++;
                    }
                    break;
                }
            case 1: // move green button
                {
                    if (true)
                    {
                        RefreshMessages(tutorialStep);

                        greenButton.SetActive(true);
                        forcefield.SetActive(true);

                        tutorialStep++;
                    }
                    break;
                }
            case 2: // rotate demo movable
                {
                    if (greenButtonHasMoved)
                    {
                        RefreshMessages(tutorialStep);

                        forcefield.SetActive(false);
                        greenButton.SetActive(false);
                        rotationDemo.SetActive(true);

                        ResetStartPositions();
                        tutorialStep++;
                    }
                    break;
                }
            case 3: // more info, no action
                {
                    if (DemoThingHasRotated)
                    {
                        RefreshMessages(tutorialStep);

                        ResetStartPositions();

                        greenButton.transform.localPosition = new Vector3(-2.25f, 0f, -0.75f);
                        greenButton.SetActive(true);
                        rotationDemo.SetActive(true);
                        spaceship.SetActive(true);
                        
                        tutorialStep++;
                    }
                    break;
                }
            case 4: // collect crystal introduction
                {
                    if (true)
                    {
                        RefreshMessages(tutorialStep);

                        DisableAll();
                        crystal.SetActive(true);
                        forcefield.SetActive(true);
                        tutorialStep++;
                    }
                    break;
                }
            case 5: // collect crystal button move
                {
                    if (true)
                    {
                        RefreshMessages(tutorialStep);
                        ResetStartPositions();

                        greenButton.SetActive(true);
                        greenButtonGhost.SetActive(true);
                        spaceship.transform.localPosition = new Vector3(-0.75f, 0f, -0.75f);
                        spaceship.SetActive(true);
                        
                        ResetStartPositions();
                        tutorialStep++;
                    }
                    break;
                }
            case 6: // collect crystal start button
                {
                    if (greenButtonGhost == null)
                    {
                        RefreshMessages(tutorialStep);

                        IngameController.AskFor.CanMoveMovables = false;
                        ui_StartMoving.SetActive(true);

                        tutorialStep++;
                    }
                    break;
                }
            case 7: // start move button
                {
                    if (IngameController.AskFor.GameCanStart)
                    {
                        RefreshMessages(tutorialStep);

                        ui_StartMoving.SetActive(false);

                        tutorialStep++;
                    }
                    break;
                }
            case 8: // on pause
                {
                    if (playerHasRotatedOnce)
                    {
                        RefreshMessages(tutorialStep);

                        Time.timeScale = 0f;

                        tutorialStep++;
                    }
                    break;
                }
            case 9: // on crystal collection
                {
                    if (IngameController.AskFor.GatheredCrystals > 0)
                    {
                        RefreshMessages(tutorialStep);

                        ui_CamToggle.SetActive(true);
                        Time.timeScale = 0f;

                        tutorialStep++;
                    }
                    break;
                }
            case 10: // on crash
                {
                    if (IngameController.AskFor.GatheredCrystals >= 1 && IngameController.AskFor.PlayerIsDead)
                    {
                        RefreshMessages(tutorialStep);

                        ui_CamToggle.SetActive(false);

                        tutorialStep++;
                    }
                    break;
                }
            case 11: // info about end portal
                {
                    if (true)
                    {
                        RefreshMessages(tutorialStep);

                        IngameController.AskFor.ChangeCamera(0);
                        //IngameController.AskFor.ChangeCamera("MainCamera");
                        DisableAll();
                        portal.SetActive(true);

                        tutorialStep++;
                    }
                    break;
                }
            case 12: // final trip into end portal
                {
                    if (true)
                    {
                        RefreshMessages(tutorialStep);

                        IngameController.AskFor.ReplayLevel();
                        //ui_TimeControl.GetComponent<Slider>().value = 1f;
                        ui_StartMoving.SetActive(true);
                        forcefield.SetActive(true);
                        greenButton.SetActive(true);
                        spaceship.SetActive(true);
                        spaceship.transform.localPosition = new Vector3(-0.75f, 0f, -0.75f);
                        crystal.SetActive(true);

                        IngameController.AskFor.CanMoveMovables = false;

                        tutorialStep++;
                    }
                    break;
                }
            case 13: // right before time slider shows
                {
                    if (true)
                    {
                        RefreshMessages(tutorialStep);

                        ui_TimeControl.SetActive(false);
                        ui_CamToggle.SetActive(false);
                        ui_StartMoving.SetActive(false);

                        tutorialStep++;
                    }
                    break;
                }
            case 14: // show time slider
                {
                    if (IngameController.AskFor.GatheredCrystals > 0)
                    {
                        RefreshMessages(tutorialStep);

                        ui_TimeControl.SetActive(true);
                        ui_CamToggle.SetActive(false);
                        ui_StartMoving.SetActive(false);
                        Time.timeScale = 0f;

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
        greenButton.SetActive(false);
        if (greenButtonGhost) greenButtonGhost.SetActive(false);
        rotationDemo.SetActive(false);
        crystal.SetActive(false);
        portal.SetActive(false);
        forcefield.SetActive(false);
        spaceship.SetActive(false);
        ui_CamToggle.SetActive(false);
        ui_StartMoving.SetActive(false);
        ui_TimeControl.SetActive(false);
    }
    public void SetTimeToNormal()
    {
        Time.timeScale = IngameController.AskFor.CurrentTimeScale;
    }
}
