using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerDataForMap : NetworkBehaviour
{

    public GameObject playerModel;
    [SerializeField]
    private string curMapName = "";
    private bool haveMapData = false;
    [SyncVar]
    public Vector3 playerBoardPos = Vector3.zero;
    [SyncVar]
    public bool boardPosUpdate = false;
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
                if (mapData != null && mapData.transform.root == mapData.transform && GetComponent<PlayerController>().charID >= 0)
                {
                    GameObject duplicate = Instantiate(mapData);
                    duplicate.transform.SetParent(transform);
                  
                    transform.GetComponentInChildren<BaseMapData>().SetPostion(GetComponent<PlayerController>().connectID);
                    playerBoardPos = transform.position;
                    boardPosUpdate = false;

                    UpdatePlayerModel();

                }
                else 
                {
                    haveMapData = false;
                    playerModel.SetActive(false);
                }
                
            }
            if (isOwned && haveMapData)
            {
                transform.GetComponentInChildren<BaseMapData>().Movement();
            }
        }
        if (!boardPosUpdate) 
        {
            transform.position = playerBoardPos;
            boardPosUpdate = true;
        }
    }

    public void UpdatePlayerModel() 
    {
        transform.GetComponentInChildren<BaseMapData>().SetMapModel(GetComponent<PlayerController>().charID, transform.GetChild(0).gameObject);
        curMapName = SceneManager.GetActiveScene().name;
        haveMapData = true;
        playerModel.SetActive(true);
        playerModel.transform.GetChild(0).transform.position = Vector3.zero;
    }
}
