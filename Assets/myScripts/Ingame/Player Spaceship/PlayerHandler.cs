using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour, IRespawnable
{
    [SerializeField] private bool isTutorial = false;
    //[SerializeField] private TMPro.TMP_Text DEBUG_TEXT;
    private Rigidbody rb;
    [Header("Death Collision:")]
    private Animator hoverAnimator;
    [SerializeField] private Collider wingCollider;
    [SerializeField] private PlayerCam playerCam;
    private StartPosition startPosScript;
    private PlayerMovement playerMoveScript;
    private LaguePortal laguePortal;
    public PortalTraveller thisTraveller { get; set; }
    [Header("Player Handling:")]
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private AudioSource deathSound;
    [SerializeField] private GameObject floatingEffect;
    public GameObject ShipObject { get; private set; }
    [SerializeField] private AudioSource[] shipFlyingSounds;
    [SerializeField] private ParticleSystem[] particles;
    [SerializeField] private Collider shipCollider;
    [Header("Other:")]
    [SerializeField] private NowhereChecker nowhereChecker;
    [SerializeField] private Spaceship myShip;
    [SerializeField] private SpaceshipSkinSpawner shipSpawner;
    [SerializeField] private LaserGun shooterScript;
    private Transform projectileSpawnPoint;

    
    private bool isPlayingShipFlyingSounds = false;
    private bool isShrinking = false;
    public float shrinkFactor = 0.02f;

    private void Awake()
    {

        //laguePortal = this.gameObject.GetComponent<LaguePortal>();
        wingCollider.enabled = false;
        rb = this.gameObject.GetComponent<Rigidbody>();
        playerMoveScript = this.gameObject.GetComponent<PlayerMovement>();
        startPosScript = this.gameObject.GetComponent<StartPosition>();

        shipSpawner.SpawnSavedSpaceshipSkin();
        if (shipSpawner.transform.childCount > 0)
        {
            for (int i = shipSpawner.transform.childCount - 1; i >= 0; i--)
            {
                var child = shipSpawner.transform.GetChild(i);
                if (child.name.Contains("Spaceship")) //if its spaceship skin
                {
                    ShipObject = child.gameObject;
                    myShip = ShipObject.GetComponent<Spaceship>();
                    //print("shipobject is not null: " + ShipObject.name);
                    break;
                }
            }
        }

        // get info from the spaceship
        hoverAnimator = myShip.shipAnimator;
        shooterScript.projectileSpawnPoint = myShip.bulletSpawnPoint;

        //DEBUG_TEXT.text = "shipobject = " + ShipObject.name;
        if (ShipObject.transform.childCount > 0)
        {
            for (int i = ShipObject.transform.childCount - 1; i >= 0; i--)
            {
                var child = ShipObject.transform.GetChild(i);

                if (child.name.Contains("After")) //if its arfterburner particles parent
                {
                    particles = child.GetComponentsInChildren<ParticleSystem>();
                    break;
                }
            }
        }


        ShipObject.SetActive(true);
        deathEffect.SetActive(false);
        floatingEffect.SetActive(true);

        if (isPlayingShipFlyingSounds)
        {
            for (int i = 0; i < shipFlyingSounds.Length; i++)
            {
                shipFlyingSounds[i].Stop();
            }
        }
        ParticleToggle(0);
    }

    private void Update()
    {
        if (isShrinking)
        {
            if (ShipObject.transform.localScale.z > 0f)
            {
                ShipObject.transform.localScale -= Vector3.one * shrinkFactor * Time.deltaTime;
            }

            if (ShipObject.transform.localScale.z < 0.002f)
            {
                isShrinking = false;
                ShipObject.SetActive(false);
                floatingEffect.SetActive(false);
                playerMoveScript.ToggleMove();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DeathBox"))
        {
            Death();
        }
    }
    public void ParticleToggle(int onOff)
    {
        for (int i = 0; i < particles.Length; i++)
        {
            if (onOff == 0)
            {
                particles[i].Stop();
            }
            else
            {
                particles[i].Play();
            }
        }
        
    }
    public void ToggleShipSounds()
    {
        if (!isPlayingShipFlyingSounds)
        {
            for (int i = 0; i < shipFlyingSounds.Length; i++)
            {
                shipFlyingSounds[i].Play();
            }
        }
        else
        {
            for (int i = 0; i < shipFlyingSounds.Length; i++)
            {
                shipFlyingSounds[i].Stop();
            }
        }
    }
    public void Death()
    {
        // return if already dead
        if (IngameController.AskFor.PlayerIsDead) return;
        if (!IngameController.AskFor.GameHasStarted) return;

        //collider.enabled = false;
        IngameController.AskFor.PlayerIsDead = true;
        IngameController.AskFor.CanRotatePlayCam = false;

        IngameController.AskFor.onPlayerDied.Invoke();

        if (isPlayingShipFlyingSounds) ToggleShipSounds();
        deathEffect.SetActive(true);
        deathSound.Play();
        // ship crashing effect
        wingCollider.enabled = true;
        rb.isKinematic = false;
        rb.useGravity = true;
        var a = Random.Range(-4f, 4f);
        rb.angularVelocity = new Vector3(a, a, 0f);
        hoverAnimator.enabled = false;
        //ShipObject.SetActive(false);
        //-----------------
        floatingEffect.SetActive(false);
        if (!isTutorial) UIStateHandler.AskFor.RefreshUI("GameLost");
        if (isTutorial) TutorialManager.AskFor.TutorialCycle();
        ParticleToggle(0);

        //traveller.ExitPortalThreshold();
        if (thisTraveller.CurrentLaguePortal) thisTraveller.CurrentLaguePortal.RemoveTraveller(thisTraveller);
    }
    public void Respawn()
    {
        nowhereChecker.Respawn();
        // set slice normal to zero so you dont keep the slice of the last portal you went through
        if (thisTraveller) thisTraveller.ExitPortalThreshold();

        playerMoveScript.IsMoving = false;
        playerMoveScript.IsRotating = false;

        if (!isPlayingShipFlyingSounds) ToggleShipSounds();
        deathEffect.SetActive(false);
        // ship crashing effect
        wingCollider.enabled = false;
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.angularVelocity = new Vector3(0f, 0f, 0f);
        hoverAnimator.enabled = true;
        playerCam.Respawn();
        //ShipObject.SetActive(false);
        //-----------------
        //ShipObject.SetActive(true);
        floatingEffect.SetActive(true);
        UIStateHandler.AskFor.RefreshUI("InGame");
        ParticleToggle(0);

        startPosScript.ResetPosRotScale();

        IngameController.AskFor.CanRotatePlayCam = true;
        IngameController.AskFor.PlayerIsDead = false;
        shipCollider.enabled = true;
    }
    public void PortalEnterShrinkEffect()
    {
        if (isPlayingShipFlyingSounds) ToggleShipSounds();
        isShrinking = true;
    }
}
