using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testmove : MonoBehaviour
{
    public float speed;


    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, 0, 1f) * speed * Time.deltaTime;
    }
}
