using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISounds : MonoBehaviour
{

    [SerializeField] private AudioSource clickSound;
    [SerializeField] private Vector2 clickPitch_MinMax;
    [SerializeField] private AudioSource exitSound;
    [SerializeField] private Vector2 exitPitch_MinMax;
    [SerializeField] private AudioSource hoverSound;
    [SerializeField] private Vector2 hoverPitch_MinMax;

    public void Click()
    {
        if (clickSound.isPlaying) return;
        clickSound.pitch = Random.Range(clickPitch_MinMax.x, clickPitch_MinMax.y);
        clickSound.Play();
    }
    public void Exit()
    {
        if (exitSound.isPlaying) return;
        exitSound.pitch = Random.Range(exitPitch_MinMax.x, exitPitch_MinMax.y);
        exitSound.Play();
    }
    public void Hover()
    {
        if (hoverSound.isPlaying) return;
        hoverSound.pitch = Random.Range(hoverPitch_MinMax.x, hoverPitch_MinMax.y);
        hoverSound.Play();
    }
}
