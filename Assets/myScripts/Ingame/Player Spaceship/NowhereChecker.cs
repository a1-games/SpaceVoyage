using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NowhereChecker : MonoBehaviour
{
    [SerializeField] private float timeBetweenChecks = 2f;

    private Coroutine cycle;

    private void Awake()
    {
        if (IngameController.AskFor)
            IngameController.AskFor.wrongdirectionWarning.SetActive(false);
    }
    private void Start()
    {
        IngameController.AskFor.wrongdirectionWarning.SetActive(false);
        cycle = StartCoroutine(CheckInFrontOfSelf(timeBetweenChecks));
    }

    private void OnDisable()
    {
        if (cycle != null)
            StopCoroutine(cycle);
    }

    private IEnumerator CheckInFrontOfSelf(float seconds)
    {
        while (!IngameController.AskFor.GameHasEnded)
        {
            yield return new WaitForSeconds(seconds);
            if (!Physics.Raycast(transform.position, transform.forward, 24f))
            {
                if (IngameController.AskFor.PlayerIsDead) yield return null;
                if (!IngameController.AskFor.GameHasStarted) yield return null;

                IngameController.AskFor.wrongdirectionWarning.SetActive(true);
            }
        }

    }

    public void Respawn()
    {
        if (IngameController.AskFor)
            IngameController.AskFor.wrongdirectionWarning.SetActive(false);
    }
}
