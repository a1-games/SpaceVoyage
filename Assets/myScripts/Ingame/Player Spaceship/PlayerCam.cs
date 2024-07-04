using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StartPosition))]
public class PlayerCam : MonoBehaviour
{
    private StartPosition startPosScript;
    public Transform lookTarget;
    public float cameraDistance = 5f;
    public float scrollSpeed = 3.4f;

    private Vector3 firstClickPos;
    private Camera thisCam;

    [SerializeField] private float winRotationSpeed = 5f;
    private bool gameWonRotation = false;

    private Vector3 spaceshipStartPos;
    private Vector3 SpaceshipCenter { get { return new Vector3(lookTarget.position.x, spaceshipStartPos.y, lookTarget.position.z); } }
    private Quaternion lastRotation;

    private Quaternion startRot;

    private void Start()
    {
        SceneLoader.AskFor.camerasToDisableOnLoading.Add(thisCam);

        gameWonRotation = false;

        startPosScript = GetComponent<StartPosition>();
        thisCam = GetComponent<Camera>();

        spaceshipStartPos = lookTarget.position;
        // rotate camera 45 degrees for better angle
        thisCam.transform.position = SpaceshipCenter;
        //thisCam.transform.position = lookTarget.position;
        thisCam.transform.Rotate(new Vector3(1f, 0f, 0f), -45f);

        thisCam.transform.Translate(0, 0, -cameraDistance);

        lastRotation = transform.rotation;
        startRot = transform.rotation;
    }

    private void Update()
    {
        if (gameWonRotation)
        {
            GameWonRotation();
            return;
        }

        if (IngameController.AskFor.CanRotatePlayCam)
        {
            MoveCamAround();
            Zoom();
        }
    }

    public void Zoom()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0f)
        {
            cameraDistance = Mathf.Clamp(cameraDistance - scroll  * Time.deltaTime * scrollSpeed, 1f, 7f); //1f is min distance and 7f is max 
            thisCam.transform.position = SpaceshipCenter;
            thisCam.transform.Translate(0, 0, -cameraDistance);
        }
    }

    public void StartGameWonRotation()
    {
        gameWonRotation = true;
    }
    private void GameWonRotation()
    {
        Vector3 thisPosNoY = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 targetPosNoY = new Vector3(SpaceshipCenter.x, SpaceshipCenter.y - 2f, SpaceshipCenter.z);

        Vector3 targetPosition = (thisPosNoY - targetPosNoY).normalized * cameraDistance;
        Vector3 moveDir = (thisPosNoY - targetPosNoY).normalized;
        
        Vector3 direction = transform.right;

        if (Vector3.Distance(transform.position, targetPosition) < 40f)
        {
            thisCam.transform.position += moveDir * Time.deltaTime * winRotationSpeed;
            //transform.position = Vector3.Lerp(transform.position, targetPosition, winRotationSpeed * Time.deltaTime);
        }
            
        thisCam.transform.position += direction.normalized * Time.deltaTime * winRotationSpeed;
            
        thisCam.transform.LookAt(lookTarget);
        
    }

    public void MoveCamAround()
    {
        if (Input.GetMouseButtonDown(0))
        {
            firstClickPos = thisCam.ScreenToViewportPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 direction = firstClickPos - thisCam.ScreenToViewportPoint(Input.mousePosition);

            thisCam.transform.position = SpaceshipCenter;
            thisCam.transform.Rotate(new Vector3(1f, 0f, 0f), direction.y * 180f);
            thisCam.transform.Rotate(new Vector3(0f, 1f, 0f), -direction.x * 180f, Space.World);

            thisCam.transform.Translate(0, 0, -cameraDistance);

            firstClickPos = thisCam.ScreenToViewportPoint(Input.mousePosition);

            lastRotation = transform.rotation;
        }
        else if (!GameSettings.AskFor.GetRotatePlayerCamWithPlayer()) // has to be called each frame, if player changes setting while ingame
        {
            thisCam.transform.position = SpaceshipCenter;
            transform.rotation = lastRotation;

            thisCam.transform.Translate(0, 0, -cameraDistance);
          
        }
    }

    public void Respawn()
    {
        /*
        thisCam.transform.position = SpaceshipCenter;
        transform.rotation = lastRotation;

        thisCam.transform.Translate(0, 0, -cameraDistance);
        */
    }
}
