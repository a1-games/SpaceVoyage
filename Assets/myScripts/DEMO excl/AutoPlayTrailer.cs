using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AutoPlayTrailer : MonoBehaviour
{
    [SerializeField] private float timeBeforeAutoPlay = 120f;
    [SerializeField] private VideoPlayer trailerPlayer;
    [SerializeField] private GameObject pressAnyKeyTextObject;
    [SerializeField] private GameObject screen;
    [SerializeField] private GameObject startOverQuestion;
    private Coroutine routine;

    public float timer;

    private void OnDisable()
    {
        if (routine != null)
            StopCoroutine(routine);
    }

    private void FixedUpdate()
    {
        if (Input.anyKey)
        {
            // reset the timer if we do anything
            timer = 0f;

            // if we are in autoplay mode
            if (screen.activeSelf)
            {
                pressAnyKeyTextObject.SetActive(false);
                startOverQuestion.SetActive(true);
            }
        }

        timer += Time.fixedDeltaTime;
        if (timer >= timeBeforeAutoPlay)
        {
            StartTrailer();

            // reset timer
            timer = 0f;
        }
    }

    private void Awake()
    {
        DisableTrailer();
    }

    private void StartTrailer()
    {
        if (!trailerPlayer.isPlaying)
            trailerPlayer.Play();
        
        screen.SetActive(true);
        startOverQuestion.SetActive(false);
        pressAnyKeyTextObject.SetActive(true);
    }

    public void DisableTrailer()
    {
        if (trailerPlayer.isPlaying)
            trailerPlayer.Stop();

        if (routine != null)
            StopCoroutine(routine);

        screen.SetActive(false);
        startOverQuestion.SetActive(false);
        pressAnyKeyTextObject.SetActive(false);
    }

    public void StartOver()
    {
        DisableTrailer();
        // this reloads the scene which turns off the loading screen
        //GameSettings.AskFor.DeleteAllGameSaves();
        SceneLoader.AskFor.ChangeScene("Tutorial");
    }
}
