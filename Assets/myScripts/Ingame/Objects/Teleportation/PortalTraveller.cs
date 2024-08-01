using System.Collections.Generic;
using UnityEngine;

public class PortalTraveller : MonoBehaviour {
    public PlayerHandler playerHandler;
    public GameObject graphicsObject;
    public GameObject graphicsClone { get; set; }
    public Vector3 previousOffsetFromPortal { get; set; }
    //public LaguePortal PreviousLaguePortal { get; set; }
    public LaguePortal CurrentLaguePortal { get; set; }

    public Material[] originalMaterials { get; set; }
    public Material[] cloneMaterials { get; set; }
    public Quaternion StartRotation { get; private set; }
    // dumb fix with the traveller tp'ing back again because I can't figure out the problem
    //public int TeleportsBeforeExit { get; set; } = 0;

    private void Start()
    {
        if (playerHandler != null)
        {
            playerHandler.thisTraveller = this;
            graphicsObject = playerHandler.ShipObject;
        }
        // else it has been set in the inspector
        StartRotation = graphicsObject.transform.localRotation;
    }

    public virtual void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) {
        transform.position = pos;
        transform.rotation = rot;
    }

    // Called when first touches portal
    public virtual void EnterPortalThreshold () {
        if (graphicsClone == null)
        {
            graphicsClone = Instantiate(graphicsObject);
            graphicsClone.transform.parent = graphicsObject.transform.parent;
            //graphicsClone.transform.rotation = graphicsObject.transform.localRotation;
            graphicsClone.transform.localScale = graphicsObject.transform.localScale;
            originalMaterials = GetMaterials (graphicsObject);
            cloneMaterials = GetMaterials (graphicsClone);
        } else {
            graphicsClone.SetActive (true);
        }
    }

    // Called once no longer touching portal (excluding when teleporting)
    public virtual void ExitPortalThreshold () {
        if (graphicsClone) graphicsClone.SetActive (false);
        // Disable slicing
        if (originalMaterials == null) return; // return if this is called in a level without portals
        for (int i = 0; i < originalMaterials.Length; i++) {
            originalMaterials[i].SetVector ("sliceNormal", Vector3.zero);
        }
    }

    public void SetSliceOffsetDst (float dst, bool clone) {
        for (int i = 0; i < originalMaterials.Length; i++) {
            if (clone) {
                cloneMaterials[i].SetFloat ("sliceOffsetDst", dst);
            } else {
                originalMaterials[i].SetFloat ("sliceOffsetDst", dst);
            }

        }
    }

    Material[] GetMaterials (GameObject g) 
    {
        var renderers = g.GetComponentsInChildren<MeshRenderer> ();
        var matList = new List<Material> ();
        foreach (var renderer in renderers) 
        {
            foreach (var mat in renderer.materials) 
            {
                matList.Add (mat);
            }
        }
        return matList.ToArray ();
    }
}