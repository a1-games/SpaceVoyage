using System.Collections.Generic;
using UnityEngine;

public class SliceTest : MonoBehaviour
{
    public GameObject graphic;
    Material[] materials;

    private void Start()
    {
        materials = GetMaterials(graphic);
    }

    private void Update()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetVector("sliceCentre", transform.position);
            materials[i].SetVector("sliceNormal", -transform.up);
        }
    }


    Material[] GetMaterials(GameObject g)
    {
        var renderers = g.GetComponentsInChildren<MeshRenderer>();
        var matList = new List<Material>();
        foreach (var renderer in renderers)
        {
            foreach (var mat in renderer.materials)
            {
                matList.Add(mat);
            }
        }
        return matList.ToArray();
    }
}
