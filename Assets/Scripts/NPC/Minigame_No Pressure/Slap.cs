using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slap : NetworkBehaviour
{
    [SyncVar]
    public bool isPlatform;

    [ServerCallback]
    private void OnTriggerStay2D(Collider2D collision)
	{
        if (collision.gameObject.tag.Equals("Player"))
        {
            Debug.Log("player in " + gameObject.name);
        }
    }

	[ClientRpc]
    private void PlayerOut(GameObject player)
    {
        Debug.Log("player out");
    }
}
