using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveAllShadowsInChildren : MonoBehaviour
{
    private void Start()
    {
        /*
        // ALL OBJECTS IN SCENE
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
            if (go.activeInHierarchy)
                print(go + " is an active object");

        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].TryGetComponent<MeshRenderer>(out MeshRenderer rend))
            {
                //print("removed shadows from " + children[i].name);
                children[i].receiveShadows = false;
                children[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }
        */

        MeshRenderer[] children = transform.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < children.Length; i++)
        {
            //print("removed shadows from " + children[i].name);
            children[i].receiveShadows = false;
            children[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

    }
}
