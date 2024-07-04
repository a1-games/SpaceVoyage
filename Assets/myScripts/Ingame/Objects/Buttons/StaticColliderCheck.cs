using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticColliderCheck : MonoBehaviour
{
    [Tooltip("The script will auto-disable/enable the static collider depending on the movable collider. It is not for any other purpose!")]
    [SerializeField] public GameObject movableCollider;

    private void Awake()
    {
        if (!movableCollider.activeSelf)
        {
            this.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}
