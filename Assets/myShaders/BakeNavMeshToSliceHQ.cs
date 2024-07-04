using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakeNavMeshToSliceHQ : MonoBehaviour
{
    public Transform bonesParent;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public Shader shader_SliceHQ;

    private void Start()
    {
        Mesh newMesh = new Mesh();
        SkinnedMeshRenderer skin = this.gameObject.GetComponent<SkinnedMeshRenderer>();
        Material skinMat = skin.material;

        // bake the skinned mesh into normal mesh
        skinnedMeshRenderer.BakeMesh(newMesh, true); 

        // destroy old stuff
        Destroy(skin);
        Destroy(bonesParent.gameObject);

        // create new stuff
        MeshFilter mf = this.gameObject.AddComponent<MeshFilter>();
        mf.mesh = newMesh;
        MeshRenderer r = this.gameObject.AddComponent<MeshRenderer>();
        r.receiveShadows = false;
        r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        r.material = skinMat;
        r.material.shader = shader_SliceHQ;
        r.enabled = true;

        //destroy after performed so we dont have a useless script hanging around
        Destroy(this);
    }
}
