using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerDataForMap : NetworkBehaviour
{

    public GameObject playerModel;
    private string curMapName = "";
    private bool haveMapData = false;
    // Start is called before the first frame update
    void Start()
    {
        playerModel.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name.Contains("Map") && GetComponent<PlayerController>().ready) 
        {  
            if (!curMapName.Equals(SceneManager.GetActiveScene().name)) 
            {
                if (transform.childCount > 1)
                {
                    Destroy(transform.GetChild(1).gameObject);
                }
                
                GameObject mapData = GameObject.FindGameObjectWithTag("MapData");
                if (mapData != null && mapData.transform.root == mapData.transform)
                {
                    GameObject duplicate = Instantiate(mapData);
                    duplicate.transform.SetParent(transform);
                    transform.GetComponentInChildren<BaseMapData>().SetMapModel(GetComponent<PlayerController>().charID, transform.GetChild(0).gameObject);
                    transform.GetComponentInChildren<BaseMapData>().SetPostion(GetComponent<PlayerController>().connectID);
                    curMapName = SceneManager.GetActiveScene().name;
                    haveMapData = true;
                    playerModel.SetActive(true);
                }
                else 
                {
                    haveMapData = false;
                    playerModel.SetActive(false);
                }
                
            }
            if(isOwned && haveMapData)
                transform.GetComponentInChildren<BaseMapData>().Movement();
        }
    }
}
