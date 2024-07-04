using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootButton : GameButton, IRespawnable
{
    private LaserGun playerLaser;
    private PlayerMovement playerMove;

    //death respawn
    public GameObject buttonPush;

    protected override void Start()
    {
        base.Start();
        playerLaser = IngameController.AskFor.PlayerObject.GetComponent<LaserGun>();
        playerMove = IngameController.AskFor.PlayerObject.GetComponent<PlayerMovement>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (triggered) return;
        if (!IngameController.AskFor.GameHasStarted) return;
        if (playerMove == null) return;
        if (playerMove.IsRotating) return;
        if (other.gameObject.CompareTag("Player"))
        {
            currentUses++;
            Vector3 tr = playerMove.gameObject.transform.position;
            if (Vector3.Distance(new Vector3(tr.x, transform.position.y, tr.z), transform.position) < 0.1f)
            {
                triggered = true;

                playerLaser.Shoot();
                
                Death();
            }
        }
    }

}
