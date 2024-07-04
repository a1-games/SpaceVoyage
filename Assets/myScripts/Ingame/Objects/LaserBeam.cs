using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour, IRespawnable
{
    [SerializeField] private bool isPortalLaser = false;



    [SerializeField] private LineRenderer line;
    [field: SerializeField] public Transform clashAnimation { get; private set; }
    [SerializeField] private Transform laserStartPoint_Transform;

    public Vector3 LaserStartPoint { get => laserStartPoint; }
    private Vector3 laserStartPoint;
    public MachineGun thisMachineGun;
    private Vector3 laserEndPoint;
    private float rayMagnitude;

    private bool canKill = false;
    private float savedTime;
    private float shootCooldown = 0f;

    public Vector3 DirectionVec { get => directionVec; }
    private Vector3 directionVec;
    [HideInInspector] public Direction direction;

    private MachineGun hitGun = null;
    private MachineGun lastHitGun = null;

    public void Awake()
    {
        SetDirection(Direction.Forward);
        SetDirectionFWBW();

        UpdateLaserStartPoint();

        this.clashAnimation.gameObject.SetActive(true);

        thisMachineGun = this.GetComponentInParent<MachineGun>();

        rayMagnitude = 20f;

        // get the end position of the laserbeam
        if (Physics.Raycast(laserStartPoint, transform.forward, out RaycastHit hit, rayMagnitude))
        {
            laserEndPoint = hit.point;
        }

        // set the visuals
        line.positionCount = 2;
        line.SetPosition(0, laserStartPoint);

        VisualizeLaser();
    }

    public void UpdateLaserStartPoint()
    {
        if (laserStartPoint_Transform)
            OverrideStartPoint(laserStartPoint_Transform.position);
    }

    public void OverrideStartPoint(Vector3 startPoint)
    {
        laserStartPoint = startPoint;
    }
    public void SetDirection(Direction dir)
    {
        direction = dir;
    }
    public void SetDirectionVec(Vector3 dir)
    {
        directionVec = dir;
    }
    private void SetDirectionFWBW()
    {
        if (direction == Direction.Forward)
            directionVec = transform.forward;
        else if (direction == Direction.Backward)
            directionVec = -transform.forward;
        else
            directionVec = Vector3.zero;
    }

    private void Update()
    {
        if (isPortalLaser)
            SetDirectionFWBW();


        if (Physics.Raycast(laserStartPoint, directionVec, out RaycastHit hit, rayMagnitude))
        {
            var hitObj = hit.transform.gameObject;

            laserEndPoint = hit.point;
            //Debug.DrawRay(laserStartPoint, directionVec * Vector3.Distance(laserStartPoint, hit.point));

            // player should always be killed if touched
            if (hitObj.CompareTag("Player"))
            {
                hitObj.GetComponent<PlayerHandler>().Death();
            }
            if (hitObj.CompareTag("Reflector"))
            {
                var reflector = hitObj.GetComponent<RootParent>().Parent.GetComponent<Reflector>();

                var reflection = Vector3.Reflect(directionVec, hit.normal);

                if (IngameController.AskFor.GameHasStarted)
                {
                    var newPoint = new Vector3(reflector.transform.position.x, laserEndPoint.y, reflector.transform.position.z);

                    reflector.SetLaserOn(newPoint, reflection);
                    laserEndPoint = newPoint;
                }
                else
                {
                    reflector.SetLaserOn(laserEndPoint, reflection);
                }

                //print("test");
                

            }
            if (hitObj.CompareTag("Teleporter"))
            {
                var hitTP = hitObj.GetComponentInParent<Teleporter>();
                
                if (thisMachineGun != null)
                {
                    hitTP.linkedTPLaserScript.thisMachineGun = thisMachineGun;
                    if (!thisMachineGun.activeTPLasers.Contains(hitTP))
                        thisMachineGun.activeTPLasers.Add(hitTP);
                }
                
                // get the way teleporter is facing
                if (hitTP.transform.forward == directionVec.normalized)
                    hitTP.ActivateLinkedLaser(Direction.Forward);
                else
                    hitTP.ActivateLinkedLaser(Direction.Backward);

                
                this.clashAnimation.gameObject.SetActive(false);

                //var newPoint = hitObj.transform.position;
                //laserEndPoint = new Vector3(newPoint.x, laserEndPoint.y, newPoint.z);
            }

            lastHitGun = hitGun;
            if (hitObj.CompareTag("MachineGun") )
            {
                if (IngameController.AskFor.GameHasStarted)
                {
                    hitGun = hitObj.GetComponent<MachineGun>();
                    if (hitGun != lastHitGun)
                    {
                        ExplosionController.AskFor.MachineGunHeatEffect(laserEndPoint);
                        canKill = false;
                        CanKillTimer();
                    }
                } 
            }
            else
                hitGun = null;

            if (canKill)
            {
                if (hitGun != null)
                {
                    hitGun.Death();
                    hitGun = null;
                }
                else
                    canKill = false;
            }
            
        }
        else
        {
            HideLaser();
        }
        VisualizeLaser();
    }

    public void HideLaser()
    {
        laserEndPoint = laserStartPoint;
        VisualizeLaser();
        this.gameObject.SetActive(false);
        //clashAnimation.gameObject.SetActive(false);
    }
    private void VisualizeLaser()
    {
        line.SetPosition(0, laserStartPoint);
        line.SetPosition(1, laserEndPoint);
        clashAnimation.position = laserEndPoint;
    }

    public void Death()
    {
        HideLaser();
    }

    public void Respawn()
    {
        hitGun = null;
        //SetStartPoint();
        //Death();
        //print("laser beam respawned");
    }
    private void CanKillTimer()
    {
        if (isActiveAndEnabled)
        StartCoroutine(CanKillRoutine());
    }
    private IEnumerator CanKillRoutine()
    {
        yield return new WaitForSeconds(1f);
        canKill = true;
    }
}
