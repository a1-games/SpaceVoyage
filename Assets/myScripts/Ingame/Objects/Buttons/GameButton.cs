using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class GameButton : MonoBehaviour
{
    [field: SerializeField] private TMP_Text CountdownText { get; set; }
    [field: SerializeField] public ButtonColor ButtonColor { get; set; }
    [field: SerializeField] public Animator PushAnimator { get; set; }
    [SerializeField] protected short usesBeforeTurnOff = 1;
    protected short currentUses;
    protected bool isDead;
    protected bool triggered = false;

    protected virtual void Start()
    {
        // if there is a text object reference, set the max uses
        if (CountdownText)
        {
            SetCountDown(usesBeforeTurnOff);
            //IngameController.AskFor.playerPressedGo.AddListener(() => CountdownText.transform.gameObject.SetActive(false));
        }
    }

    protected void SetCountDown(int number)
    {
        if (!CountdownText) return;
        CountdownText.text = number.ToString();
        if (number <= 0) CountdownText.transform.gameObject.SetActive(false);
    }

    public virtual void Death()
    {
        if (isDead) return;

        isDead = true;
        PushAnimator.SetBool("isUsed", true);
    }

    public virtual void Respawn()
    {
        // reset uses no matter if the button has been pushed
        currentUses = 0;
        if (CountdownText)
        {
            CountdownText.transform.gameObject.SetActive(true);
            SetCountDown(usesBeforeTurnOff);
        }

        if (!isDead) return;
        isDead = false;
        PushAnimator.SetBool("isUsed", false);

        triggered = false;
    }

    public virtual void OnTriggerExit(Collider other)
    {
        // make the button reusable if all its uses haven't been spent
        if (currentUses < usesBeforeTurnOff) triggered = false;
    }
}
