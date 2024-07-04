using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WobbleUpDown : MonoBehaviour
{
    public float speed = 1f;
    public float distance = 3f;

    private float upDown = 1f;

    private Vector3 savedUpPos;
    private Vector3 savedDownPos;
    private void Awake()
    {
        savedDownPos = new Vector3(transform.position.x, transform.position.y - distance / 2f, transform.position.z);
        savedUpPos = new Vector3(transform.position.x, transform.position.y + distance / 2f, transform.position.z);
    }

    private void Update()
    {
        /*
        transform.position = Vector3.Slerp(savedDownPos, savedUpPos, speed * Time.deltaTime );
        */

        transform.position += upDown * transform.up * speed * Time.deltaTime;

        if (upDown == 1f && transform.position.y > savedUpPos.y) upDown = -1f;
        if (upDown == -1f && transform.position.y < savedDownPos.y) upDown = 1f;
    }
}
