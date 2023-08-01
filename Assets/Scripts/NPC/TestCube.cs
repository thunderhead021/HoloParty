using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCube : NetworkBehaviour
{
    public Material[] testColor;
    public MeshRenderer mesh;

    [ServerCallback]
    private void OnTriggerEnter(Collider col)
	{
        Debug.Log("Hit");
        if (col.gameObject.tag.Equals("Player"))
        {
            mesh.material = testColor[col.transform.GetComponent<PlayerController>().charID];
        }
    }
}
