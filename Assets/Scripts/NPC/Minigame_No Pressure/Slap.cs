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

    private readonly SyncList<GameObject> players = new SyncList<GameObject>();

    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D collision)
	{
        if (collision.gameObject.tag.Equals("Player"))
        {
            Debug.Log("Player currently in " + name);
            players.Add(collision.transform.root.gameObject);
        }
            

    }

    [ServerCallback]
    private void OnTriggerExit2D(Collider2D collision)
	{
        if (collision.gameObject.tag.Equals("Player"))
        {
            Debug.Log("Player is out of " + name);
            players.Remove(collision.transform.root.gameObject);
        }
            
    }

	[ServerCallback]
    public void PlayersOut()
    {
        if (isPlatform)
            return;

        foreach (GameObject player in players)
        {
            Debug.Log("Player out from " + name);
            player.GetComponent<PlayerDataForMap>().isLose = true;
        }
	}

    public void TriggerCall(float speed)
    {
        GetComponent<Animator>().SetFloat("speed", speed);
        GetComponent<Animator>().SetBool("gone", true);
    }


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
