using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    private static ExplosionController instance;
    public static ExplosionController AskFor { get => instance; }
    //_-------------------
    // heating up machine gun before explode
    public GameObject machineGunHeatPrefab;
    private GameObject machineGunHeat_spawned;
    // missile
    public GameObject missileExplosionPrefab;
    private GameObject missileExplosion_spawned;
    // crystals
    public GameObject crystalExplosionPrefab;
    private List<GameObject> crystalExplosions_spawned;
    private List<ParticleSystem> crystalExplosions_spawned_ps;


    private void Awake()
    {
        instance = this;
        // machine gun heat
        machineGunHeat_spawned = Instantiate(machineGunHeatPrefab);
        machineGunHeat_spawned.SetActive(false);
        // missile
        missileExplosion_spawned = Instantiate(missileExplosionPrefab);
        missileExplosion_spawned.SetActive(false);
        // crystals
        crystalExplosions_spawned = new List<GameObject>();
        crystalExplosions_spawned_ps = new List<ParticleSystem>();
        AddCrystalExplosion();
    }

    public void MachineGunHeatEffect(Vector3 atThisPosition)
    {
        machineGunHeat_spawned.SetActive(false);
        machineGunHeat_spawned.transform.position = atThisPosition;
        machineGunHeat_spawned.SetActive(true);
    }

    public void MissileExplosion(Vector3 atThisPosition)
    {
        missileExplosion_spawned.SetActive(false);
        missileExplosion_spawned.transform.position = atThisPosition;
        missileExplosion_spawned.SetActive(true);
    }

    public void CrystalExplosion(Vector3 atThisPosition)
    {
        // close them before using them as to not accidentally turn off one we use
        CloseUnusedCrystalExplosions();
        
        // check if the first effect is ready
        int index = 0;
        while (crystalExplosions_spawned_ps[index].isPlaying)
        {
            // check next until we find an unused
            index++;
            // if all are being used, create another
            if (index >= crystalExplosions_spawned_ps.Count)
            {
                AddCrystalExplosion();
            }
        }
        crystalExplosions_spawned[index].transform.position = atThisPosition;
        crystalExplosions_spawned[index].SetActive(true);

    }
    private void AddCrystalExplosion()
    {
        // add the gameobject
        crystalExplosions_spawned.Add(Instantiate(crystalExplosionPrefab));
        var index = crystalExplosions_spawned.Count - 1;
        // add the particlesystem
        crystalExplosions_spawned_ps.Add(crystalExplosions_spawned[index].GetComponent<ParticleSystem>());
        // set invisible at spawn
        crystalExplosions_spawned[index].SetActive(false);
    }
    private void CloseUnusedCrystalExplosions()
    {
        for (int i = 0; i < crystalExplosions_spawned_ps.Count; i++)
        {
            if (!crystalExplosions_spawned_ps[i].isPlaying)
            {
                crystalExplosions_spawned[i].SetActive(false);
            }
        }
    }
}
