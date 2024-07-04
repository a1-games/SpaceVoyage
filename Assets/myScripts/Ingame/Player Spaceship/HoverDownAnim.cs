using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverDownAnim : MonoBehaviour
{
    [SerializeField] private float lerpSpeed = 1f;
    [SerializeField] private Transform securityCam;
    [SerializeField] private float howCloseBeforeLockPosition = 0.1f;
    public AnimationCurve curve;

    private Spaceship ship;
    private Vector3 landingPosition;
    private Vector3 skyPosition;
    private float lerpFloat = 0f;

    private bool firstFrame = true;

    private void Start()
    {
        landingPosition = this.transform.position;
        securityCam = IngameController.AskFor.cameras[0].transform;
        skyPosition = securityCam.position;
        skyPosition.y -= 2f;

        IngameController.AskFor.GameCanStart = false;
    }
    private void MyStart()
    {
        // position close to cam
        transform.position = skyPosition;
        IngameController.AskFor.CanMoveMovables = false;
    }
    private void OnDisable()
    {
        // if this is disabled in the middle of the process of hovering down, end the process
        if (lerpFloat != 0)
        {
            DestroyThis();
        }
    }
    private void FixedUpdate()
    {
        if (firstFrame)
        {
            MyStart();
            firstFrame = false;
        }
        //if (ship == null) return;    //start once ship is not null

        lerpFloat += Time.deltaTime * lerpSpeed;
        var lerpTime = curve.Evaluate(Mathf.Clamp01(lerpFloat));
        transform.position = Vector3.Lerp(skyPosition, landingPosition, lerpTime);

        if (Vector3.Distance(transform.position, landingPosition) < howCloseBeforeLockPosition)
        {
            transform.position = landingPosition;
            DestroyThis();
        }
    }

    private void DestroyThis()
    {
        IngameController.AskFor.GameCanStart = true;
        IngameController.AskFor.CanMoveMovables = true;
        Destroy(this);
    }

}
