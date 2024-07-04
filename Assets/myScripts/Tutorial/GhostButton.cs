using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonColor
{
    Blue,
    Green,
    Red,
}

public class GhostButton : MonoBehaviour
{
    public ButtonColor colorToFetch;
    [Header("Only needed in tutorial level:")]
    public TutorialManager tutorial;

    private MouseControl mouseControl;
    private Vector3 lastPos;
    private bool hasBeenPlaced = false;

    private void Start()
    {
        mouseControl = IngameController.AskFor.gameObject.GetComponent<MouseControl>();
    }
    private void Update()
    {
        if (hasBeenPlaced)
            Death();
    }
    private void OnTriggerStay(Collider other)
    {
        if (hasBeenPlaced) return;

        if (other.gameObject.CompareTag("Movable"))
        {
            other.gameObject.TryGetComponent<Movable>(out Movable movableScript);
            if (!movableScript) return;

            movableScript.moveParent.gameObject.TryGetComponent<GameButton>(out GameButton buttonScript);
            if (!buttonScript) return;

            ButtonColor fetchedColor = ButtonColor.Blue;

            if (buttonScript) fetchedColor = buttonScript.ButtonColor;


            if (fetchedColor == colorToFetch)
            {
                mouseControl.SelectedTransform.position = this.transform.position;
                mouseControl.NullifyClickTarget();
                hasBeenPlaced = true;

                // \/ this code was made before the grid system \/
                /*lastPos = other.gameObject.transform.position;
                other.gameObject.transform.position = this.transform.position;
                if (lastPos == this.transform.position) hasBeenPlaced = true; */
            }
        }
    }
    private void Death()
    {
        if (tutorial) tutorial.greenButtonGhost = null;
        tutorial?.TutorialCycle();
        Destroy(this.gameObject);
    }
}
