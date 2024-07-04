using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MusicIngame : MonoBehaviour
{
    public AudioClip[] ingameSongs;
    public AudioSource audioSource;
    public TMP_Text nowPlayingSongTitle;

    public static float currentTimeStamp { get; private set; }
    public static int currentSongIndex { get; private set; }

    private Coroutine coroutine;

    private void Awake()
    {
        audioSource.loop = true;
        //currentSongIndex = Random.Range(0, ingameSongs.Length - 1);
        audioSource.clip = ingameSongs[currentSongIndex];
        audioSource.time = currentTimeStamp;

        coroutine = StartCoroutine(TrackCurrentSong());

        if (!audioSource.isPlaying) audioSource.Play();

        RefreshNowPlaying();
    }

    private void OnDestroy()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
    }

    private IEnumerator TrackCurrentSong()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(2f);

            currentTimeStamp = audioSource.time;
        }
    }

    public void SkipCurrentSong()
    {
        print("audio source time" + audioSource.time);
        currentSongIndex++;
        if (currentSongIndex >= ingameSongs.Length) currentSongIndex = 0;

        if (audioSource.isPlaying) audioSource.Stop();
        audioSource.clip = ingameSongs[currentSongIndex];
        audioSource.Play();

        RefreshNowPlaying();
    }
    
    public void RewindCurrentSong()
    {
        currentSongIndex--;
        if (currentSongIndex < 0) currentSongIndex = ingameSongs.Length - 1;

        if (audioSource.isPlaying) audioSource.Stop();
        audioSource.clip = ingameSongs[currentSongIndex];
        audioSource.Play();

        RefreshNowPlaying();
    }

    private void RefreshNowPlaying()
    {
        nowPlayingSongTitle.text = CurrentSongName();
        //print("current song is nr " + currentSongIndex + " with name: " + CurrentSongName());
    }

    public string CurrentSongName()
    {
        var name = audioSource.clip.name;
        name = name.Replace("(", "");
        name = name.Replace(")", "");
        name = name.Replace("Long", "");
        name = name.Replace("Loop", "");

        return name;
    }
}
