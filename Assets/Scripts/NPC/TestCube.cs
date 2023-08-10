using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCube : NetworkBehaviour
{
    public Material[] testColor;
    
    public MeshRenderer mesh;

    [SyncVar]
    public int curColorID;

    private void ChangeColor(int colorID) 
    {
        curColorID = colorID;  
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider col)
	{
        if (col.gameObject.tag.Equals("Player"))
        {
            ChangeColor(col.transform.GetComponent<PlayerController>().charID);
            SetColor();
        }
    }

    [ClientRpc]
    private void SetColor() 
    {
        mesh.material = testColor[curColorID];
    }
}
