///
///	Made by a1-creator
///	All rights reserved to a1-creator
///

using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private Transform objectToTrack;
    [SerializeField] private bool objectToTrackIsActiveCamera;

    private void Update()
    {
        if (!objectToTrackIsActiveCamera)
        {
            transform.LookAt(objectToTrack.transform.position);
            transform.Rotate(Vector3.up, 180f);
        }
        else
        {
            transform.LookAt(Camera.allCameras[0].transform.position);
            transform.Rotate(Vector3.up, 180f);
        }
    }
}
