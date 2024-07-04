using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalPickup : MonoBehaviour, IRespawnable
{
    public GameObject crystalObject;
    public GameObject rockObject;
    public Transform effectSpawnPoint;
    private bool isDead = false;
    private Collider crystalCollider;
    //[SerializeField] private MeshRenderer crystalRenderer;

    private void Awake()
    {
        crystalCollider = GetComponent<Collider>();
        crystalObject.SetActive(true);

        //crystalRenderer.material.color = 
        crystalObject.transform.Rotate(transform.up, Random.Range(0f, 359f));
        rockObject.transform.Rotate(transform.up, Random.Range(0f, 359f));
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Death();
        }
    }
    public void Death()
    {
        if (isDead) return;
        crystalCollider.enabled = false;
        IngameController.AskFor.GatheredCrystals++;
        IngameController.AskFor.Portal.PortalCheck();
        ExplosionController.AskFor.CrystalExplosion(effectSpawnPoint.position);
        crystalObject.SetActive(false);
        isDead = true;
    }
    public void Respawn()
    {
        crystalCollider.enabled = true;
        crystalObject.SetActive(true);
        isDead = false;
    }
}
