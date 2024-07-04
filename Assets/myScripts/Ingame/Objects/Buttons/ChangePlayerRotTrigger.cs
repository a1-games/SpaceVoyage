using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Right,
    Left,
    Forward,
    Backward,
}
public class ChangePlayerRotTrigger : GameButton, IRespawnable
{
    [field: SerializeField] public Direction RightOrLeft { get; private set; }

    [SerializeField] private LayerMask buttonRaycastLayer;
    [SerializeField] private AudioSource soundEffect;
    [SerializeField] private Vector2 soundEffectMinMax = Vector2.one;
    [SerializeField] private Vector2 volumeMinMax = Vector2.one;

    private PlayerMovement playerMove;

    public GameObject buttonPush;
    //private Color usedColor;
    //private Color unusedColor;


    protected override void Start()
    {
        base.Start();
        playerMove = IngameController.AskFor.PlayerObject.GetComponent<PlayerMovement>();

        //usedColor = new Color(0, 0, 0, 1f);
        //unusedColor = new Color(1f, 1f, 1f, 1f);
    }
    private ChangePlayerRotTrigger GetButtonBehindMe()
    {
        if (Physics.Raycast(transform.position, -transform.forward, out RaycastHit hit, 24f, buttonRaycastLayer))
        {
            var movable = hit.transform.gameObject.GetComponent<Movable>();
            return movable.moveParent.GetComponent<ChangePlayerRotTrigger>();
        }
        return null;
    }
    // have one script that handles the turning and raycasting on the player object, and make this function call that script every time a button moves.
    private void ProjectPath(Direction direction)
    {
        Vector3 castDir;
        if (direction == Direction.Right)
            castDir = transform.right;
        else
            castDir = -transform.right;

        if (Physics.Raycast(transform.position, castDir, out RaycastHit hit, 24f, buttonRaycastLayer))
        {
            var movable = hit.transform.gameObject.GetComponent<Movable>();
            var hitButton = movable.moveParent.gameObject;
            var id = this.GetInstanceID();

            if (hitButton.CompareTag("ButtonGreen"))
            {
                RotateAndProject(hitButton, 90f, direction, id);
            }
            else if (hitButton.CompareTag("ButtonBlue"))
            {
                RotateAndProject(hitButton, -90f, direction, id);
            }

        }
    }
    private void RotateAndProject(GameObject hitobj, float degrees, Direction dir, int instanceID)
    {
        var cprt = hitobj.GetComponent<ChangePlayerRotTrigger>();
        int rotationAttemps = 0;
        while (cprt.GetButtonBehindMe() != this && rotationAttemps < 8)
        {
            hitobj.transform.Rotate(Vector3.up, degrees);
            rotationAttemps++;
        }
        //cprt.ProjectPath(dir);
    }

    private void OnTriggerStay(Collider other)
    {
        if (triggered) return;
        if (!IngameController.AskFor.GameHasStarted) return;
        if (playerMove == null) return;
        if (playerMove.IsRotating) return;
        if (other.gameObject.CompareTag("Player"))
        {
            playerMove.RotateTowards(this.transform, RightOrLeft);
            currentUses++;
            SetCountDown(usesBeforeTurnOff - currentUses);
            triggered = true;

            PlaySound();
            if (currentUses >= usesBeforeTurnOff) Death();
        }
    }

    public void PlaySound()
    {
        if (!IngameController.AskFor.GameHasStarted) return;
        soundEffect.pitch = Random.Range(soundEffectMinMax.x, soundEffectMinMax.y);
        soundEffect.volume = Random.Range(volumeMinMax.x, volumeMinMax.y);
        soundEffect.Play();
    }

}
