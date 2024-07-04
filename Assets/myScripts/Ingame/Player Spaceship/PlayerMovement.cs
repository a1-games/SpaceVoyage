using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [field: SerializeField] public float SpeedModifier { get; set; } = 1f;
    [field: SerializeField] public float MoveSpeed { get; private set; } = 1f;
    [SerializeField] private float turnSpeed = 2f;

    public bool IsMoving { get; set; } = false;
    public bool IsRotating { get; set; } = false;

    private float lerpFloatRotation = 0f;
    [SerializeField] private AnimationCurve lerpCurve;

    private Vector3 targetDir;
    private Vector3 targetPos;
    private Vector3 oldDir;
    private Vector3 oldPos;

    private Vector3 rotationCenter;
    private Vector3 offsetOldPos;
    private Vector3 offsetTargetPos;
    
    void FixedUpdate()
    {
        if (!IngameController.AskFor.GameHasStarted) return;
        if (IngameController.AskFor.PlayerIsDead) return;

        if (IsMoving)
        {
            transform.position += transform.forward * SpeedModifier * MoveSpeed * Time.deltaTime;
        }
        if (IsRotating)
        {
            IsMoving = false;
            lerpFloatRotation += 0.01f * turnSpeed * Time.deltaTime * SpeedModifier;
            // check if rotation is over
            if (lerpFloatRotation >= 1f)
            {
                IsRotating = false;
                IsMoving = true;
            }

            var lerpTime = lerpCurve.Evaluate(lerpFloatRotation);

            transform.position = Vector3.SlerpUnclamped(offsetOldPos, offsetTargetPos, lerpTime);
            transform.position += rotationCenter;

            // multiply by a little more than 1 so that the rotation is ahead of the movement and therefore lined up straight before the turn is complete (no jittery movement)
            var dir = Vector3.Lerp(oldDir, targetDir, lerpTime * 1.3f);
            transform.LookAt(transform.position + dir);

        }
        
    }

    public void ToggleMove()
    {
        if (!IsMoving) IsMoving = true;
        else IsMoving = false;
    }

    public void RotateTowards(Transform buttonTransform, Direction rightLeft)
    {
        if (!IngameController.AskFor.GameHasStarted) return;

        var center = buttonTransform.position;
        var entryPoint = transform.position;
        var dist = Vector3.Distance(center, entryPoint);
        var turnDir = transform.right;
        if (rightLeft == Direction.Left) turnDir = - transform.right;
        var exitPoint = center + turnDir * dist;

        oldDir = transform.forward;
        oldPos = transform.position;
        targetDir = (exitPoint - center).normalized;
        targetPos = exitPoint;

        var combinedPointsCorner = entryPoint + transform.right * dist;
        if (rightLeft == Direction.Left) combinedPointsCorner = entryPoint - transform.right * dist;
        var offsetDir = (center - combinedPointsCorner).normalized;

        var centerOfLerp = (entryPoint + exitPoint) * 0.5f;
        centerOfLerp -= offsetDir * 0.36f; // placing the center of the circle to lerp in

        offsetOldPos = oldPos - centerOfLerp;
        offsetTargetPos = targetPos - centerOfLerp;

        rotationCenter = centerOfLerp;

        lerpFloatRotation = 0f;
        IsRotating = true;
    }

}
