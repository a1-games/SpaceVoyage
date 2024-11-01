///
///	Made by a1-creator
///	All rights reserved to a1-creator
///

using System.Collections;
using UnityEngine;

public class ColourblindAssistObject : MonoBehaviour
{
    [SerializeField] private bool idleRotation = false;
    [SerializeField] private Direction rightOrLeft = Direction.Right;
    [Tooltip("If this is null, script uses gameobject it is attached to instead")] 
    [SerializeField] private GameObject colourblindAssistObject;
    private Coroutine routine;
    private void Start()
    {
        routine = null;
        GameSettings.AskFor.CB_Assists.Add(this);
        RefreshThisObject();
    }
    private void OnEnable()
    {
        if (idleRotation && routine == null) routine = StartCoroutine(IdleRotation());
    }
    private void OnDisable()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
    }
    public void RefreshThisObject()
    {
        if (GameSettings.AskFor.GetColourblindAssist())
        {
            ShowThisObject(true);
        }
        else
        {
            ShowThisObject(false);
        }
    }

    private void ShowThisObject(bool trueFalse)
    {
        if (colourblindAssistObject)
        {
            colourblindAssistObject.SetActive(trueFalse);
        }
        else
        {
            this.gameObject.SetActive(trueFalse);
        }
        //if (trueFalse == true) StartIdleRotation();
    }

    private IEnumerator IdleRotation()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.04f);
            var curRot = transform.rotation.eulerAngles;
            if (rightOrLeft == Direction.Left)
                curRot.y -= 2f;
            else
                curRot.y += 2f;
            transform.rotation = Quaternion.Euler(curRot);
        }
    }
    /*
    private IEnumerator IdleRotation()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(idleRotationCd);
            var curRot = transform.rotation.eulerAngles;
            if (rightOrLeft == Direction.Left)
                curRot.y -= 90f;
            else
                curRot.y += 90f;
            transform.rotation = Quaternion.Euler(curRot);
        }
    }*/
}
