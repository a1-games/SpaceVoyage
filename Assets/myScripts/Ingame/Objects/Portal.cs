using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IRespawnable
{
    [SerializeField] private Material mat;
    [SerializeField] private GameObject portalEffect;
    [SerializeField] private GameObject portalExplosionEffect;

    private void Awake()
    {
        portalEffect.SetActive(false);
        portalExplosionEffect.SetActive(false);
        mat.DisableKeyword("_EMISSION");
    }
    public void PortalCheck()
    {
        if (IngameController.AskFor.GatheredCrystals >= IngameController.AskFor.CrystalCount)
        {
            ActivatePortal();
        }
    }
    private void ActivatePortal()
    {
        IngameController.AskFor.PortalIsOpen = true;
        portalEffect.SetActive(true);
        mat.EnableKeyword("_EMISSION");
        // play portal turn on sound
    }
    public void Respawn()
    {
        IngameController.AskFor.PortalIsOpen = false;
        portalEffect.SetActive(false);
        portalExplosionEffect.SetActive(false);
        mat.DisableKeyword("_EMISSION");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!IngameController.AskFor.PortalIsOpen) return;
        if (other.gameObject.CompareTag("Player"))
        {
            var ingC = IngameController.AskFor;
            // end game
            ingC.GameIsOver();
            GameSaves.AskFor.SaveLevelComplete(SceneManager.GetActiveScene().name);
            ingC.PlayerObject.GetComponent<PlayerHandler>().PortalEnterShrinkEffect();
            portalExplosionEffect.SetActive(true);
            ingC.PlayerCam.StartGameWonRotation();
            //print("Player has won the game!");
            UIStateHandler.AskFor.RefreshUI("GameWon");
        }
    }
    public void Death()
    {

    }
}
