///
///	Made by a1-creator
///	All rights reserved to a1-creator
///

using UnityEngine;

public class DestroyGameObjectAfterSeconds : MonoBehaviour
{

    [SerializeField] private float lifetime;
    [SerializeField] private bool destroyThisGameObject = true;
    [SerializeField] private GameObject gameObjectToDestroy;
    private float timeAtSpawn;

    private void Start()
    {
        timeAtSpawn = Time.time;
    }
    private void Update()
    {
        if (lifetime + timeAtSpawn < Time.time)
        {
            if (destroyThisGameObject)
                Destroy(this.gameObject);
            else Destroy(gameObjectToDestroy);
        }
    }

}
