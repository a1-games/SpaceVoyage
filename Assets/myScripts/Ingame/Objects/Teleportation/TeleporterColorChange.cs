using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterColorChange : MonoBehaviour
{
    [field: SerializeField] public bool RandomizeColorOnStart { get; set; } = false;
    public Color color;
    [Header("Shader Name")]
    public MeshRenderer[] legacyVertexlitBlended;
    public MeshRenderer[] spritesDefault;
    public MeshRenderer[] collisionBorders;
    public MeshRenderer[] defaultShader;
    public MeshRenderer[] bentPipes;
    public ParticleSystem[] particleSystems;


    private void Start()
    {
        if (RandomizeColorOnStart)
        {
            float[] rgb = new float[3];
            var maxColIndex = Random.Range(0, rgb.Length);
            var rndIndex = maxColIndex;
            while (rndIndex == maxColIndex)
            {
                rndIndex = Random.Range(0, rgb.Length);
            }
            rgb[maxColIndex] = 1f;
            rgb[rndIndex] = (byte)Random.Range(0f, 1f);

            //create new color but maintain the set alpha from the inspector
            color = new Color(rgb[0], rgb[1], rgb[2], color.a);
            //print(color);
        }

        for (int i = 0; i < legacyVertexlitBlended.Length; i++)
        {
            var mat = legacyVertexlitBlended[i].material;
            mat.SetColor("_EmisColor", color);
        }
        for (int i = 0; i < spritesDefault.Length; i++)
        {
            var mat = spritesDefault[i].material;
            mat.SetColor("_Color", color);
        }
        for (int i = 0; i < collisionBorders.Length; i++)
        {
            var mat = collisionBorders[i].materials;
            mat[1].SetColor("_Color", color);
        }
        for (int i = 0; i < defaultShader.Length; i++)
        {
            var mat = defaultShader[i].material;
            mat.SetColor("_Color", color);
            mat.SetColor("_EmissionColor", color);
        }
        for (int i = 0; i < bentPipes.Length; i++)
        {
            var mat = bentPipes[i].materials;
            mat[1].SetColor("_Color", color);
        }
        for (int i = 0; i < particleSystems.Length; i++)
        {
            var col = color;
            Color startCol = particleSystems[i].main.startColor.color;
            col.a = startCol.a;
            particleSystems[i].startColor = col;
        }
    }

}
