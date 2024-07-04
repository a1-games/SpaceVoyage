using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPosition : MonoBehaviour
{
    public Vector3 StartPos { get; private set; }
    public Vector3 StartScale { get; private set; }
    public Quaternion StartRot { get; private set; }

    // only happens if object is active at start
    void Start()
    {
        StartPos = transform.position;
        StartScale = transform.localScale;
        StartRot = transform.localRotation;
    }

    public void ResetPosRotScale()
    {
        transform.position = StartPos;
        transform.localScale = StartScale;
        transform.localRotation = StartRot;
    }
}
