using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class IngameController : MonoBehaviour
{
    private static IngameController instance;
    public static IngameController AskFor { get => instance; }

    private void Awake()
    {
        instance = this;
        GameHasStarted = false;
        GameHasEnded = false;
        PlayerIsDead = false;
        PortalIsOpen = false;
        CanRotatePlayCam = false;
        GatheredCrystals = 0;
        CurrentTimeScale = Time.timeScale;
        CanMoveMovables = true;
        CurrentCamTag = "MainCamera";
        currentCam = cameras[0];

        PlayerCam = PlayerObject.GetComponentInChildren<PlayerCam>();


        crystals = new List<CrystalPickup>();
        teleporters = new List<Teleporter>();
        respawnables = new List<IRespawnable>();
        respawnables.Add(PlayerObject.GetComponent<PlayerHandler>());
        respawnables.Add(Portal); // ADD the portal so it turns off on replay if it was on
        
        for (int i = 0; i < movablesParent.transform.childCount; i++)
        {
            var child = movablesParent.transform.GetChild(i);
            child.TryGetComponent<IRespawnable>(out IRespawnable respawnable);
            if (respawnable != null) respawnables.Add(respawnable);

            if (child.TryGetComponent<Teleporter>(out Teleporter tp))
            {
                teleporters.Add(tp);
            }
        }
        for (int i = 0; i < prePlacedObjectsParent.transform.childCount; i++)
        {
            var child = prePlacedObjectsParent.transform.GetChild(i);
            child.TryGetComponent<IRespawnable>(out IRespawnable respawnable);
            if (respawnable != null) respawnables.Add(respawnable);

            if (child.TryGetComponent<CrystalPickup>(out CrystalPickup crystal))
            {
                crystals.Add(crystal);
            }
        }

        cameraAnimObject.SetActive(false);
        securityCamOverlay.SetActive(true); // true because securityCam is the starting camera

    }

    private bool isTutorial = false;
    ///-----------------------------
    [SerializeField] private GameObject prePlacedObjectsParent;
    [SerializeField] private GameObject movablesParent;
    private List<IRespawnable> respawnables;
    private List<CrystalPickup> crystals;
    private List<Teleporter> teleporters;

    [field: SerializeField] 
    public GameObject PlayerObject { get; private set; }

    [SerializeField] private Portal portal;
    public Portal Portal { get => portal; }

    [Header("UI")]
    public UIState currentUIState;
    [Header("Are You Lost?")]
    public GameObject wrongdirectionWarning;

    [Header("Camera Switching")]
    public AudioSource securityCamAudio;
    private int currentCamNr = 0;
    private bool isSwitching = false;
    public float switchTime = 2f;
    public GameObject cameraAnimObject;
    public GameObject securityCamOverlay;
    public Camera[] cameras;
    public Sprite[] cameraIcons;
    private Camera currentCam;
    public Image cameraIcon;

    public PlayerCam PlayerCam { get; private set; }
    public bool GameCanStart { get; set; } = true;
    public bool GameHasStarted { get; set; }
    public bool GameHasEnded { get; private set; }
    public bool GameIsPaused { get; private set; }
    public bool PlayerIsDead { get; set; }
    public float CurrentTimeScale { get; set; }
    public bool CanRotatePlayCam { get; set; }
    public bool CanMoveMovables { get; set; }
    public bool PortalIsOpen { get; set; }
    public int CrystalCount { get => crystals.Count; }
    public int GatheredCrystals { get; set; }
    private string CurrentCamTag { get; set; }

    public UnityEvent onPlayerPressedGo;
    public UnityEvent onRoundEnded;
    public UnityEvent onPlayerDied;

    private void Start()
    {
        UIStateHandler.AskFor.RefreshUI(currentUIState.ToString());
        if (SceneLoader.AskFor.isDemo)
            isTutorial = true;

        ChangeCamera(0);
    }

    private void Update()
    {
        KeyboardCheck();
    }

    // RESTART LEVEL BUT KEEP MOVABLE POSITIONS
    public void ReplayLevel()
    {
        Awake();

        for (int i = 0; i < respawnables.Count; i++)
        {
            //player respawn is handled in respawnables aswell
            //print(respawnables[i]);
            respawnables[i].Respawn();
        }

        ChangeCamera(0);
        //ChangeCamera("MainCamera");
    }

    // OTHER CONTROLS
    public void KeyboardCheck()
    {
        if (Input.GetKeyDown(KeyCode.Pause) ||
           Input.GetKeyDown(KeyCode.P) ||
           Input.GetKeyDown(KeyCode.Escape))
        {
            if (UIStateHandler.CurrentUIState == UIState.InGame ||
                UIStateHandler.CurrentUIState == UIState.Paused ||
                UIStateHandler.CurrentUIState == UIState.Settings )
            {
                TogglePause();
            }
        }

        if (isTutorial) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (UIStateHandler.CurrentUIState != UIState.InGame) return;
            OnPlayerBeginMoving();
        }
        
        if (Input.GetKeyDown(KeyCode.KeypadEnter) ||
            Input.GetKeyDown(KeyCode.Return))
        {
            if (UIStateHandler.CurrentUIState != UIState.InGame) return;
            ToggleBetweenCameras();
        }
    }

    // PAUSE MENU
    public void TogglePause()
    {
        if (UIStateHandler.CurrentUIState == UIState.Paused || UIStateHandler.CurrentUIState == UIState.InGame) // only works ingame or pause mode
        switch (GameIsPaused)
        {
            case true:
                {
                    UIStateHandler.AskFor.RefreshUI(UIState.InGame);
                    Time.timeScale = CurrentTimeScale;
                    GameIsPaused = false;
                    break;
                }
            case false:
                {
                    UIStateHandler.AskFor.RefreshUI(UIState.Paused);
                    Time.timeScale = 0f;
                    GameIsPaused = true;
                    break;
                }
        }
    }

    // GAME STATE
    public void GameIsOver()
    {
        if (GameHasEnded) return;

        onRoundEnded.Invoke();

        GameHasEnded = true;

        CanRotatePlayCam = false;
    }


    // --- PLAYER PRESSES START MOVING
    public void OnPlayerBeginMoving()
    {
        if (!GameCanStart) return;
        if (GameHasStarted) return;
        GameHasStarted = true;

        // call event subscribtions
        onPlayerPressedGo.Invoke();

        foreach (Teleporter tp in teleporters)
        {
            tp.ActivateTeleporter();
        }

        PlayerHandler ph = PlayerObject.GetComponent<PlayerHandler>();
        ph.ToggleShipSounds();
        ph.ParticleToggle(1);

        PlayerObject.GetComponent<PlayerMovement>().ToggleMove();
        if (isTutorial) return;
        if (GameSettings.AskFor.GetCameraAutoSwitchOnStart() && currentCamNr == 0) ToggleBetweenCameras();
    }

    // --- CAMERA
    public void UIButton_ToggleBetweenCameras()
    {
        if (!GameCanStart) return;
        ToggleBetweenCameras();
    }
    public void ToggleBetweenCameras()
    {
        Camera newCam = new Camera(); //new empty camera to be replaced with the found one in the for loop

        if (currentCamNr >= cameras.Length - 1 )
        {
            ChangeCamera(0);
            newCam = cameras[0];
        }
        else
        {
            for (int i = 0; i < cameras.Length; i++)
            {
                if (currentCamNr + 1 == i)
                {
                    ChangeCamera(i);
                    newCam = cameras[i];
                    break;
                }
            }
        }

        // control if player can rotate playercamera
        if (currentCamNr == 1) 
            CanRotatePlayCam = true;
        else
            CanRotatePlayCam = false;

        // security camera audio control
        if (currentCamNr == 0)
        {
            CanMoveMovables = true;
            if (securityCamAudio.isPlaying) securityCamAudio.Stop();
        }
        else
        {
            CanMoveMovables = false;
            if (!securityCamAudio.isPlaying) securityCamAudio.Play();
        }

        currentCam = newCam;
    }

    public void ChangeCamera(string cameraTag)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i].gameObject.tag == cameraTag)
            {
                ChangeCamera(i);
            }
        }
    }
    public void ChangeCamera(int camNr)
    {
        if (isSwitching) return;

        cameraIcon.sprite = cameraIcons[camNr];
        StartCoroutine(SwitchCameraAnimation(camNr));
    }

    private IEnumerator SwitchCameraAnimation(int cameraNr)
    {
        isSwitching = true;
        if (currentCamNr != cameraNr) cameraAnimObject.SetActive(true);

        currentCamNr = cameraNr;
        CurrentCamTag = cameras[cameraNr].gameObject.tag;


        for (int i = 0; i < cameras.Length; i++)
        {
            if (i == cameraNr) cameras[i].enabled = true;
            else cameras[i].enabled = false;

        }

        yield return new WaitForSeconds(switchTime);
        //fade audio and animation out

        if (cameraNr != 0)
            securityCamOverlay.SetActive(false);
        else
            securityCamOverlay.SetActive(true);


        // control if player can rotate playercamera
        if (currentCamNr == 1)
            CanRotatePlayCam = true;
        else
            CanRotatePlayCam = false;

        cameraAnimObject.SetActive(false);
        isSwitching = false;
    }

    public void InGameToLevelSelect()
    {
        GameSettings.AskFor.SaveAll();
        GameSaves.AskFor.SaveAll();
        UIStateHandler.AskFor.RefreshUI(UIState.StageSelect);
        UIStateHandler.CurrentUIState = UIState.StageSelect;
        SceneLoader.AskFor.ChangeScene("MainMenuScene");
    }

    public void InGameToMainMenu()
    {
        GameSettings.AskFor.SaveAll();
        GameSaves.AskFor.SaveAll();
        UIStateHandler.AskFor.RefreshUI(UIState.MainMenu);
        UIStateHandler.CurrentUIState = UIState.MainMenu;
        SceneLoader.AskFor.ChangeScene("MainMenuScene");
    }
}
