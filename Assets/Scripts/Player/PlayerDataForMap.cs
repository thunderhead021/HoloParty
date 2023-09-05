using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Mirror.Experimental;

public class PlayerDataForMap : NetworkBehaviour
{

    public GameObject playerModel;
    [SerializeField]
    private string curMapName = "";
    private bool haveMapData = false;
    [SyncVar]
    public Vector3 playerBoardPos = Vector3.zero;
    [SyncVar]
    public bool notUpdate = false;
    // Start is called before the first frame update
    void Start()
    {
        playerModel.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (notUpdate && transform != null)
        {
            transform.position = playerBoardPos;
            notUpdate = false;
        }
        if (SceneManager.GetActiveScene().name.Contains("Map") && GetComponent<PlayerController>().ready) 
        {  
            if (!curMapName.Equals(SceneManager.GetActiveScene().name)) 
            {
                if (transform.childCount > 1)
                {
                    Destroy(transform.GetChild(1).gameObject);
                    RemoveRB();
                }

                GameObject mapData = GameObject.FindGameObjectWithTag("MapData");
                if (mapData != null && mapData.transform.root == mapData.transform && GetComponent<PlayerController>().charID >= 0)
                {
                    GameObject duplicate = Instantiate(mapData);
                    duplicate.transform.SetParent(transform);
                    UpdatePlayerModel();
                    if (playerBoardPos != Vector3.zero && SceneManager.GetActiveScene().name.Contains("Board"))
                    {
                        notUpdate = true;
                    }
                    else 
                    {
                        transform.GetComponentInChildren<BaseMapData>().SetPostion(GetComponent<PlayerController>().connectID);
                    }
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
    }


    public void UpdatePlayerModel() 
    {
        transform.GetComponentInChildren<BaseMapData>().SetMapModel(GetComponent<PlayerController>().charID, transform.GetChild(0).gameObject);
        curMapName = SceneManager.GetActiveScene().name;
        haveMapData = true;
        playerModel.SetActive(true);
        
    }

    public void AddRB(bool is2D) 
    {
        if (is2D)
        {
            gameObject.AddComponent<Rigidbody2D>();
            gameObject.GetComponent<Rigidbody2D>().angularDrag = 0;
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
            gameObject.AddComponent<NetworkRigidbody2D>();
        }
        else 
        {
            gameObject.AddComponent<Rigidbody>();
            gameObject.GetComponent<Rigidbody>().angularDrag = 0;
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            gameObject.AddComponent<NetworkRigidbody>();
        }
    }

    public void RemoveRB() 
    {
        Rigidbody2D rb2d = gameObject.GetComponent<Rigidbody2D>();
        NetworkRigidbody2D nwrb2d = gameObject.GetComponent<NetworkRigidbody2D>();

        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        NetworkRigidbody nwrb = gameObject.GetComponent<NetworkRigidbody>();

        if (rb != null)
        {
            Destroy(rb);
            Destroy(nwrb);
        }
        else if (rb2d != null) 
        {
            Destroy(rb2d);
            Destroy(nwrb2d);
        }

    }

}
