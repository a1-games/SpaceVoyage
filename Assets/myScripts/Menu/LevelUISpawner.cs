using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelUISpawner : MonoBehaviour
{
    [SerializeField] private Sprite completedLevelTile;
    [SerializeField] private Transform stageParent;
    [SerializeField] private Transform parent;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector2 firstPosition;
    [SerializeField] private Vector2[] positions;
    [SerializeField] private Toggle hideCompletedStagesToggle;
    private List<StageSelectUIItem> stageSelectUIItems = new List<StageSelectUIItem>();
    private GameObject[] generatedTiles;


    private void Start()
    {
        SetUsedUIItems();
        SpawnStageBoxes();

        if (HideToggleIfNotCompleted()) 
            hideCompletedStagesToggle.gameObject.SetActive(false);

        var hideCompletedStages = GameSettings.AskFor.GetHideCompletedStages();
        hideCompletedStagesToggle.isOn = hideCompletedStages;
        ToggleHideCompletedStages(hideCompletedStages);
    }


    private bool HideToggleIfNotCompleted()
    {
        for (int i = 0; i < stageSelectUIItems.Count; i++)
        {
            var item = stageSelectUIItems[i];
            if (item == null) continue;
            if (item.completionPercentage >= 100f)
            {
                return false;
            }
        }
        //else
        return true;
    }
    public void ToggleHideCompletedStages(bool hideCompletedStages = false)
    {
        for (int i = 0; i < stageSelectUIItems.Count; i++)
        {
            var item = stageSelectUIItems[i];
            if (item == null) continue;
            if (!hideCompletedStages)
            {
                stageSelectUIItems[i].gameObject.SetActive(true);
            }
            else
            {
                if (item.completionPercentage >= 100f)
                {
                    stageSelectUIItems[i].gameObject.SetActive(false);
                }
            }
        }
        GameSettings.AskFor.SetHideCompletedStages(hideCompletedStages);
    }

    private void SetUsedUIItems()
    {
        for (int i = 0; i < stageParent.childCount; i++)
        {
            var childObject = stageParent.GetChild(i).gameObject;

            childObject.TryGetComponent<StageSelectUIItem>(out StageSelectUIItem item);
            if (item == null) continue;

            if (childObject.activeSelf)
            {
                stageSelectUIItems.Add(item);
            }
        }
    }

    private void SpawnStageBoxes()
    {
        for (int i = 0; i < stageSelectUIItems.Count; i++)
        {
            var childObject = stageParent.GetChild(i).gameObject;

            var item = stageSelectUIItems[i];

            item.SetVariables();

            item.GetComponent<Button>().onClick.AddListener(() => MenuController.AskFor.uISounds.Click());

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => { MenuController.AskFor.uISounds.Hover(); });
            item.GetComponent<EventTrigger>().triggers.Add(entry);
        }
    }

    public void SpawnLevelTiles(string stageName)
    {
        // despawning if another level menu was loaded before
        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
        
        // get amount of tiles
        var length = SceneLoader.AskFor.LevelNames(stageName).Length;
        generatedTiles = new GameObject[length];

        // dont do anything if there are no levels
        if (length == 0) return;
        // create all tiles and set non-centered positions 
        // these positions have to be set here for the centering to work
        var posIndex = 0;
        var tmpPos = new Vector2();
        for (int i = 0; i < length; i++)
        {
            GameObject currentTile = Instantiate(prefab, parent);
            MakeUITileForLevel(currentTile, i, stageName);
            generatedTiles[i] = currentTile;

            if (i == 0)
            {
                tmpPos = currentTile.GetComponent<RectTransform>().anchoredPosition;
                tmpPos.y = firstPosition.y;
            }
            else
            {
                tmpPos.x -= positions[posIndex].x;
                tmpPos.y = positions[posIndex].y;
            }
            currentTile.GetComponent<RectTransform>().anchoredPosition = tmpPos;

            posIndex++;
            if (posIndex >= positions.Length) posIndex = 0;
        }

        // set width of content parent
        // x to y (fx 1 - 5) makes so much sense. 1 to 5 is a length of 4 which is sqrt(pow(1-5))
        var contentWidth = generatedTiles[generatedTiles.Length - 1].GetComponent<RectTransform>().anchoredPosition.x - generatedTiles[0].GetComponent<RectTransform>().anchoredPosition.x;
        // add 2 tiles in width so the width starts half a tile outside the edge tiles instead of in the pivot(center) of them
        var widthOfOneTile = generatedTiles[0].GetComponent<RectTransform>().sizeDelta.x;
        contentWidth += widthOfOneTile * 2;

        //can't modify x directly
        var rectW = parent.GetComponent<RectTransform>().sizeDelta;
        rectW.x = contentWidth;
        parent.GetComponent<RectTransform>().sizeDelta = rectW;

        // set position of all tiles
        //return;
        for (int i = 0; i < generatedTiles.Length; i++)
        {
            var tileXY = generatedTiles[i].GetComponent<RectTransform>().anchoredPosition;
            // this moves all tiles so that they are centered within the content object and therefore are centered in the scroll rect when moved around
            tileXY.x -= (contentWidth / 2f - widthOfOneTile);

            generatedTiles[i].GetComponent<RectTransform>().anchoredPosition = tileXY;

        }
        // set alignment of the content parent to fit center if under 10 and left align if over 10 levels
        var contentRect = parent.GetComponent<RectTransform>();
        if (generatedTiles.Length > 10)
        {
            contentRect.pivot = new Vector2(0f, 0.5f);
            contentRect.anchorMin = new Vector2(0f, 0f);
            contentRect.anchorMax = new Vector2(0f, 1f);
        }
        else
        {
            contentRect.pivot = new Vector2(0.5f, 0.5f);
            contentRect.anchorMin = new Vector2(0.5f, 0f);
            contentRect.anchorMax = new Vector2(0.5f, 1f);
        }
        
    }

    public void MakeUITileForLevel(GameObject tile, int index, string stageName)
    {
        var sceneNameFromStageIndex = SceneLoader.AskFor.SceneNamesFromStage(stageName)[index];
        tile.GetComponent<Button>().onClick.AddListener( () => SceneLoader.AskFor.ChangeScene(sceneNameFromStageIndex) );
        tile.GetComponent<Button>().onClick.AddListener( () => MenuController.AskFor.uISounds.Click() );
        tile.GetComponentInChildren<TMPro.TMP_Text>().text = (index+1).ToString();
        // if level has been completed
        if (GameSaves.AskFor.IsLevelComplete(sceneNameFromStageIndex))
        {
            tile.GetComponent<Image>().sprite = completedLevelTile;
        }
    }


    
}
