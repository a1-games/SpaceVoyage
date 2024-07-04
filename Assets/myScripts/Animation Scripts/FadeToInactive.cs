///
///	Made by a1-creator
///	All rights reserved to a1-creator
///

using UnityEngine;
using System.Collections;

public class FadeToInactive : MonoBehaviour
{
    [Header("Animation Control")]
    [SerializeField] private float animationSpeedModifier = 1f;
    [SerializeField] private AnimationCurve curve;
    private float lerpFloat = 0f;
    private bool animIsOver = false;
    

    public void StartFadingThisObject()
    {
        StartCoroutine(FadeAnimation(this.gameObject));
    }
    
    private IEnumerator FadeAnimation(GameObject go)
    {
        go.TryGetComponent<MeshRenderer>(out MeshRenderer meshRend);
        go.TryGetComponent<SkinnedMeshRenderer>(out SkinnedMeshRenderer skinMeshRend);

        Material mat = null;
        if (meshRend != null) mat = meshRend.material;
        if (skinMeshRend != null) mat = skinMeshRend.material;


        while (!animIsOver)
        {
            Animate(mat);
            yield return new WaitForSecondsRealtime(0f);
        }

        // reset so that the bool can be reused
        animIsOver = false;
        // set inactive
        go.SetActive(false);

        // color is visible at next starting point
        Color c = mat.color;
        c.a = 1f;
        mat.color = c;
    }

    private void Animate(Material mat)
    {
        // set the right animation type
        var speed = 0.01f * Time.deltaTime * animationSpeedModifier;
        
        lerpFloat += speed;

        // go out of the method if the destination has been reached
        if (lerpFloat >= 1f)
        {
            animIsOver = true;

            // reset lerpfloat for respawn use case
            lerpFloat = 0f;
            
            return;
        }

        // make lerpTime into the curve animation time
        var lerpTime = curve.Evaluate(lerpFloat);

        // affect the objects visuals
        Color newColor = mat.color;
        newColor.a = lerpTime;
        mat.color = newColor;
    }
}
