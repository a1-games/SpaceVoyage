using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AnimateLineRenderer : MonoBehaviour
{
    [SerializeField] private float fps = 12f;
    [SerializeField] private Texture[] textures;

    private int animationStep;
    private Material mat;
    private float fpsCounter;


    private void Awake()
    {
        mat = this.gameObject.GetComponent<LineRenderer>().material;
    }

    private void Update()
    {
        fpsCounter += Time.deltaTime;

        if (fpsCounter >= 1f / fps)
        {
            animationStep++;
            if (animationStep >= textures.Length) animationStep = 0;

            mat.SetTexture("_MainTex", textures[animationStep]);
            mat.SetTexture("_EmissionMap", textures[animationStep]);

            fpsCounter = 0f;
        }
    }
}
