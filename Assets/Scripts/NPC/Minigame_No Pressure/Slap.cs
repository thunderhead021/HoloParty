using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slap : NetworkBehaviour
{
    [SyncVar]
    public bool isPlatform;

    public Collider2D slapColider;

    float resetTime = 1f;

    [ServerCallback]
    public void PlayersOut()
    {
        if (isPlatform)
            return;

        // Get all the colliders2D inside this game object's collider
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, slapColider.bounds.size, 0);

        foreach (Collider2D collider in colliders)
        {
			// Check if the collider belongs to a game object
			GameObject model = collider.gameObject;

			if (model != null && model.tag.Equals("Player"))
			{
				Debug.Log("Player out");
                NoPressureMapData playerMapdata = (NoPressureMapData)model.GetComponentInParent<PlayerDataForMap>().GetMapData();
                playerMapdata.canMove = false;
                model.SetActive(false);

            }
		}
	}

    [ClientRpc]
    public void TriggerCall(float speed)
    {
        GetComponent<Animator>().SetFloat("speed", speed);
        GetComponent<Animator>().SetBool("gone", true);
    }

    [ClientRpc]
    public void TriggerReset() 
    {
        StartCoroutine(Reset());
    }

    IEnumerator Reset() 
    {
        yield return new WaitForSecondsRealtime(resetTime);
        GetComponent<Animator>().SetBool("gone", false);
    }
}
