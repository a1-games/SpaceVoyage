using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControl : MonoBehaviour
{
    public LayerMask raycastLayerGround;
    public LayerMask raycastLayerObject;
    public LayerMask raycastLayerPlayer;
    private Camera mainCamera;
    public Transform SelectedTransform { get; set; }
    private Movable SelectedMovable { get; set; }
    private Vector3 lastSavedPos;
    private Vector3 posOnClick;
    public AudioSource moveStuffSound;
    private float moveStuffPitchOnStart;
    public bool isClicking { get; private set; } = false;
    public int lastRotated_ID { get; private set; } = 0;
    public int lastMoved_ID { get; private set; } = 0;

    // steam achievement stuff:
    public int barrelsMoved { get; private set; } = 0;

    private void SetBarrelProgress()
    {
        MySteamAchievementHandler.SaveAchievementProgress(barrelsMoved, SteamStatNames.STAT_BARRELS_MOVED, SteamAchNames.ACHIEVEMENT_BARRELS_MOVED_250);
    }

    private void Awake()
    {
        mainCamera = Camera.main;
        SelectedTransform = null;
        moveStuffPitchOnStart = moveStuffSound.pitch;
    }

    private void Start()
    {
        barrelsMoved = MySteamAchievementHandler.GetStatProgress(SteamStatNames.STAT_BARRELS_MOVED);
    }

    private void Update()
    {
        if (IngameController.AskFor.GameHasStarted) return;

        MouseStuff();
    }

    public void MouseStuff()
    {
        HighlightOnMouseOver();

        if (!IngameController.AskFor.CanMoveMovables) return;

        MouseDown();

        MouseHold();

        if (Input.GetMouseButtonUp(0) && SelectedTransform)
        {
            var tag = SelectedTransform.gameObject.tag;
            if (tag == "Teleporter" || tag == "DemoMovable" || tag == "Reflector") // if its anything other than the barrel, rotate it
            {
                //if the item didnt move on click, rotate it
                //if (posOnClick == SelectedTransform.position)
                if (posOnClick == SelectedMovable.Ghost.position)
                {
                    float rotationDegree = 90f;
                    if (tag == "ButtonBlue") rotationDegree = -90f;

                    SelectedTransform.Rotate(Vector3.up, rotationDegree);
                    // PLAY ROTATION SOUND HERE

                    lastRotated_ID = SelectedTransform.GetInstanceID();
                }
            }
            SelectedTransform.position = SelectedMovable.Ghost.position;
            // steam achievement barrel stat
            if (tag == "BlockingBarrel")
            {
                barrelsMoved++;
                SetBarrelProgress();
            }
            /*
            if (tag == "ButtonGreen" || tag == "ButtonBlue")
            {
                // selected transform is already the moveParent of movable 
                var cprt = SelectedTransform.GetComponent<ChangePlayerRotTrigger>();
                cprt.ProjectPath(cprt.RightOrLeft);
            }
            */

            SelectedMovable.OnEndMove.Invoke();

            PlayMoveMovableSound();
            NullifyClickTarget();
        }

    }
    public void NullifyClickTarget()
    {
        SelectedTransform = null;
        SelectedMovable.HideGhost();
        SelectedMovable = null;
        isClicking = false;
        lastSavedPos = Vector3.zero;
        posOnClick = Vector3.zero;
    }
    private void MouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 180, raycastLayerObject))
            {
                if (hit.transform.gameObject.tag == "Movable")
                {
                    hit.transform.gameObject.TryGetComponent<Movable>(out Movable movable);
                    if (movable)
                    {
                        // return if this item is not supposed to be moved
                        if (movable.actAsPrePlacedObject) return;
                        SelectedTransform = movable.moveParent;
                        SelectedMovable = movable;
                        SelectedMovable.ReadyGhost();
                    }
                    else
                    {
                        SelectedTransform = hit.transform;
                    }

                    posOnClick = SelectedTransform.position;
                    isClicking = true;
                }
            }
        }
    }

    private float xDifference;
    private float zDifference;
    private Vector3 gridPos;
    private Vector3 freePos;

    private void MouseHold()
    {
        if (Input.GetMouseButton(0) && SelectedTransform != null)
        {
            //-------------------------------------------------
            // if the mouse is out of bounds, the object stays where it lat was

            if (GetMouseRayHit() == Vector3.zero)
            {
                //SelectedTransform.position = lastSavedPos;
                SelectedMovable.ShowGhost(lastSavedPos);
                return;
            }

            //-------------------------------------------------
            // make sure the object is snapped to a grid

            gridPos = GetMouseRayHit();
            freePos = GetMouseRayHit();
            freePos.y = SelectedTransform.position.y;

            xDifference = 0f;
            zDifference = 0f;

            //round to 0.75 because that is the grid size

            // for some reason adding or subtracting 0.75f depends on the axis being above or under 0.... this is a dumb fix
            if (gridPos.x < 0) xDifference = gridPos.x % 1.5f + 0.75f;
            else xDifference = gridPos.x % 1.5f - 0.75f;
            if (gridPos.z < 0) zDifference = gridPos.z % 1.5f + 0.75f;
            else zDifference = gridPos.z % 1.5f - 0.75f;

            // set gridpos with difference and correct Y pos
            gridPos.x = gridPos.x - xDifference;
            //gridPos.y = SelectedTransform.position.y;
            gridPos.y = SelectedMovable.Ghost.position.y;
            gridPos.z = gridPos.z - zDifference;

            //SelectedTransform.position = gridPos;
            SelectedMovable.ShowGhost(gridPos);
            SelectedTransform.position = freePos;

            // set last moved if object was moved
            if (posOnClick != SelectedMovable.Ghost.position) lastMoved_ID = SelectedTransform.GetInstanceID();
            //if (posOnClick != SelectedMovable.Ghost.position) lastMoved_ID = SelectedMovable.Ghost.GetInstanceID();
            //-------------------------------------------------
            // make sure the player cant place object on a map object
            // or another movable object
            // overlapsphere check grounded objects
            //Collider[] hitColliders = Physics.OverlapSphere(SelectedTransform.position, 0.4f);
            Collider[] hitColliders = Physics.OverlapSphere(SelectedMovable.Ghost.position, 0.4f);
            // the first frame will mess with the whole thing if lastSavedPos has not been set yet, but it needs to be at the bottom for this to work. therefore:
            //if (lastSavedPos == Vector3.zero) lastSavedPos = SelectedTransform.position;
            if (lastSavedPos == Vector3.zero) lastSavedPos = SelectedMovable.Ghost.position;
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].gameObject.CompareTag("PrePlacedObject") ||
                    hitColliders[i].gameObject.CompareTag("MachineGun") ||
                    hitColliders[i].gameObject.CompareTag("Movable"))
                {
                    //SelectedTransform.position = lastSavedPos;
                    SelectedMovable.ShowGhost(lastSavedPos);
                    return;
                }
            }
            // raycast check floating objects
            //if (Physics.Raycast(SelectedTransform.position, transform.up, out RaycastHit hit, 4f, raycastLayerPlayer))
            if (Physics.Raycast(SelectedMovable.Ghost.position, transform.up, out RaycastHit hit, 4f, raycastLayerPlayer))
            {
                if (hit.transform.gameObject.CompareTag("Player")) 
                {
                    //SelectedTransform.position = lastSavedPos;
                    SelectedMovable.ShowGhost(lastSavedPos);
                    return;
                }
            }
            //-------------------------------------------------
            // save this position
            if (/*SelectedTransform.position != lastSavedPos*/ SelectedMovable.Ghost.position != lastSavedPos) 
                CurrentSelectedMovableChangedTilePosition();
            //lastSavedPos = SelectedTransform.position;
            lastSavedPos = SelectedMovable.Ghost.position;

        }
    }

    private Movable currentMovable;

    private void HighlightOnMouseOver()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        //only check when we dont have a movable selected, because the highlight is distracting when moving pieces
        if (SelectedMovable == null && Physics.Raycast(ray, out RaycastHit hit, 180, raycastLayerObject))
        {
            if (!IngameController.AskFor.CanMoveMovables) return; //return is fine bc we only go into the elseif if we dont get to here
            if (hit.transform.gameObject.tag == "Movable")
            {
                hit.transform.gameObject.TryGetComponent<Movable>(out Movable movable);
                if (movable)
                {
                    // return if this item is not supposed to be moved
                    if (movable.actAsPrePlacedObject) return;
                    if (movable != currentMovable)
                    {
                        if (currentMovable) currentMovable.HighlightOff();
                        currentMovable = movable;
                        currentMovable.HighlightOn();
                    }
                }
            }
        }
        //else if (currentMovable != null && currentMovable != SelectedMovable) // stay selected while holding down clicked
        //hide if cursor goes away
        else if (currentMovable != null)
        {
            currentMovable.HighlightOff();
            currentMovable = null;
        }
    }

    public void CurrentSelectedMovableChangedTilePosition()
    {
        // play moving sound, different depending on the item
        PlayMoveMovableSound();
    }
    private void PlayMoveMovableSound()
    {
        moveStuffSound.pitch = Random.Range(moveStuffPitchOnStart - 0.03f, moveStuffPitchOnStart + 0.03f);
        moveStuffSound.Play();
    }


    public Vector3 GetMouseRayHit()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, raycastLayerGround))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
}
