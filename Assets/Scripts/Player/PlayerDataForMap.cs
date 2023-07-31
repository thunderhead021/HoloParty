using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerDataForMap : NetworkBehaviour
{

    public GameObject playerModel;

    // Start is called before the first frame update
    void Start()
    {
        playerModel.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name.Contains("Map")) 
        {
            if (playerModel.activeSelf == false) 
            {
                playerModel.SetActive(true);
                GameObject mapData = GameObject.FindGameObjectWithTag("MapData");
                GameObject duplicate = Instantiate(mapData);
                duplicate.transform.SetParent(transform);
                transform.GetComponentInChildren<BaseMapData>().SetMapModel(GetComponent<PlayerController>().charID, transform.GetChild(0).gameObject);
                transform.GetComponentInChildren<BaseMapData>().SetPostion(GetComponent<PlayerController>().connectID);
            }
            if(isOwned)
                transform.GetComponentInChildren<BaseMapData>().Movement();
        }
    }
}
