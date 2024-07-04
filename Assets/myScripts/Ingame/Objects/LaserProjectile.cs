using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LaserTeam
{
    Ally,
    Enemy,
}
public class LaserProjectile : MonoBehaviour
{
    [SerializeField] private float projectileSpeed = 0.2f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private LaserTeam allyOrEnemy;
    //[SerializeField] private GameObject deathEffect;

    //private Vector3 direction;
    private float timeOnSpawn;

    private void Awake()
    {
        timeOnSpawn = Time.time;
    }
    private void Start()
    {
        IngameController.AskFor.onPlayerDied.AddListener(() => { Death(); } );
    }
    private void FixedUpdate()
    {
        transform.position += transform.forward * Time.deltaTime * projectileSpeed;
        //transform.LookAt(direction); //rotation is set on instantiate in LaserGun
        //death
        if (Time.time - timeOnSpawn > lifetime) Destroy(this.gameObject);
    }

    public void SetDirection(Vector3 dir)
    {
        transform.LookAt(transform.position + dir);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reflector"))
        {
            var reflectNormal = other.transform.forward;

            //print("reflectNormal = " + reflectNormal);
            //print("forward = " + transform.forward);
            //print("forward after = " + Vector3.Reflect(transform.forward, reflectNormal));

            var reflection = Vector3.Reflect(transform.forward, reflectNormal);

            var oTR = other.transform.position;
            transform.position = new Vector3(oTR.x, this.transform.position.y, oTR.z);

            transform.rotation = Quaternion.LookRotation(reflection);

            var tr = transform.position;
            tr += transform.forward * 0.15f;
            transform.position = tr;

        }
        if (other.CompareTag("Player") && allyOrEnemy == LaserTeam.Enemy)
        {
            other.gameObject.GetComponent<PlayerHandler>().Death();
            Death();
        }
        if (other.CompareTag("MachineGun") /* && allyOrEnemy == LaserTeam.Ally*/)
        {
            other.gameObject.GetComponent<MachineGun>().Death();
            Death();
        }
        if (other.CompareTag("PrePlacedObject"))
        {
            other.gameObject.TryGetComponent<CrystalPickup>(out CrystalPickup cp);
            if (cp)
            {
                cp.Death();
                Death();
            }
        }
        if (other.CompareTag("DeathBox"))
        {
            Death();
        }
    }

    private void Death()
    {
        ExplosionController.AskFor.MissileExplosion(transform.position);
        Destroy(this.gameObject);
    }
}
