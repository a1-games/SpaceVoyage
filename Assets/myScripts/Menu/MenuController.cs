using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    private static MenuController instance;
    public static MenuController AskFor { get => instance; }

    private void Awake()
    {
        instance = this;
    }
    [field: SerializeField] public UISounds uISounds { get; private set; }
    [SerializeField] private Transform stageParent;
    [SerializeField] private Button closeTitleScreenButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitSettingsButton;

    /*private List<GameObject> stageButtons;

    private void Start()
    {
        stageButtons = new List<GameObject>();
        for (int i = 0; i < stageParent.childCount; i++)
        {
            print(i);
            var childObject = stageParent.GetChild(i).gameObject;
            if (!childObject.activeInHierarchy) continue;
            if (!childObject.activeSelf) continue;

            stageButtons.Add(childObject);
        }
    }
    */
    // BUTTON PRESSES
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UIStateHandler.CurrentUIState == UIState.TitleScreen ||
                UIStateHandler.CurrentUIState == UIState.MainMenu ||
                UIStateHandler.CurrentUIState == UIState.StageSelect)
            {
                settingsButton.onClick.Invoke();
                return;
            }
            if (UIStateHandler.CurrentUIState == UIState.Settings)
            {
                exitSettingsButton.onClick.Invoke();
                return;
            }
        }
        /*
        if (Input.GetKeyDown(KeyCode.LeftArrow) && stageButtons.Count != 0)
        {
            if (UIStateHandler.CurrentUIState == UIState.StageSelect)
            {
                stageButtons[0].GetComponent<EventTrigger>().Invoke();
            }
        }
        */
        if (UIStateHandler.CurrentUIState == UIState.TitleScreen)
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1))
                    return;
                
                closeTitleScreenButton.onClick.Invoke();
                return;
            }
        }
    }

    // QUIT FUNCTION
    public void QuitGame()
    {
        if (GameSettings.AskFor != null)
            GameSettings.AskFor.SaveAll();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}
