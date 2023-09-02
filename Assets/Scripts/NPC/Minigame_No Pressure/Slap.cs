using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slap : NetworkBehaviour
{
    private Rigidbody gravityControl;

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
        gravityControl = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < 0.03f)
        {
            gravityControl.useGravity = false;
            gravityControl.angularDrag = 0;
            transform.position = new Vector3(0, 0.03f, 0);
        }
    }
}
