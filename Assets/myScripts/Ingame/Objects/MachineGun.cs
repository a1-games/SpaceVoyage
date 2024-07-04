using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : MonoBehaviour, IRespawnable
{

    [SerializeField] private Collider[] colliders;
    [SerializeField] private GameObject aliveTurret;
    [SerializeField] private GameObject deadTurret_Simple;
    [SerializeField] private GameObject deadTurret_Rigified;
    [SerializeField] private GameObject deathAnimation;
    [SerializeField] private AudioSource deathSound;
    [SerializeField] private Rigification rigifyScript;
    [SerializeField] private GameObject laserObject;
    [field: SerializeField] public List<Teleporter> activeTPLasers { get; set; }

    private void Start()
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = true;
        }
        deadTurret_Simple.SetActive(false);
        deadTurret_Rigified.SetActive(false);
        aliveTurret.SetActive(true);
        deathAnimation.SetActive(false);
        laserObject.SetActive(true);

        activeTPLasers = new List<Teleporter>();
    }
    public void Death()
    {
        // turn off all linked lasers
        if (activeTPLasers.Count > 0)
        {
            for (int i = 0; i < activeTPLasers.Count; i++)
            {
                activeTPLasers[i].linkedTPLaserScript.Death();
            }
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }

        deathAnimation.SetActive(true);
        deathSound.Play();
        aliveTurret.SetActive(false);
        laserObject.SetActive(false);

        // if quality level is above the median within the available settings, use the advanced explosion
        var useRigified = QualitySettings.GetQualityLevel() > 1 ? true : false; 
        if (!useRigified)
        {
            deadTurret_Simple.SetActive(true);
        }
        else
        {
            deadTurret_Rigified.SetActive(true);
            rigifyScript.Rigify();
        }
    }

    public void Respawn()
    {
        Start();
        rigifyScript.Respawn();

        var beam = laserObject.GetComponent<LaserBeam>();
        beam.Respawn();
        beam.clashAnimation.gameObject.SetActive(true);
    }
}
