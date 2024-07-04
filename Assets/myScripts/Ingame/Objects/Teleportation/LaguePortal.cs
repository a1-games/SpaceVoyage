using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaguePortal : MonoBehaviour {
    [Header ("Main Settings")]
    public LaguePortal linkedPortal;
    public int recursionLimit = 5;
    public float screenThickness = 0.1f;

    [Header ("Advanced Settings")]
    public float nearClipOffset = 0.05f;
    public float nearClipLimit = 0.2f;

    // Private variables
    private Vector3 thisTransPos;
    private List<PortalTraveller> trackedTravellers;

    void Awake () {
        thisTransPos = transform.position;
        trackedTravellers = new List<PortalTraveller> ();
    }

    void LateUpdate () {
        HandleTravellers ();
    }

    public void RemoveTraveller(PortalTraveller traveller)
    {
        if (trackedTravellers.Contains(traveller))
        {
            traveller.ExitPortalThreshold();
            trackedTravellers.Remove(traveller);
        }
    }

    private Vector3 offsetFromPortal;
    private int portalSide = 0;
    private int portalSideOld = 0;
    private Transform travellerT;
    void HandleTravellers () {

        for (int i = 0; i < trackedTravellers.Count; i++) {

            PortalTraveller traveller = trackedTravellers[i];
            if (!traveller) continue;


            travellerT = traveller.transform;
            var m = linkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * travellerT.localToWorldMatrix;

            offsetFromPortal = travellerT.position - transform.position;
            portalSide = System.Math.Sign (Vector3.Dot (offsetFromPortal, transform.forward));
            portalSideOld = System.Math.Sign (Vector3.Dot (traveller.previousOffsetFromPortal, transform.forward));


            //UpdateSliceParams(traveller);

            // Teleport the traveller if it has crossed from one side of the portal to the other
            if (portalSide != portalSideOld) 
            {
                //a1creator project specfic use
                var ph = traveller.gameObject.GetComponent<PlayerHandler>();
                if (ph)
                {
                    ph.ParticleToggle(0);
                    ParticleToggleOnRoutine(ph, 0.12f); // turn off particles so that emission by distance doesnt go crazy when teleported
                }

                //sebastian lague
                var positionOld = travellerT.position;
                // if you dont make sure the y is correct, it will be incorrect for a single frame if running high frame rate.
                // thats why we change y here:
                positionOld.y = traveller.graphicsClone.transform.position.y;
                var rotOld = Quaternion.Euler(travellerT.rotation.eulerAngles + new Vector3(0f, 180f, 0f));

                traveller.Teleport (transform, linkedPortal.transform, m.GetColumn (3), m.rotation);
                traveller.graphicsClone.transform.SetPositionAndRotation (positionOld, rotOld);

                //print("slice numbers are wrong in this single frame");
                //UpdateSliceParams(traveller);

                // Can't rely on OnTriggerEnter/Exit to be called next frame since it depends on when FixedUpdate runs
                linkedPortal.OnTravellerEnterPortal(traveller);
                trackedTravellers.RemoveAt (i);
                i--; // dont skip an item bc you delete this and i increases, so stay on i after list moves down -1
                
            } 
            else 
            {
                //if rotation is wrong, change + to -
                // this is here so that if the graphicsobject has a different angle in the inspector, that angle is considered when calculating exit angle
                var rot = Quaternion.Euler(m.rotation.eulerAngles + traveller.StartRotation.eulerAngles + new Vector3(0f, 180f, 0f)); //wow, i actually improved sebastian lagues code! -a1creator
                var newPosition = new Vector3(m.GetColumn(3).x, traveller.graphicsObject.transform.position.y, m.GetColumn(3).z); // wow, fixed two things! -a1creator
                traveller.graphicsClone.transform.SetPositionAndRotation(newPosition, rot); 
                
                traveller.previousOffsetFromPortal = offsetFromPortal;
                UpdateSliceParams (traveller);
            }


        }
    }

    void ParticleToggleOnRoutine(PlayerHandler ph, float delay)
    {
        StartCoroutine(ToggleParticlesOnWithDelay(ph, delay));
    }
    IEnumerator ToggleParticlesOnWithDelay(PlayerHandler ph, float delay)
    {
        yield return new WaitForSeconds(delay);
        ph.ParticleToggle(1);
    }

    void HandleClipping () {
        // There are two main graphical issues when slicing travellers
        // 1. Tiny sliver of mesh drawn on backside of portal
        //    Ideally the oblique clip plane would sort this out, but even with 0 offset, tiny sliver still visible
        // 2. Tiny seam between the sliced mesh, and the rest of the model drawn onto the portal screen
        // This function tries to address these issues by modifying the slice parameters when rendering the view from the portal
        // Would be great if this could be fixed more elegantly, but this is the best I can figure out for now
        const float hideDst = -1000;
        const float showDst = 1000;
        

        foreach (var traveller in trackedTravellers) {
            if (SameSideOfPortal (traveller.transform.position, thisTransPos)) {
                // Addresses issue 1
                traveller.SetSliceOffsetDst (hideDst, false);
            } else {
                // Addresses issue 2
                traveller.SetSliceOffsetDst (showDst, false);
            }

            // Ensure clone is properly sliced, in case it's visible through this portal:
            int cloneSideOfLinkedPortal = -SideOfPortal (traveller.transform.position);
            bool camSameSideAsClone = linkedPortal.SideOfPortal (thisTransPos) == cloneSideOfLinkedPortal;
            if (camSameSideAsClone) {
                traveller.SetSliceOffsetDst (screenThickness, true);
            } else {
                traveller.SetSliceOffsetDst (-screenThickness, true);
            }
        }

        //var offsetFromPortalToCam = thisTransPos - transform.position;
        foreach (var linkedTraveller in linkedPortal.trackedTravellers) {
            var travellerPos = linkedTraveller.graphicsObject.transform.position;
            //var clonePos = linkedTraveller.graphicsClone.transform.position;
            // Handle clone of linked portal coming through this portal:
            bool cloneOnSameSideAsCam = linkedPortal.SideOfPortal (travellerPos) != SideOfPortal (thisTransPos);
            if (cloneOnSameSideAsCam) {
                // Addresses issue 1
                linkedTraveller.SetSliceOffsetDst (hideDst, true);
            } else {
                // Addresses issue 2
                linkedTraveller.SetSliceOffsetDst (showDst, true);
            }

            // Ensure traveller of linked portal is properly sliced, in case it's visible through this portal:
            bool camSameSideAsTraveller = linkedPortal.SameSideOfPortal (linkedTraveller.transform.position, thisTransPos);
            if (camSameSideAsTraveller) {
                linkedTraveller.SetSliceOffsetDst (screenThickness, false);
            } else {
                linkedTraveller.SetSliceOffsetDst (-screenThickness, false);
            }
        }
    }

    void UpdateSliceParams (PortalTraveller traveller) {
        // Calculate slice normal
        int side = SideOfPortal (traveller.transform.position);
        Vector3 sliceNormal = transform.forward * -side;
        Vector3 cloneSliceNormal = linkedPortal.transform.forward * side;

        // Calculate slice centre
        Vector3 slicePos = transform.position;
        Vector3 cloneSlicePos = linkedPortal.transform.position;

        // Adjust slice offset so that when player standing on other side of portal to the object, the slice doesn't clip through
        float sliceOffsetDst = 0;
        float cloneSliceOffsetDst = 0;

        bool playerSameSideAsTraveller = SameSideOfPortal (traveller.transform.position, traveller.transform.position);
        if (!playerSameSideAsTraveller) {
            sliceOffsetDst = -screenThickness;
        }
        bool playerSameSideAsCloneAppearing = side != linkedPortal.SideOfPortal (traveller.transform.position);
        if (!playerSameSideAsCloneAppearing) {
            cloneSliceOffsetDst = -screenThickness;
        }

        // Apply parameters
        for (int i = 0; i < traveller.originalMaterials.Length; i++) {
            traveller.originalMaterials[i].SetVector ("sliceCentre", slicePos);
            traveller.originalMaterials[i].SetVector ("sliceNormal", sliceNormal);
            traveller.originalMaterials[i].SetFloat ("sliceOffsetDst", sliceOffsetDst);

            traveller.cloneMaterials[i].SetVector ("sliceCentre", cloneSlicePos);
            traveller.cloneMaterials[i].SetVector ("sliceNormal", cloneSliceNormal);
            traveller.cloneMaterials[i].SetFloat ("sliceOffsetDst", cloneSliceOffsetDst);

        }

    }


    void OnTravellerEnterPortal (PortalTraveller traveller) {
        if (!trackedTravellers.Contains (traveller)) {
            traveller.EnterPortalThreshold ();
            traveller.previousOffsetFromPortal = traveller.transform.position - transform.position;
            trackedTravellers.Add (traveller);
            UpdateSliceParams(traveller);
        }
    }

    void OnTriggerEnter (Collider other) 
    {
        if (!IngameController.AskFor.GameHasStarted) return;
        if (IngameController.AskFor.PlayerIsDead) return;
        //if (!IngameController.AskFor.PlayerObject.GetComponent<PlayerMovement>().IsMoving) return; removed bc it would disable portals if entering whilst turning

        var traveller = other.GetComponent<PortalTraveller> ();
        if (traveller) {
            //print("found traveller " + traveller.gameObject.name);
            OnTravellerEnterPortal (traveller);
            traveller.CurrentLaguePortal = this;
        }
    }

    void OnTriggerExit (Collider other) {
        var traveller = other.GetComponent<PortalTraveller> ();
        if (traveller && trackedTravellers.Contains (traveller)) {
            traveller.CurrentLaguePortal = null;
            UpdateSliceParams (traveller);
            trackedTravellers.Remove (traveller);
            traveller.ExitPortalThreshold ();
        }
    }

    /*
     ** Some helper/convenience stuff:
     */

    int SideOfPortal (Vector3 pos) {
        return System.Math.Sign (Vector3.Dot (pos - transform.position, transform.forward));
    }

    bool SameSideOfPortal (Vector3 posA, Vector3 posB) {
        return SideOfPortal (posA) == SideOfPortal (posB);
    }


    void OnValidate () {
        if (linkedPortal != null) {
            linkedPortal.linkedPortal = this;
        }
    }
}