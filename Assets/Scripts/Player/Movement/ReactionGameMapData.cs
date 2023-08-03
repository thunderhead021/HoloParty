using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionGameMapData : BaseMapData
{
    public override void Movement()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Debug.Log("Space");
    }
}
