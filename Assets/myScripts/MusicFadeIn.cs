using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicFadeIn : MonoBehaviour
{
    public float speed = 5f;
    private AudioSource audioSource;
    private float volOnAwake = 0;
    private float increase = 0;


    private void Start() //gamesettings messes with this in awake
    {
        audioSource = this.gameObject.GetComponent<AudioSource>();
        volOnAwake = audioSource.volume;
        increase = audioSource.volume / 100f;

        audioSource.volume = 0f;
    }

    private void Update()
    {
        audioSource.volume += increase * Time.deltaTime * speed;

        if (audioSource.volume >= volOnAwake) Destroy(this);
    }
}
