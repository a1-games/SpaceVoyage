using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceShaderChanger : MonoBehaviour
{
    public GameObject graphicsParent;
    public Shader shader_SliceHQ;
    public Shader shader_SliceHQ_Transparent;
    public bool noTransparency = false;
    public bool executeOnAwake = true;

    void Awake()
    {
        if (!executeOnAwake) return;
        ReplaceMaterials(graphicsParent);

        //destroy after performed so we dont have a useless script hanging around
        Destroy(this);
    }

    public void ReplaceAndDestroy(GameObject graphicParent)
    {
        ReplaceMaterials(graphicParent);
        Destroy(this);
    }
    private void ReplaceMaterials(GameObject g)
    {
        var renderers = g.GetComponentsInChildren<MeshRenderer>();
        var opaqueMatList = new List<Material>();
        var transparentMatList = new List<Material>();
        foreach (var renderer in renderers)
        {
            foreach (var mat in renderer.materials)
            {
                //some materials have emission color defaulted to white and that is bad so #ezfix
                if (mat.GetColor("_EmissionColor") == Color.white)
                {
                    mat.SetColor("_EmissionColor", new Color(0, 0, 0, 0));
                }
                
                var mode = mat.GetFloat("_Mode");

                if (mode == 0) // Opaque
                {
                    opaqueMatList.Add(mat);
                }
                if (mode == 3) // Transparent
                {
                    transparentMatList.Add(mat);
                }
            }
            //renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        foreach (Material mat in opaqueMatList)
        {
            mat.shader = shader_SliceHQ;
        }
        foreach (Material mat in transparentMatList)
        {
            if (!noTransparency)
                mat.shader = shader_SliceHQ_Transparent;
            else
                mat.shader = shader_SliceHQ;

        }
    }
}
