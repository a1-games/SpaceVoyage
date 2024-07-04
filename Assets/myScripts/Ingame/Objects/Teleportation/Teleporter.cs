using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour, IRespawnable
{
    [field: SerializeField] public GameObject PortalAnimation { get; private set; }
    [field: SerializeField] public GameObject PortalArrow { get; private set; }
    [field: SerializeField] public LaguePortal LaguePortal { get; private set; }
    [field: SerializeField] public GameObject Laser { get; private set; }
    [SerializeField] private Collider[] collidersToToggle;

    public GameObject linkedTPLaser { get; private set; }
    public LaserBeam linkedTPLaserScript { get; private set; }

    private void Awake()
    {
        Laser.SetActive(false);
        linkedTPLaser = LaguePortal.linkedPortal.GetComponentInParent<Teleporter>().Laser;
        linkedTPLaserScript = linkedTPLaser.GetComponent<LaserBeam>();
        Respawn();
    }


    public void ActivateLinkedLaser(Direction forwardBackward)
    {
        linkedTPLaser.SetActive(true);
        linkedTPLaserScript.SetDirection(forwardBackward);
    }

    public void ActivateTeleporter()
    {
        PortalAnimation.SetActive(true);
        PortalAnimation.GetComponent<AnimateSize>().DoSizeAnimation(AnimateType.Increase);
        //portalArrow.SetActive(false); the function below will do this for you
        PortalArrow.GetComponent<FadeToInactive>().StartFadingThisObject();

        foreach (Collider collider in collidersToToggle)
        {
            collider.enabled = true;
        }
    }

    public void Death()
    {
        throw new System.NotImplementedException();
    }

    // respawn
    public void Respawn()
    {
        PortalAnimation.SetActive(false);
        PortalArrow.SetActive(true);
        linkedTPLaserScript.Respawn();
        linkedTPLaserScript.HideLaser(); // sets object to inactive
        
        foreach (Collider collider in collidersToToggle)
        {
            collider.enabled = false;
        }
    }
}
