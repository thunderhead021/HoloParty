using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slap : NetworkBehaviour
{
    private Rigidbody2D gravityControl;

    [SyncVar]
    public float gravityScale = 1;

    [ServerCallback]
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag.Equals("Player"))
        {
            PlayerOut(col.gameObject);
        }
        else if(col.gameObject.tag.Equals("Slap"))
        {
            Destroy(this);
        }
    }

    [ClientRpc]
    private void PlayerOut(GameObject player)
    {
        Debug.Log("player out");
    }

    // Start is called before the first frame update
    void Start()
    {
        gravityControl = GetComponent<Rigidbody2D>();
        gravityControl.gravityScale = gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < 0.03f)
        {
            gravityScale = 0;
            gravityControl.gravityScale = gravityScale;
            gravityControl.simulated = false;
            transform.position = new Vector3(0, 0.03f, 0);
        }
    }
}
