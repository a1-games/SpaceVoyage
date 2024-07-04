using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAroundSelf : MonoBehaviour
{
    public float rotationSpeed = 1f;

    private void Update()
    {
        transform.Rotate(transform.up, rotationSpeed * Time.deltaTime);
    }
}
