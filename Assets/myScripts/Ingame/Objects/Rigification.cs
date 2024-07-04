using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rigification : MonoBehaviour, IRespawnable
{
    [SerializeField] private GameObject[] objectsToRigify;
    [SerializeField] private Transform explosionCenter;
    [SerializeField] private Vector2 explosionStrengthMinMax;
    [SerializeField] private Vector2 angularRotationMinMax;

    private void Awake()
    {
        EnableRespawning();
    }

    public void Rigify()
    {
        for (int i = 0; i < objectsToRigify.Length; i++)
        {
            if (!objectsToRigify[i].TryGetComponent<Rigidbody>(out Rigidbody rb))
                rb = objectsToRigify[i].AddComponent<Rigidbody>();

            rb.isKinematic = false;
            rb.useGravity = true;

            // set random spinning rotation
            var x = Random.Range(angularRotationMinMax.x, angularRotationMinMax.y);
            var y = Random.Range(angularRotationMinMax.x, angularRotationMinMax.y);
            var z = Random.Range(angularRotationMinMax.x, angularRotationMinMax.y);

            rb.angularVelocity = new Vector3(x, y, z);

            // explosion push force
            var direction = rb.transform.position - explosionCenter.position;
            rb.AddForce(direction.normalized * Random.Range(explosionStrengthMinMax.x, explosionStrengthMinMax.y), ForceMode.Impulse);
            //rb.AddForce(Vector3.up * Random.Range(explosionStrengthMinMax.x, explosionStrengthMinMax.y));
        }
    }


    private void EnableRespawning()
    {
        for (int i = 0; i < objectsToRigify.Length; i++)
        {
            //add script to keep track of starting position
            if (!objectsToRigify[i].TryGetComponent<StartPosition>(out StartPosition sp))
                sp = objectsToRigify[i].AddComponent<StartPosition>();
        }
    }

    public void Death()
    {
        // Rigify is used instead
    }

    public void Respawn()
    {
        for (int i = 0; i < objectsToRigify.Length; i++)
        {
            objectsToRigify[i].GetComponent<StartPosition>().ResetPosRotScale();
        }
    }
}
