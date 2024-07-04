using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public struct ResolutionOption
{
    public ResolutionOption(TMP_Dropdown.OptionData optionData, Resolution resolution)
    {
        OptionData = optionData;
        Resolution = resolution;
    }

    public TMP_Dropdown.OptionData OptionData { get; set; }
    public Resolution Resolution { get; set; }
}

public class ResolutionSettings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resDropDown;
    [SerializeField] private TMP_Dropdown hertzDropDown;
    [SerializeField] private GameObject customResObject;
    [Header("Advanced")]
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_InputField screenWidth_advancedInput;
    [SerializeField] private TMP_InputField screenHeight_advancedInput;

    private List<ResolutionOption> options = new List<ResolutionOption>();
    private List<TMP_Dropdown.OptionData> htzOptions = new List<TMP_Dropdown.OptionData>();
    private List<int> hertzList = new List<int>();
    private int myHertz;

    private void Start()
    {
        //hide advanced settings
        DeActivateCustomRes();
        // create drop down settings
        CreateResolutionOptions();
        CreateHertzOptions();
        // get the saved resolution before refreshing current resolution selected
        SetSavedScreenRes();
        // set current resolution selected
        RefreshDropDownVisuals();

        // double check that vsync isnt affecting these settings
        QualitySettings.vSyncCount = 0;
    }
    private void CreateHertzOptions()
    {
        var resses = Screen.resolutions;
        // add available resolutions
        if (resses.Length != 0)
        {
            for (int i = resses.Length - 1; i >= 0; i--)
            {
                var htz = resses[i].refreshRate;
                if (!hertzList.Contains(htz))
                    hertzList.Add(htz);
            }
        }

        for (int i = 0; i < hertzList.Count; i++)
        {
            htzOptions.Add(new TMP_Dropdown.OptionData(hertzList[i].ToString() + "Hz"));
        }

        // Add Unlimited Option
        hertzList.Add(999);
        htzOptions.Add(new TMP_Dropdown.OptionData("Max"));

        // add to dropdown
        hertzDropDown.AddOptions(htzOptions);
    }

    private void CreateResolutionOptions()
    {
        var resses = Screen.resolutions;
        // add available resolutions
        if (resses.Length != 0)
        {
            //create skipcycle so it doesnt create for each loop
            bool skipCycle = false;
            
            for (int i = resses.Length - 1; i >= 0; i--)
            {
                // skip if the resolution width has already been added (if not, the list is like 20 elements long and thats too many when custom res is available.
                for (int j = 0; j < options.Count; j++)
                {
                    if (options[j].Resolution.width == resses[i].width)
                        skipCycle = true;
                    else
                        skipCycle = false;
                }
                if (skipCycle) continue;

                // get resolution
                var resolution = resses[i].ToString();

                var stringIndex = resolution.IndexOf('@');
                resolution = resolution.Substring(0, stringIndex); // -2 to delete the space after 
                var item = new ResolutionOption(new TMP_Dropdown.OptionData(resolution), resses[i]);
                if (!options.Contains(item))
                    options.Add(item);
            }
        }
        
        // add custom option
        var customItem = new ResolutionOption(new TMP_Dropdown.OptionData("Custom"), resses[0]); 
        options.Add(customItem);
        // add to dropdown in ui

        var resOptions = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < options.Count; i++)
        {
            resOptions.Add(options[i].OptionData);
        }
        resDropDown.AddOptions(resOptions);
    }

    public void ChooseResolutionOption(int index)
    {
        var optionText = resDropDown.options[index].text;
        if (optionText == "Custom")
        {
            ActivateCustomRes();
            
            screenWidth_advancedInput.text = Screen.currentResolution.width.ToString();
            screenHeight_advancedInput.text = Screen.currentResolution.height.ToString();
        }
        else
        {
            DeActivateCustomRes();
            screenWidth_advancedInput.text = options[index].Resolution.width.ToString();
            screenHeight_advancedInput.text = options[index].Resolution.height.ToString();
        }

    }

    public void ActivateCustomRes()
    {
        customResObject.SetActive(true);
    }
    public void DeActivateCustomRes()
    {
        GameSettings.jPlayerPrefs.Save();
        customResObject.SetActive(false);
    }

    // VISUALS FOR ADVANCED SETTINGS
    public void RefreshDropDownVisuals()
    {
        // resolution
        for (int i = 0; i < options.Count; i++)
        {
            if (options[i].Resolution.width == Screen.currentResolution.width)
            {
                resDropDown.value = i;
                break;
            }
        }
        // hertz / refreshrate
        for (int i = 0; i < hertzList.Count; i++)
        {
            //print("hertz " + i + " is " + hertzList[i]);
            if (hertzList[i] == Application.targetFrameRate)
            {
                hertzDropDown.value = i;
                break;
            }
        }
    }

    // REFRESH RATE
    public void ChooseRefreshRate(int index)
    {
        myHertz = hertzList[index];
    }
    public int GetChosenRefreshRate()
    {
        var savedHertz = GameSettings.jPlayerPrefs.GetInt("RESOLUTION_HERTZ", 0);
        var fallback = Screen.currentResolution.refreshRate;
        // return the set value if possible
        if (myHertz != 0) return myHertz;
        // else return the saved value if possible
        if (savedHertz != 0) return savedHertz;
        // if hertz has not been set before, use current hertz
        return fallback;
    }

    // SCREEN RESOLUTION
    public void ResetResolution()
    {
        var ress = Screen.resolutions;
        if (ress.Length <= 0) return;
        var res = ress[ress.Length - 1]; // the highest resolution is the highest index
        SetScreenRes(res.width, res.height, true);

        RefreshDropDownVisuals();
    }
    public void SetCustomScreenRes()
    {
        Int32.TryParse(screenHeight_advancedInput.text, out int height);
        Int32.TryParse(screenWidth_advancedInput.text, out int width);
        bool fullscreen = fullscreenToggle.isOn;
        if (height == 0) return;
        SetScreenRes(width, height, fullscreen);
    }
    public void SetScreenRes(int width, int height, bool fullscreen)
    {
        var fps = GetChosenRefreshRate();
        Screen.SetResolution(width, height, fullscreen);
        Application.targetFrameRate = fps;
        GameSettings.jPlayerPrefs.SetInt("RESOLUTION_WIDTH", width);
        GameSettings.jPlayerPrefs.SetInt("RESOLUTION_HEIGHT", height);
        GameSettings.jPlayerPrefs.SetInt("RESOLUTION_FULLSCREEN", fullscreen == true ? 1 : 0);
        GameSettings.jPlayerPrefs.SetInt("RESOLUTION_HERTZ", myHertz);
        GameSettings.jPlayerPrefs.Save();
    }
    public Resolution GetSavedScreenRes()
    {
        var _height = GameSettings.jPlayerPrefs.GetInt("RESOLUTION_HEIGHT", 1920);
        var _width = GameSettings.jPlayerPrefs.GetInt("RESOLUTION_WIDTH", 1080);
        var outRes = new Resolution();
        outRes.width = _width;
        outRes.width = _height;

        return outRes;
    }
    public void SetSavedScreenRes()
    {
        var width = GameSettings.jPlayerPrefs.GetInt("RESOLUTION_WIDTH", Screen.currentResolution.width);
        var height = GameSettings.jPlayerPrefs.GetInt("RESOLUTION_HEIGHT", Screen.currentResolution.height);
        var fullscreen = GameSettings.jPlayerPrefs.GetInt("RESOLUTION_FULLSCREEN", 1) == 0 ? false : true; //default to true
        var hertz = GameSettings.jPlayerPrefs.GetInt("RESOLUTION_HERTZ", Screen.currentResolution.refreshRate);

        fullscreenToggle.isOn = fullscreen;
        Screen.SetResolution(width, height, fullscreen, hertz);
    }

}
