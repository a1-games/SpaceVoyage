using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reflector : MonoBehaviour
{
    public bool LaserIsOn { get; private set; }

    [SerializeField] private LaserBeam laser;

    private bool laserHidden = false;

    private void Awake()
    {
        laser.SetDirection(Direction.Forward);
        SetLaserOff();
        
    }


    private void Update()
    {
        if (!LaserIsOn)
        {
            SetLaserOff();
        }
        LaserIsOn = false;
    }

    public void SetLaserOn(Vector3 laserStartPoint, Vector3 laserDirection)
    {
        if (laserDirection != laser.DirectionVec ||
            laserStartPoint != laser.LaserStartPoint)
        {
            laser.OverrideStartPoint(laserStartPoint);
            laser.SetDirectionVec(laserDirection);
        }

        laser.gameObject.SetActive(true);
        LaserIsOn = true;
        laserHidden = false;
    }

    public void SetLaserOff()
    {
        if (laserHidden) return;
        laserHidden = true;
        laser.gameObject.SetActive(false);
    }
}
