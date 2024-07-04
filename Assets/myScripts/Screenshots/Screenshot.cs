using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Screenshot : MonoBehaviour
{
    [SerializeField] public bool useOldInputSystem = true;
    [SerializeField] public bool useCustomResolution = false;
    [SerializeField] public KeyCode screenshotKeybind = KeyCode.T;
    [SerializeField] public string defaultScreenshotName = "Screenshot";
    [SerializeField] public int resX = 0;
    [SerializeField] public int resY = 0;
    [SerializeField] public UnityEvent<Texture2D> onScreenshotTaken;
    // I advise against changing this but go ahead if you know what you're doing
    private string screenshotsFoldername = "Screenshots";

    // had to put this here bc unity is confusing but it is only related to the custom editor scipt, don't touch
    [SerializeField] public bool hideDeveloperTools = false;

    private int highestUnusedScreenshotNr = 0;


    private void Update()
    {
        if (!useOldInputSystem) return;
        if (Input.GetKeyDown(screenshotKeybind))
        {
            TakeScreenshot();
        }
    }

    public void RebindScreenshotKey(KeyCode key)
    {
        screenshotKeybind = key;
    }

    public List<Texture2D> GetAllScreenshots()
    {
        // get all files in the folder as Object type
        var filesInScreenShotsFolder = Resources.LoadAll(screenshotsFoldername);
        List<Texture2D> screenshots = new List<Texture2D>();
        // go through all
        for (int i = 0; i < filesInScreenShotsFolder.Length; i++)
        {
            // if the file is a screenshot by name, add it to our list if it is a Texture2D
            if (filesInScreenShotsFolder[i].name.Contains(defaultScreenshotName))
            {
                // try converting
                var file = filesInScreenShotsFolder[i] as Texture2D;
                // if successful, add to our list
                if (file != null)
                    screenshots.Add(file);
            }
        }
        // return as array for better optional performance
        return screenshots;
    }
    public Texture2D[] GetAllScreenshotsAsArray()
    {
        var list = GetAllScreenshots();
        return list.ToArray();
    }

    public void ScreenShotKeyBind(InputAction.CallbackContext context)
    {
        if (context.performed)
            TakeScreenshot();
    }

    public void TakeScreenshot()
    {
        StartCoroutine(EndOfFrameRoutine());
    }

    private IEnumerator EndOfFrameRoutine()
    {
        yield return new WaitForEndOfFrame();
        ProcessScreenshotTaken();
    }
    private void ProcessScreenshotTaken()
    {
        var path = Application.dataPath + "/Resources/" + screenshotsFoldername + "/";

        var texture = ScreenCapture.CaptureScreenshotAsTexture();
        // invoke the event and send out the screenshot as Texture2D
        onScreenshotTaken.Invoke(texture);

        // if a custom resolution is set, use that
        if (useCustomResolution && resX != 0 && resY != 0)
        {
            // For older unity versions, uncomment this and delete the other:
            texture.Resize(resX, resY);
            //texture.Reinitialize(resX, resY);
        }

        // encode to bytes in png format
        var texturebytes = texture.EncodeToPNG();

        // create directory if it doesn't exist
        Directory.CreateDirectory(path);

        // if directory was found, save the image with a number corresponding to (the amount of screenshots in the folder + 1)
        File.WriteAllBytes(path + GetAvailableFileName() + ".png", texturebytes);

        // debug information if we are in unity editor
#if UNITY_EDITOR
        print("Screenshot was saved at " + path);
        // force unity to reload screenshot folder, because it otherwise doesn't read screenshots that were taken in this session
        AssetDatabase.Refresh();
        //----
#endif
    }

    private string GetAvailableFileName()
    {
        // get all file names to compare
        var filesInScreenShotsFolder = Resources.LoadAll(screenshotsFoldername);
        var fileNames = new string[filesInScreenShotsFolder.Length];
        for (int i = 0; i < fileNames.Length; i++)
        {
            fileNames[i] = filesInScreenShotsFolder[i].name;
        }
        Array.Sort(fileNames, new NumericOrderComparer());
        //print(fileNames.Length);

        // iterate through all files from first to last
        for (int i = highestUnusedScreenshotNr; i < fileNames.Length; i++)
        {
            var name = fileNames[i];
            var expectedName = defaultScreenshotName + "_" + (i+1);
            //print("expnam " + expectedName + "filnam " + name);

            // if the expectedName isn't taken already, take it.
            if (name != expectedName)
            {
                highestUnusedScreenshotNr = i + 1;
                return expectedName;
            }
        }
        // we expect not to reach the following code unless all screnshot names are taken:
        // in case we didn't find a usable expectedName, we know the highest unsaved is length + 1
        highestUnusedScreenshotNr = filesInScreenShotsFolder.Length + 1;
        return defaultScreenshotName + "_" + highestUnusedScreenshotNr;

    }

    public class NumericOrderComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            var regex = new Regex("^(d+)");

            // run the regex on both strings
            var xRegexResult = regex.Match(x);
            var yRegexResult = regex.Match(y);

            // check if they are both numbers
            if (xRegexResult.Success && yRegexResult.Success)
            {
                return int.Parse(xRegexResult.Groups[1].Value).CompareTo(int.Parse(yRegexResult.Groups[1].Value));
            }

            // otherwise return as string comparison
            return x.CompareTo(y);
        }
    }
}
