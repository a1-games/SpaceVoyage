///
///	Made by a1-creator
///	All rights reserved to a1-creator
///

using System.Collections;
using UnityEngine;

public enum AnimateType
{
    Increase,
    Decrease,
}

public class AnimateSize : MonoBehaviour
{
    [Tooltip("If this Transform is empty, it uses the transform of the object this is attached to.")] 
    [SerializeField] private Transform objectTrans;
    [Header("Animation Control")]
    [SerializeField] private AnimateType animateType;
    [SerializeField] private float animationSpeedModifier = 1f;
    [SerializeField] private AnimationCurve curve;
    private float lerpFloat = 0f;
    private Vector3 startScale;
    private bool animIsOver = false;

    private void Awake()
    {
        // objectTrans is never null
        if (!objectTrans) objectTrans = this.transform;

        startScale = objectTrans.localScale;
    }

    public void DoSizeAnimation(AnimateType animationType, bool pingPong = false)
    {
        StartCoroutine(RunAnimation(animationType, pingPong));
    }

    public IEnumerator RunAnimation(AnimateType type, bool pingPong)
    {
        animateType = type;
        if (animateType == AnimateType.Decrease) lerpFloat = 1f;
        else lerpFloat = 0f;

        while (!animIsOver)
        {
            Animate(pingPong);
            yield return new WaitForSecondsRealtime(0f);
        }

        // reset so that the bool can be reused
        animIsOver = false;
    }

    private void Animate(bool pingPong = false)
    {
        // set the right animation type
        var speed = 0.01f * Time.deltaTime * animationSpeedModifier;
        if (animateType == AnimateType.Increase)
        {
            lerpFloat += speed;
            if (lerpFloat >= 1f && !pingPong)
            {
                animIsOver = true;
                return;
            }
        }

        if (animateType == AnimateType.Decrease)
        {
            lerpFloat -= speed;
            if (lerpFloat <= 0f && !pingPong)
            {
                animIsOver = true;
                return;
            }
        }

        // make lerpTime into the curve animation time
        var lerpTime = curve.Evaluate(lerpFloat);
        if (pingPong) lerpTime = curve.Evaluate(Mathf.PingPong(lerpFloat, 1f));

        // affect the objects transform
        objectTrans.localScale = startScale * lerpTime;
    }
    
}
