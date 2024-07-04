using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipSkinSpawner : MonoBehaviour
{
    private static SpaceshipSkinSpawner instance;
    public static SpaceshipSkinSpawner AskFor { get => instance; }

    [Header("Spawning")]
    [SerializeField] private bool spawnOnStart = false;

    [SerializeField] private SliceShaderChanger sliceHQChanger;
    [SerializeField] private Transform skinParent;
    [SerializeField] private GameObject afterburners;

    [SerializeField] private GameObject[] skinPrefabs;

    public GameObject shipSkinObject { get; private set; }
    public GameObject afterburnersObject { get; private set; }

    private void Awake() //other scripts refernce me in start
    {
        instance = this;
    }
    private void Start()
    {
        if (spawnOnStart) SpawnSavedSpaceshipSkin();
    }

    public void SpawnSavedSpaceshipSkin()
    {
        GameObject ship = Instantiate(skinPrefabs[GetSavedSkin()], skinParent.position, skinParent.rotation);
        ship.transform.parent = skinParent;
        if (sliceHQChanger)
            sliceHQChanger.ReplaceAndDestroy(ship);
        shipSkinObject = ship;

        GameObject burners = Instantiate(afterburners, skinParent.position, skinParent.rotation);
        burners.transform.parent = ship.transform;
        afterburnersObject = burners;

        
        var particles = transform.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].Stop();
            particles[i].Play();
        }

    }
    public void SetSavedSkin(int skinIndex)
    {
        // do some code that checks if its in the steam inventory
        GameSettings.jPlayerPrefs.SetInt("SPACESHIP_SKIN", skinIndex);
        GameSettings.jPlayerPrefs.Save();
    }
    public int GetSavedSkin()
    {
        // do some code that checks if its in the steam inventory
        return GameSettings.jPlayerPrefs.GetInt("SPACESHIP_SKIN", 0);
    }
}
