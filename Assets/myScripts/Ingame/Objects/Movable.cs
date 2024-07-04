using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightPlus;
using UnityEngine.Events;

public class Movable : MonoBehaviour
{
    [field: SerializeField] public bool actAsPrePlacedObject { get; private set; }
    [field: SerializeField] public Transform moveParent { get; private set; }
    [field: SerializeField] public Transform parentToHighlight { get; private set; }
    [field: SerializeField] public Transform graphicsParent { get; private set; }
    [field: SerializeField] public Material ghostMaterial { get; private set; }
    [field: SerializeField] public HighlightEffect highlightScript { get; private set; }
    [field: SerializeField] public HighlightProfile highlightProfile { get; private set; }
    public Transform Ghost { get; private set; }
    public UnityEvent OnEndMove;

    private void Awake()
    {
        //print("highlightscript.profile null? : " + highlightScript.profile == null);
        if (parentToHighlight == null) parentToHighlight = graphicsParent;
        highlightScript.profile = highlightProfile;
        highlightScript.target = parentToHighlight;
        
        HighlightOff();
    }
    private void AddAllDescendants(Transform from, List<GameObject> to)
    {
        for (int i = 0; i < from.childCount; i++)
        {
            var child = from.GetChild(i);
            to.Add(child.gameObject);
            AddAllDescendants(child, to);
        }
    }
    public void ReadyGhost()
    {
        //this will create the ghost where the button was clicked
        if (Ghost == null)
        {
            //instantiate the ghost
            Ghost = Instantiate(graphicsParent, graphicsParent.position, graphicsParent.rotation);
            Ghost.localScale = graphicsParent.transform.lossyScale;
            Ghost.localScale *= 0.9f;
            // get all changable objects
            var objects = new List<GameObject>();
            objects.Add(Ghost.gameObject);
            AddAllDescendants(Ghost, objects);

            var renderers = new List<MeshRenderer>();
            for (int i = 0; i < objects.Count; i++)
            {
                // look for meshrenderers
                if (objects[i].TryGetComponent<MeshRenderer>(out MeshRenderer childRend))
                {
                    renderers.Add(childRend);
                }
                // destroy animators
                if (objects[i].TryGetComponent<Animator>(out Animator anim))
                {
                    Destroy(anim);
                }
            }

            // change renderer materials
            for (int i = 0; i < renderers.Count; i++)
            {
                var mats = renderers[i].materials;
                for (int j = 0; j < mats.Length; j++)
                {
                    var emit = mats[j].IsKeywordEnabled("_EMISSION");
                    var eColor = mats[j].GetColor("_EmissionColor");
                    var tempTexture = mats[j].mainTexture;
                    Material newGhostMat = new Material(ghostMaterial);
                    var newColor = new Color(eColor.r, eColor.g, eColor.b, newGhostMat.color.a);

                    mats[j] = newGhostMat;
                    mats[j].mainTexture = tempTexture;
                    // assign emission color if other material was emitting
                    mats[j].color = newColor;
                    if (emit)
                    {
                        mats[j].EnableKeyword("_EMISSION");
                        mats[j].SetColor("_EmissionColor", eColor * 0.3f);
                    }
                    
                }
                renderers[i].materials = mats;
            }
        }
        else
        {
            Ghost.position = graphicsParent.position;
            Ghost.localRotation = moveParent.localRotation;
        }
        // setactive false last in case it was just instatiated
        Ghost.gameObject.SetActive(false);
    }

    public void ShowGhost(Vector3 pos)
    {
        ReadyGhost();
        if (Ghost != null)
        {
            Ghost.gameObject.SetActive(true);
            Ghost.position = pos;
        }
    }
    public void HideGhost()
    {
        if (Ghost)
            Ghost.gameObject.SetActive(false);
    }

    public void HighlightOn()
    {
        highlightScript.highlighted = true;
    }
    public void HighlightOff()
    {
        if (Ghost) HideGhost();
        highlightScript.highlighted = false;
    }
}
