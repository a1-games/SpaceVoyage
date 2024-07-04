using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreLoadPrefabs : MonoBehaviour
{
    [SerializeField] private string sceneName = "PreLoad";

    private Scene activeScene;

    void Start()
    {
        // get active scene
        activeScene = SceneManager.GetActiveScene();

        // start checking if its loaded so we can preload the rest
        StartCoroutine(WaitUntilReady());
    }

    // load the heavy stuff async while in the menu so level loading takes less
    private IEnumerator WaitUntilReady()
    {
        bool run = true;

        while (run)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            if (activeScene.isLoaded)
            {
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                run = false;
            }

        }
    }
}
