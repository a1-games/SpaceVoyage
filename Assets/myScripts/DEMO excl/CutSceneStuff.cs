using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneStuff : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        IngameController.AskFor.OnPlayerBeginMoving();
    }

}
